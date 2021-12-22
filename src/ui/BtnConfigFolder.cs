using Godot;
using System.IO;

public class BtnConfigFolder : Button
{

    public void Open() {
        string href = OS.GetExecutablePath().GetBaseDir() + "\\y3d_fm_configs\\";
        try {
            DirectoryInfo di = new DirectoryInfo(OS.GetExecutablePath().GetBaseDir());
            di.CreateSubdirectory("y3d_fm_configs");
            System.Diagnostics.Process.Start(href);
        } catch(System.Exception e) {
            ErrorLog.instance.Clear();
            ErrorLog.instance.Add("Error creating config folder at " + href, e.ToString(), ErrorLog.LogColor.RED);
            ErrorLog.instance.PopUp();
        }
    }

}