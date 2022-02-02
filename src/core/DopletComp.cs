using Godot;
using System;
using System.Collections.Generic;

public class DopletComp : Node
{
    LineEdit DropletPath;
    LineEdit DropletOuptutPath;
    LineEdit DropletCustomOutputPath;
    FoldersList foldersList;
    [Export]
    NodePath NPFoldersList;

    [Export]
    NodePath NPRenameOptions;
    RenameOptions rn;
    public override void _Ready()
    {
        DropletPath = GetNode<LineEdit>("HbDropletPath/Le");
        DropletOuptutPath = GetNode<LineEdit>("HbDropletOutput/Le");
        DropletCustomOutputPath = GetNode<LineEdit>("HbDropletCustomOutput/Le");
        foldersList = GetNode<FoldersList>(NPFoldersList);
        rn = GetNode<RenameOptions>(NPRenameOptions);
    }

    public void Run()
    {
        string fileStringList = "";

        foreach (string file in GetFiles())
        {
            fileStringList += '"' + file + '"';
        }
        try {
            System.Diagnostics.Process.Start(DropletPath.Text, fileStringList);
        } catch(System.Exception e) {
            ErrorLog.instance.Add("Droplet Fehler", "/ müssen im Droplet Pfad mit \\ ersetzt werden.\n" + e.ToString(), ErrorLog.LogColor.RED);
        }
    }

    List<string> GetFiles()
    {
        List<string> list = new List<string>();
        foreach (KeyValuePair<string, string> folder in foldersList.GetProductFoldersDict())
        {
            string[] files;
            files = RNUtil.TryParseFiles(folder.Key, true);
            foreach (string file in files)
            {
                if (!rn.ignoreFile(rn.UnParseDate(file.GetFile()), file.GetFile()))
                {
                    list.Add(file);
                }
            }
        }
        return list;
    }
}
