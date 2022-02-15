using Godot;
using System;
using System.Collections.Generic;
using ImageMagick;

public class DopletComp : Node
{
    FoldersList foldersList;
    [Export]
    NodePath NPFoldersList;

    [Export]
    NodePath NPRenameOptions;
    RenameOptions rn;
    public override void _Ready()
    {
        foldersList = GetNode<FoldersList>(NPFoldersList);
        rn = GetNode<RenameOptions>(NPRenameOptions);

    }

    public void Run()
    {
        string img = @"C:\Users\simon\Desktop\ct.png";
        string sourceImgTrimmed = @"C:\Users\simon\Desktop\Rockingchair_02.psd";
        using (MagickImage i = new MagickImage(img)) {
            i.Trim();
            i.RePage();
            i.Density = new Density(300, 300);
            i.Depth = 8;
            i.Write(sourceImgTrimmed);
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
