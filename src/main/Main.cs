using Godot;
using System;
using System.Collections.Generic;

public class Main : Node
{

    [Export]
    NodePath NPProductFolder;

    [Export]
    NodePath NPRemoveFiles;

    [Export]
    NodePath NPIgnoreFiles;

    [Export]
    NodePath NPReplaceWith;

    [Export]
    NodePath NPPreview;

    [Export]
    NodePath NPRemoveFileNameParts;


    RenameOptions options = new RenameOptions();

    public List<FileJob> jobList;
    
    public override void _Ready()
    {
        
    }

    public void OnGenerateJobList() {
        OnUpdateOptions();
    }

    public void OnUpdateOptions() {
        options.productFoldersDict =  GetNode<FoldersList>(NPProductFolder).GetProductFoldersDict();
        options.replaceWithDict = GetNode<ReplaceWithList>(NPReplaceWith).GetReplaceWithDict();
        options.ignoreFilesList = new List<string>(GetNode<TextEdit>(NPIgnoreFiles).Text.Split("\n"));
        
        RNUtil.PrintStringPairDict(options.replaceWithDict);
        jobList = options.ParseFiles();
        GetNode<Preview>(NPPreview).Show(jobList);
    }
}
