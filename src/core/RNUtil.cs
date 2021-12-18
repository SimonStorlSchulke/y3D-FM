using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RNUtil : Node {

    static string[] files = { };
    static Task parseWorker(string path) {
        return Task.Factory.StartNew(() => {
            files = System.IO.Directory.GetFiles(path, "*", System.IO.SearchOption.AllDirectories);
        });
    }

    static async void Parse(string path) {
        try {
            await parseWorker(path);
            GD.Print("Getting Files compleded");
            Main.instance.updateOptionsOnly();
        } catch (System.Exception e) {
            //Warning.instance.Warn("Fehler beim auslesen der Dateien:\n" + a.ToString());
        }
    }

    // TODO handle warnings
    public static string[] TryParseFiles(string path, bool recursive) {
        try {
            Parse(path);
        } catch (System.Exception e) {
            //Warning.instance.Warn("Fehler beim auslesen der Dateien:\n" + a.ToString());
        }
        return files;
    }

    public static List<string> TryParseDirs(string path, bool recursive) {
        List<string> dirs;
        string warnings = "Ordner konnte nicht gelesen werden.\n"; ;
        try {
            string[] dirsArr = recursive ? System.IO.Directory.GetDirectories(path, "*", SearchOption.AllDirectories) : System.IO.Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
            dirs = new List<string>(dirsArr);
            dirs.Add(path);
        } catch (System.Exception e) {
            warnings += e.ToString();
            //Warning.instance.Warn(warnings);

            dirs = new List<string>();
        }
        return dirs;
    }

    public static void PrintStringPairDict(Dictionary<string, string> dict) {
        foreach (KeyValuePair<string, string> kvp in dict) {
            GD.Print("Key = ", kvp.Key, "    Value = ", kvp.Value);
        }
    }

    public static void PrintStringList(List<string> list) {
        foreach (string str in list) {
            GD.Print(str);
        }
    }
}
