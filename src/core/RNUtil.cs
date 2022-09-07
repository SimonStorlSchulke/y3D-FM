using Godot;
using System.Linq;
using System.IO;
using System.Collections.Generic;

public class RNUtil : Node
{

    //File Paths that are forbidden to directly load in
    readonly static string[] blacklistedDirs = new string[]{
        @"\\FILE-SERVER\a",
        @"\\FILE-SERVER\jobs",
        @"\\FILE-SERVER\raid",
        @"H:\",
        @"\\mca-dtp\DFS-Area GLC",
        };
    
    public static string[] TryParseFiles(string path, bool recursive)
    {
        var d = new Godot.Directory();
        bool isDir = d.DirExists(path);
        string[] files = { };
        if (blacklistedDirs.Contains(path))
        {
            ErrorLog.instance.Add("CANNOT DIRECTLY ACCESS BLACKLISTED DIRECTORY " + path, "blacklisted Directories are:\n" + string.Join("\n", blacklistedDirs), ErrorLog.LogColor.RED);
            ErrorLog.instance.PopUp();
            return files;
        }
        try
        {
            if (isDir)
                files = recursive ? System.IO.Directory.GetFiles(path, "*", System.IO.SearchOption.AllDirectories) : System.IO.Directory.GetFiles(path);
            else
                files = new string[] { path };
        }
        catch (System.Exception e)
        {
            ErrorLog.instance.Add("Error reading files", e.ToString(), ErrorLog.LogColor.RED);
        }
        return files;
    }


    public static void PrintStringPairDict(Dictionary<string, string> dict)
    {
        foreach (KeyValuePair<string, string> kvp in dict)
        {
            GD.Print("Key = ", kvp.Key, "    Value = ", kvp.Value);
        }
    }

    public static void PrintStringList(List<string> list)
    {
        foreach (string str in list)
        {
            GD.Print(str);
        }
    }
}
