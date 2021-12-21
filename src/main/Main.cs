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
    NodePath NPMoveToTopFolder;

    [Export]
    NodePath NPReplaceWith;

    [Export]
    NodePath NPPreview;

    [Export]
    NodePath NPRemoveFileNameParts;

    [Export]
    NodePath NPTimerShowPreview;

    [Export]
    NodePath NPLePreFix;

    [Export]
    NodePath NPLeSubFix;
    Timer timerShowPreview;

    public static Main instance;


    RenameOptions options;

    public List<FileJob> jobList;
 
    public override void _Ready() {
        timerShowPreview = GetNode<Timer>(NPTimerShowPreview);
        if (instance == null) {
            instance = this;
            options = GetNode<RenameOptions>("RenameOptions");
        } else {
            GD.Print("Only one Instance of Main is allowed");
        }
    }

    public void StartUpdateTimer() {
        refreshFiles = true;
        timerShowPreview.Start();
    }

    
    bool refreshFiles;
    public override void _Input(InputEvent e) {
        if (e is InputEventKey) {
            timerShowPreview.Start();
            if (GetFocusOwner() == null) {
                return;
            }
            if (GetFocusOwner().Name == "LEF" || GetFocusOwner().Name == "LEP") { // Refresh Files if user types in Folders UI
                refreshFiles = true;
            }
        }
    }

    // Read all Files and update Joblist
    public void OnUpdateJobList() {
        options.productFoldersDict = GetNode<FoldersList>(NPProductFolder).GetProductFoldersDict();

        options.replaceWithDict = GetNode<ReplaceWithList>(NPReplaceWith).GetReplaceWithDict();
        options.ignoreFilesList = new List<string>(GetNode<TextEdit>(NPIgnoreFiles).Text.Split("\n"));
        options.removeFilesList = new List<string>(GetNode<TextEdit>(NPRemoveFiles).Text.Split("\n"));
        options.removeFileNamePartsList = new List<string>(GetNode<TextEdit>(NPRemoveFileNameParts).Text.Split("\n"));
        options.prefix = GetNode<LineEdit>(NPLePreFix).Text;
        options.subfix = GetNode<LineEdit>(NPLeSubFix).Text;
        options.moveToBaseFolders = GetNode<CheckBox>(NPMoveToTopFolder).Pressed;

        jobList = options.ParseFiles(refreshFiles);
        GetNode<Preview>(NPPreview).Show(jobList);
        if (refreshFiles) refreshFiles = false;
    }

    public void OnRun() {
        //TODO avoid runnning while files arent Updated (waiting for Timer)
        FileJob.Execute(jobList);
    }

    public void FromSaveData(SaveData sd) {
        GetNode<ReplaceWithList>(NPReplaceWith).PopulateFromData(sd.replaceDictFrom, sd.replaceDictTo);
        GetNode<FoldersList>(NPProductFolder).PopulateFromData(sd.baseFolders, sd.productNames);
        GetNode<CheckBox>(NPMoveToTopFolder).Pressed = sd.moveToBaseFolders;
        GetNode<TextEdit>(NPIgnoreFiles).Text = string.Join("\n", sd.ignoreFilesList);
        GetNode<TextEdit>(NPRemoveFiles).Text = string.Join("\n", sd.removeFilesList);
        GetNode<TextEdit>(NPRemoveFileNameParts).Text = string.Join("\n", sd.removeNamePartsList);
        GetNode<TextEdit>(NPLePreFix).Text = sd.prefix;
        GetNode<TextEdit>(NPLeSubFix).Text = sd.subfix;
        refreshFiles = true;
        OnUpdateJobList();
    }
}
