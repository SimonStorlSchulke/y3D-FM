using Godot;
using System.Collections.Generic;

public class FoldersList : VBoxContainer
{

    public override void _Ready() {
        GetTree().Connect("files_dropped", this, nameof(OnFilesDropped));
    }

    /// <summary>Returns Productfolders (Folder + Productname)from the UI as Dictionary .</summary>
    public Dictionary<string, string> GetProductFoldersDict() {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        foreach (FolderLineEdit fe in GetChildren())
        {
            if(!dict.ContainsKey(fe.leFolder.Text) && fe.leFolder.Text != "") {
                dict.Add(fe.leFolder.Text, fe.leProduct.Text);
            }
        }
        return dict;
    }

    public void OnFilesDropped(string[] files, int screen) {
        var d = new Directory();
        bool has_updated = false;
        foreach (var f in files)
        {

            if (d.DirExists(f)) {
                // If dropped files are folders, add them, but only if the folder isn't added already
                bool alreadyAdded = false;
                foreach (Node c in GetChildren()) {
                    if ((c as FolderLineEdit).leFolder.Text == f) {
                        alreadyAdded = true;
                    }
                }
                if (alreadyAdded) {
                    continue;
                }

                has_updated = true;

                FolderLineEdit lastChild = GetChild<FolderLineEdit>(GetChildCount()-1);

                if (lastChild.leFolder.Text == "") {
                    lastChild.leFolder.Text = f;
                    lastChild.leProduct.Text = f.GetFile();

                    FolderLineEdit dp = lastChild.Duplicate() as FolderLineEdit;
                    AddChild(dp);
                    dp.leFolder.Text = "";
                    dp.leProduct.Text = "";
                } else {
                    FolderLineEdit dp = lastChild.Duplicate() as FolderLineEdit;
                    AddChild(dp);
                    dp.leFolder.Text = f;
                    dp.leProduct.Text = f.GetFile();
                }
            }
        }
        if (has_updated) {
            Main.instance.StartUpdateTimer();
        }
    }

    public void PopulateFromData(string[] from, string[] to) {
        FolderLineEdit inp = GetChild(0).Duplicate() as FolderLineEdit;
        foreach (Node c in GetChildren()) {
            c.QueueFree();
        }

        for (int i = 0; i < from.Length; i++) {
            FolderLineEdit dp = inp.Duplicate() as FolderLineEdit;
            AddChild(dp);
            dp.leFolder.Text = from[i];
            dp.leProduct.Text = to[i];
        }

    }
}
