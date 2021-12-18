using Godot;
using System;
using System.Collections.Generic;

public class Main : Control {

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

    [Export]
    NodePath NPTimerShowPreview;
    Timer timerShowPreview;

    public static Main instance;


    RenameOptions options = new RenameOptions();

    public List<FileJob> jobList;

    public override void _Ready() {
        timerShowPreview = GetNode<Timer>(NPTimerShowPreview);
        if (instance == null) {
            instance = this;
        } else {
            GD.Print("Only one Instance of Main is allowed");
        }
    }

    
    bool refreshFiles;
    public override void _Input(InputEvent e) {
        if (e is InputEventKey) {
            timerShowPreview.Start();
            if (GetFocusOwner() == null) {
                return;
            }
            if (GetFocusOwner().Name == "LEF") { // Refresh Files if user types in Folders UI
                refreshFiles = true;
            }
        }
    }

    public void updateOptionsOnly() {
        GD.Print("Update Options");
        options.productFoldersDict = GetNode<FoldersList>(NPProductFolder).GetProductFoldersDict();

        options.replaceWithDict = GetNode<ReplaceWithList>(NPReplaceWith).GetReplaceWithDict();
        options.ignoreFilesList = new List<string>(GetNode<TextEdit>(NPIgnoreFiles).Text.Split("\n"));
        options.removeFilesList = new List<string>(GetNode<TextEdit>(NPRemoveFiles).Text.Split("\n"));

        RNUtil.PrintStringPairDict(options.replaceWithDict);
        jobList = options.ParseFiles(false);
        GetNode<Preview>(NPPreview).Show(jobList);
    }

    // Read all Files and update Joblist
    public void OnUpdateJobList() {
        options.productFoldersDict = GetNode<FoldersList>(NPProductFolder).GetProductFoldersDict();

        options.replaceWithDict = GetNode<ReplaceWithList>(NPReplaceWith).GetReplaceWithDict();
        options.ignoreFilesList = new List<string>(GetNode<TextEdit>(NPIgnoreFiles).Text.Split("\n"));
        options.removeFilesList = new List<string>(GetNode<TextEdit>(NPRemoveFiles).Text.Split("\n"));

        RNUtil.PrintStringPairDict(options.replaceWithDict);
        jobList = options.ParseFiles(refreshFiles);
        GetNode<Preview>(NPPreview).Show(jobList);
        if (refreshFiles) refreshFiles = false;
    }
}
