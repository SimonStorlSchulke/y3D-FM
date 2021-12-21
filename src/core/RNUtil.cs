using Godot;
using System;
using System.IO;
using System.Collections.Generic;

public class RNUtil : Node
{
    // TODO handle warnings
        public static string[] TryParseFiles(string path, bool recursive)
    {
        string[] files = { };
        try
        {
            files = recursive ? System.IO.Directory.GetFiles(path, "*", System.IO.SearchOption.AllDirectories) : System.IO.Directory.GetFiles(path);
        }
        catch (System.Exception e)
        {
            ErrorLog.instance.Add("Error reading files", e.ToString(), ErrorLog.LogColor.RED);
        }
        return files;
    }

    public static List<string> TryParseDirs(string path, bool recursive)
    {
        List<string> dirs;
        try
        {
            string[] dirsArr = recursive ? System.IO.Directory.GetDirectories(path, "*", SearchOption.AllDirectories) : System.IO.Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
            dirs = new List<string>(dirsArr);
            dirs.Add(path);
        }
        catch (System.Exception e)
        {
            ErrorLog.instance.Add("Error reading directories", e.ToString(), ErrorLog.LogColor.RED);

            dirs = new List<string>();
        }
        return dirs;
    }

    public static void PrintStringPairDict(Dictionary<string, string> dict) {
        foreach (KeyValuePair<string, string> kvp in dict)
        {
            GD.Print("Key = ", kvp.Key, "    Value = ", kvp.Value);
        }
    }

    public static void PrintStringList(List<string> list) {
        foreach (string str in list)
        {
            GD.Print(str);
        }
    }
}
