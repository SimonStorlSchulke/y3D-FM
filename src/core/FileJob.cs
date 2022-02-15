using System.Collections.Generic;
using Godot;
public struct FileJob {

    public string pathOriginal;
    public string pathDestination;

    public FileJob(string from, string to) {
        pathOriginal = from;
        pathDestination = to;
    }

    /// <summary>Exectues the FIleJob and returns errors if there were any.</summary>
    public static void Execute(List<FileJob> jobList) {
        bool overwrite = Main.instance.options.overwrite;
        bool had_errors = false;
        ErrorLog.instance.Clear();
        foreach (FileJob job in jobList) {
            if (job.pathDestination == "DELETE") { // kinda dirty ;)
                System.IO.File.Delete(job.pathOriginal);
                continue;
            }
            try {
                if (overwrite && System.IO.File.Exists(job.pathDestination)) {
                    System.IO.File.Delete(job.pathDestination);
                }
                System.IO.File.Move(job.pathOriginal, job.pathDestination);
            } catch (System.Exception e) {
                had_errors = true;
                ErrorLog.instance.Add(
                    job.pathOriginal.GetFile() + " could not be renamed to " + job.pathDestination.GetFile(),
                    e.ToString(),
                    ErrorLog.LogColor.RED);
            }
        }
        if (had_errors) {
            ErrorLog.instance.PopUp();
        }
    }
}
