using Godot;
using System;
using System.Collections.Generic;

public class Main : Control
{

    [Export] NodePath NPProductFolder;

    [Export] NodePath NPDate;

    [Export] NodePath NPRemoveFiles;

    [Export] NodePath NPIgnoreFiles;

    [Export] NodePath NPMoveToTopFolder;
    [Export] NodePath NPMoveToPath;

    [Export] NodePath NPReplaceWith;

    [Export] NodePath NPPreview;

    [Export] NodePath NPRemoveFileNameParts;

    [Export] NodePath NPTimerShowPreview;

    [Export] NodePath NPLePreFix;

    [Export] NodePath NPLeSubFix;
    Timer timerShowPreview;

    public static Main instance;

    public RenameOptions options;
    public List<FileJob> jobList;
    public List<FileJob> JobListExistingFiles;

    public override void _Ready()
    {
        timerShowPreview = GetNode<Timer>(NPTimerShowPreview);
        if (instance == null)
        {
            instance = this;
            options = GetNode<RenameOptions>("RenameOptions");
        }
        else
        {
            GD.Print("Only one Instance of Main is allowed");
        }
        FoldersList.instance.OpenArgsPaths();

        int d = DateTime.Now.Day;
        int m = DateTime.Now.Month;


        // Andy zum Geburtstag gratulieren (wichtig)
        if (d == 14 && m == 5) {
            GetNode<Control>("hb").Visible = true;
        } else if ((DateTime.Now.DayOfWeek == DayOfWeek.Monday) && ((d == 15 && m == 5) || (d == 16 && m == 5))) {
            GetNode<Control>("hb").Visible = true;
            GetNode<Label>("hb/ct/vb/Label").Text = "Happy Birthday (im Nachhinein) Andy!";
        }
    }

    public void hideit() {
        GetNode<Control>("hb").Visible = false;
    }

    /// <summary> A timer is used to update the Rename-Preview. To save performance, it is not updated instantly after each
    /// User input, but after a short time without user-input </summary>
    public void StartUpdateTimer()
    {
        refreshFiles = true;
        timerShowPreview.Start();
    }


    bool refreshFiles;
    public override void _Input(InputEvent e)
    {
        if (e is InputEventKey)
        {
            timerShowPreview.Start();
            if (GetFocusOwner() == null)
            {
                return;
            }
            if (GetFocusOwner().Name == "LEF" || GetFocusOwner().Name == "LEP")
            {   // Refresh Files if user types in Folders UI
                refreshFiles = true;
            }
        }
    }

    // Read all Files and update Joblist
    public void OnUpdateJobList()
    {
        options.productFoldersDict = GetNode<FoldersList>(NPProductFolder).GetProductFoldersDict();

        options.date = GetNode<LineEdit>(NPDate).Text;
        options.replaceWithDict = GetNode<ReplaceWithList>(NPReplaceWith).GetReplaceWithDict();
        options.ignoreFilesList = new List<string>(GetNode<TextEdit>(NPIgnoreFiles).Text.Split("\n"));
        options.removeFilesList = new List<string>(GetNode<TextEdit>(NPRemoveFiles).Text.Split("\n"));
        options.removeFileNamePartsList = new List<string>(GetNode<TextEdit>(NPRemoveFileNameParts).Text.Split("\n"));
        options.prefix = GetNode<LineEdit>(NPLePreFix).Text;
        options.subfix = GetNode<LineEdit>(NPLeSubFix).Text;
        options.moveToBaseFolders = GetNode<CheckBox>(NPMoveToTopFolder).Pressed;
        options.moveToPath = GetNode<LineEdit>(NPMoveToPath).Text;

        jobList = options.ParseFiles(refreshFiles);
        GetNode<Preview>(NPPreview).Show(jobList);
        if (refreshFiles) refreshFiles = false;
    }

    public void OnRun()
    {
        //TODO avoid runnning while files arent Updated (waiting for Timer)
        ErrorLog.instance.Clear();
        if (jobList != null)
        {
            FileJob.Execute(jobList);
        }
    }

    /// <summary> updates the UI from the given SaveData object </summary>
    public void FromSaveData(SaveData sd)
    {
        GetNode<ReplaceWithList>(NPReplaceWith).PopulateFromData(sd.replaceDictFrom, sd.replaceDictTo);
        GetNode<CheckBox>(NPMoveToTopFolder).Pressed = sd.moveToBaseFolders;
        GetNode<TextEdit>(NPIgnoreFiles).Text = string.Join("\n", sd.ignoreFilesList);
        GetNode<TextEdit>(NPRemoveFiles).Text = string.Join("\n", sd.removeFilesList);
        GetNode<TextEdit>(NPRemoveFileNameParts).Text = string.Join("\n", sd.removeNamePartsList);
        GetNode<LineEdit>(NPLePreFix).Text = sd.prefix;
        GetNode<LineEdit>(NPLeSubFix).Text = sd.subfix;
        GetNode<LineEdit>(NPMoveToPath).Text = sd.moveToPath;
        refreshFiles = true;
        OnUpdateJobList();
    }
}
