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
        string warnings = "Fehler:\n";
        foreach (FileJob job in jobList)
        {
             try {
                 System.IO.File.Move(job.pathOriginal, job.pathDestination);
             } catch (System.Exception e) {
                warnings += job.pathOriginal.GetFile() + " konnte nicht zu " + job.pathDestination.GetFile() + " umbenannt werden: ";
                warnings += "\n" + e.ToString() + "\n";
             }
        }
        return warnings;
    }
}
