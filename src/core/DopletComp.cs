using Godot;
using System;
using System.Collections.Generic;
using ImageMagick;
using System.Threading;
using System.Linq;

/// <summary> For executing <see cref="BatchPresets"/> on the loaded files </summary>
public class DopletComp : Node
{
    FoldersList foldersList;
    [Export] NodePath NPFoldersList;
    [Export] NodePath NPOutputFolder;
    [Export] NodePath NPOutputFolderJpg;

    [Export] NodePath NPRenameOptions;

    [Export] NodePath NPProgressBar;
    ProgressBar progressBar;
    public static RichTextLabel lblProcessed;
    OptionButton presetSelector;
    LineEdit lePresetOptions;
    Label lblPresetDescription;
    RenameOptions rn;
    List<Tuple<string, string>> files;
    Godot.Timer tmr;
    BatchProcess process;
    public static string outputFolderJPG;
    int atImage = 0;
    public static DopletComp instance;

    public static readonly string[] supportedFormats = { "png", "jpg", "jpeg", "tiff", "tif", "tga", "exr", "bmp", "psd" };
    
    public override void _Ready()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            GD.Print("Only one Instance of DropletComp is allowed");
        }

        foldersList = GetNode<FoldersList>(NPFoldersList);
        rn = GetNode<RenameOptions>(NPRenameOptions);
        progressBar = GetNode<ProgressBar>(NPProgressBar);
        tmr = GetNode<Godot.Timer>("tmrNextImage");
        lblProcessed = GetNode<RichTextLabel>("LblProcessed");
        presetSelector = GetNode<OptionButton>("HbPreset/ObPreset");
        lePresetOptions = GetNode<LineEdit>("HbOptions/LeOptions");
        lblPresetDescription = GetNode<Label>("LblPresetDescription");

        foreach (var process in BatchPresets.list)
        {
            presetSelector.AddItem(process.name);
        }

        process = BatchPresets.list[presetSelector.Selected];
        UpdatePresetOptions(0);
    }

    public void OnOutputFolderChanged(string path) {
        GetNode<Label>(NPOutputFolderJpg).Text = path + "_jpg";
    }

    public void OpenPsdFolder() {
        string path = instance.GetNode<LineEdit>(instance.NPOutputFolder).Text;
        RNUtil.CreateDirIfNotExisting(path);
        try {
            System.Diagnostics.Process.Start(path);
        } catch {

        }
    }

    public void OpenJpgFolder() {
        string path = instance.GetNode<LineEdit>(instance.NPOutputFolder).Text + "_jpg";
        RNUtil.CreateDirIfNotExisting(path);
        try {
            System.Diagnostics.Process.Start(path);
        } catch {

        }
    }

    public static string GetSaveDestination(string filename, bool createJpgFolder, string productname = "Unknown Product", bool sort = true)
    {

        string[] paths;
        if (sort)
        {
            string subfoldername = productname;
            paths = new string[]{
            instance.GetNode<LineEdit>(instance.NPOutputFolder).Text,
            DateTime.Now.ToString("yyyyMMdd"),
            subfoldername,
            System.IO.Path.GetFileNameWithoutExtension(filename)};
        }
        else
        {
            paths = new string[]{
            instance.GetNode<LineEdit>(instance.NPOutputFolder).Text,
            DateTime.Now.ToString("yyyyMMdd"),
            System.IO.Path.GetFileNameWithoutExtension(filename)};
        }

        string fullPath = System.IO.Path.Combine(paths);
        string parentFolder = System.IO.Directory.GetParent(fullPath).FullName;

        if (!System.IO.Directory.Exists(parentFolder))
        {
            System.IO.Directory.CreateDirectory(parentFolder);
        }

        return System.IO.Path.Combine(paths);
    }

    public void UpdatePresetOptions(int i)
    {
        process = BatchPresets.list[i];
        lblPresetDescription.Text = process.description;
        lePresetOptions.Text = process.defaultOptions;
    }

    bool saveJpg;
    bool savePsd;
    public void Run()
    {
        saveJpg = GetNode<CheckBox>("HbOutputJpg/CbJpg").Pressed;
        savePsd = GetNode<CheckBox>("HbOutput/CbPsd").Pressed;
        process = BatchPresets.list[presetSelector.Selected];
        process.defaultOptions = lePresetOptions.Text;

        Main.instance.OnUpdateJobList();
        files = GetFiles();
        progressBar.Value = 0;
        if (files.Count < 1)
        {
            lblProcessed.BbcodeText = "nothing to process - add one or more folders to the List on the left first.";
            return;
        }
        lblProcessed.BbcodeText = "[font=res://theme/new_dynamicfont.tres]Processed Images[/font]	";
        GetNode<Button>("HbRun/BtnRun").Text = "Running... ESC to Cancel";
        atImage = 0;
        tmr.Start();
    }


    void ProcessImages()
    {
        if (Input.IsActionJustReleased("ui_cancel") || Input.IsActionJustPressed("ui_cancel") || Input.IsActionPressed("ui_cancel"))
        {
            atImage = 0;
            GetNode<Button>("HbRun/BtnRun").Text = "Run Batch Process";
            progressBar.Value = 0;
            return;
        }

        string outputFolder = GetNode<LineEdit>(NPOutputFolder).Text;
        outputFolderJPG = outputFolder + "_jpg";
        bool supported = supportedFormats.Contains(files[atImage].Item1.Extension());

        bool isRenderElement = false;
        string path = files[atImage].Item1;
        if (path.Contains(".VRayAO") || path.Contains(".bumpNormals") || path.Contains("Shadow_") || path.Contains(".txt"))
        { //TODO unmagicify
            isRenderElement = true;
        }

        if (supported && !isRenderElement)
        {
            using (MagickImage img = new MagickImage(files[atImage].Item1))
            {
                try
                {
                    string dest = GetSaveDestination(img.FileName, saveJpg, files[atImage].Item2);
                    process.function(img, lePresetOptions.Text, dest, savePsd, saveJpg);
                    lblProcessed.BbcodeText += "\n[color=lime]Processed[/color] " + dest;
                }
                catch (System.Exception e)
                {
                    lblProcessed.BbcodeText += "\n[color=red]Could not Process[/color] " + files[atImage].Item1.GetFile() + " " + e;
                }
            }
        }
        else if (!supported && !isRenderElement)
        {
            lblProcessed.BbcodeText += "\n[color=red]Could not Process (Filetype not supported)[/color] " + files[atImage].Item1.GetFile();
        }
        atImage++;
        progressBar.Value = ((float)atImage / (float)files.Count) * 100;
        if (atImage < files.Count)
        {
            tmr.Start();
        }
        else
        {
            GetNode<Button>("HbRun/BtnRun").Text = "Run Batch Process";
        }
    }

    List<Tuple<string, string>> GetFiles()
    {
        List<Tuple<string, string>> list = new List<Tuple<string, string>>();
        foreach (KeyValuePair<string, string> folder in foldersList.GetProductFoldersDict())
        {
            string[] files;
            files = RNUtil.TryParseFiles(folder.Key, true);
            foreach (string file in files)
            {
                if (!rn.ignoreFile(rn.UnParseDate(file.GetFile()), file.GetFile()) && !file.EndsWith("Thumbs.db"))
                {
                    list.Add(new Tuple<string, string>(file, folder.Value));
                }
            }
        }
        return list;
    }
}
