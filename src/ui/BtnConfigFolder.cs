using Godot;
using System.IO;

public class BtnConfigFolder : Button
{

    /// <summary> Opens an File-Explorer Window with the folder containing .yfm savefiles. </summary>
    public void Open()
    {
        string href = OS.GetExecutablePath().GetBaseDir() + "\\y3d_fm_configs";
        href = href.Replace("//", "\\\\").Replace("/", "\\"); // Strangely required for this to work with UNC Paths
        try
        {
            DirectoryInfo di = new DirectoryInfo(OS.GetExecutablePath().GetBaseDir());
            di.CreateSubdirectory("y3d_fm_configs");
            System.Diagnostics.Process.Start(href);
        }
        catch (System.Exception e)
        {
            ErrorLog.instance.Clear();
            ErrorLog.instance.Add("Error creating / opening config folder at " + href, e.ToString(), ErrorLog.LogColor.RED);
            ErrorLog.instance.PopUp();
        }
    }
}