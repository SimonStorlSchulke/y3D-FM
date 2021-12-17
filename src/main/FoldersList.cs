using Godot;
using System.Collections.Generic;

public class FoldersList : VBoxContainer
{
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
}
