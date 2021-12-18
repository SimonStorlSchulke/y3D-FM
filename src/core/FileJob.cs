using System.Collections.Generic;
using Godot;
public struct FileJob
{
    
    public string pathOriginal;
    public string pathDestination;

    public FileJob(string from, string to) {
        pathOriginal = from;
        pathDestination = to;
    }

    /// <summary>Exectues the FIleJob and returns errors if there were any.</summary>
    public static string Execute(List<FileJob> jobList) 
    {
        string errors = "Fehler:\n";
        foreach (FileJob job in jobList)
        {
            if (job.pathDestination == "DELETE") { // kinda dirty ;)
                System.IO.File.Delete(job.pathOriginal);
            }
             try {
                 System.IO.File.Move(job.pathOriginal, job.pathDestination);
             } catch (System.Exception e) {
                errors += job.pathOriginal.GetFile() + " konnte nicht zu " + job.pathDestination.GetFile() + " umbenannt werden: ";
                errors += "\n" + e.ToString() + "\n";
             }
        }
        return errors;
    }
}
