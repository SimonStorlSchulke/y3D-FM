using Godot;
using System.Collections.Generic;
using System;

public class FoldersList : VBoxContainer
{

    [Export] PackedScene fEdit;

    public static FoldersList instance;

    public override void _Ready()
    {
        instance = this;
        GetTree().Connect("files_dropped", this, nameof(OnFilesDropped));
    }

    public void OpenArgsPaths() {
        string[] args = OS.GetCmdlineArgs();
        
        if (args.Length == 0) {
            return;
        }
        
        OnFilesDropped(args, 0);
    }

    /// <summary> Update the folders and files currently loaded to the updated paths after running the renamer. </summary>
    public void UpdateFileUI(string from, string to)
    {
        // Brute force check all FolderLineEdits if the file to rename is there. Not exactly elegant but performance seems ok.
        foreach (FolderLineEdit fe in GetChildren())
        {
            if (fe.leFolder.Text == from)
            {
                fe.leFolder.Text = to;
            }
        }
    }

    /// <summary>Returns Productfolders (Folder / File + Productname)from the UI as Dictionary .</summary>
    public Dictionary<string, string> GetProductFoldersDict()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        foreach (FolderLineEdit fe in GetChildren())
        {
            if (!dict.ContainsKey(fe.leFolder.Text) && fe.leFolder.Text != "")
            {
                dict.Add(fe.leFolder.Text, fe.leProduct.Text);
            }
        }
        return dict;
    }

    public bool IsSubfolder(string parentPath, string childPath)
    {
        Uri parentUri;
        try
        {
            parentUri = new Uri(parentPath);
        }
        catch
        {
            return false;
        }

        var childUri = new System.IO.DirectoryInfo(childPath).Parent;
        while (childUri != null)
        {
            if (new Uri(childUri.FullName) == parentUri)
            {
                return true;
            }
            childUri = childUri.Parent;
        }
        return false;
    }


    public void OnFilesDropped(string[] files, int screen)
    {
        var d = new Directory();
        bool has_updated = false;
        bool hadErrors = false;
        ErrorLog.instance.Clear();
        foreach (var droppedFile in files)
        {
            // If dropped files are folders, add them, but only if the folder isn't added already
            bool alreadyAdded = false;
            bool isParentFolder = false;
            foreach (Node c in GetChildren())
            {
                string fe = (c as FolderLineEdit).leFolder.Text;
                if (fe != "" && IsSubfolder(fe, droppedFile))
                {
                    alreadyAdded = true;
                }
                if (fe != "" && fe.Contains(droppedFile))
                {
                    isParentFolder = true;
                }
            }
            if (alreadyAdded)
            {
                ErrorLog.instance.Add("Could not add Folder", "Folder " + droppedFile + " was already added or is a subfolder of one already added", ErrorLog.LogColor.YELLOW);
                hadErrors = true;
                continue;
            }
            if (isParentFolder)
            {
                ErrorLog.instance.Add("Could not add Folder", "Folder " + droppedFile + " is a parent folder of one already added", ErrorLog.LogColor.YELLOW);
                hadErrors = true;
                continue;
            }

            has_updated = true;

            FolderLineEdit lastChild = GetChild<FolderLineEdit>(GetChildCount() - 1);
            bool isFolder = d.DirExists(droppedFile);
            string productName = isFolder ? droppedFile.GetFile() : "";

            if (lastChild.leFolder.Text == "")
            {
                lastChild.leFolder.Text = droppedFile;
                lastChild.leProduct.Text = productName;

                FolderLineEdit dp = lastChild.Duplicate() as FolderLineEdit;
                AddChild(dp);
                dp.leFolder.Text = "";
                dp.leProduct.Text = "";
            }
            else
            {
                FolderLineEdit dp = lastChild.Duplicate() as FolderLineEdit;
                AddChild(dp);
                dp.leFolder.Text = droppedFile;
                dp.leProduct.Text = productName;
            }

        }
        if (has_updated) Main.instance.StartUpdateTimer();
        if (hadErrors) ErrorLog.instance.PopUp();
    }

    public void PopulateFromData(string[] from, string[] to)
    {

        foreach (Node c in GetChildren())
        {
            c.QueueFree();
        }

        for (int i = 0; i < from.Length; i++)
        {
            FolderLineEdit dp = fEdit.Instance<FolderLineEdit>();
            AddChild(dp);
            dp.leFolder.Text = from[i];
            dp.leProduct.Text = to[i];
        }

        AddChild(fEdit.Instance<FolderLineEdit>());
    }
}
