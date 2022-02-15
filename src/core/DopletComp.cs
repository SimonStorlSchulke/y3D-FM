using Godot;
using System;
using System.Collections.Generic;
using ImageMagick;
using System.Threading;

public class DopletComp : Node
{
    FoldersList foldersList;
    [Export]
    NodePath NPFoldersList;
    [Export]
    NodePath NPOutputFolder;

    [Export]
    NodePath NPRenameOptions;

    [Export]
    NodePath NPProgressBar;
    ProgressBar progressBar;
    RichTextLabel lblProcessed;
    OptionButton presetSelector;
    LineEdit lePresetOptions;
    RenameOptions rn;
    List<string> files;
    Godot.Timer tmr;
    BatchProcess process;
    int atImage = 0;
    public static readonly string[] supportedFormats = {"png", "jpg", "jpeg", "tiff", "tga", "exr", "bmp", "psd"};
    public override void _Ready()
    {
        foldersList = GetNode<FoldersList>(NPFoldersList);
        rn = GetNode<RenameOptions>(NPRenameOptions);
        progressBar = GetNode<ProgressBar>(NPProgressBar);
        tmr = GetNode<Godot.Timer>("tmrNextImage");
        lblProcessed = GetNode<RichTextLabel>("LblProcessed");
        presetSelector = GetNode<OptionButton>("HbPreset/ObPreset");
        lePresetOptions = GetNode<LineEdit>("HbOptions/LeOptions");

        foreach (var process in BatchPresets.list) {
            presetSelector.AddItem(process.name);
        }

        process = BatchPresets.list[presetSelector.Selected];
        UpdatePresetOptions(0);
    }

    public void UpdatePresetOptions(int i) {
        process = BatchPresets.list[i];
        lePresetOptions.Text = process.options;
    }

    public void Run()
    {
        process = BatchPresets.list[presetSelector.Selected];
        process.options = lePresetOptions.Text;

        Main.instance.OnUpdateJobList();
        files = GetFiles();
        progressBar.Value = 0;
        if (files.Count < 1) {
            lblProcessed.BbcodeText = "nothing to process - add one or more folders to the List on the left first.";
            return;
        }
        lblProcessed.BbcodeText = "[font=res://theme/new_dynamicfont.tres]Processed Images[/font]	";
        GetNode<Button>("HbRun/BtnRun").Text = "Running... ESC to Cancel";
        atImage = 0;
        tmr.Start();
    }

    void ProcessImages() {
        
        if (Input.IsActionJustReleased("ui_cancel") || Input.IsActionJustPressed("ui_cancel") || Input.IsActionPressed("ui_cancel")) {
            atImage = 0;
            GetNode<Button>("HbRun/BtnRun").Text = "Run Batch Process";
            progressBar.Value = 0;
            return;
        }

        string outputFolder = GetNode<LineEdit>(NPOutputFolder).Text;
        bool supported = false;
        foreach (string format in supportedFormats) {
            if (files[atImage].Extension() == format) {supported = true;}
        }
        if (supported) {
            using (MagickImage img = new MagickImage(files[atImage]))
            {
                try {
                process.function(img);

                string[] paths = {outputFolder, System.IO.Path.GetFileNameWithoutExtension(files[atImage]) + ".psd"};
                string destName = System.IO.Path.Combine(paths);
                img.Write(destName);
                lblProcessed.BbcodeText  += "\n[color=lime]Processed[/color] " + destName.GetFile();
                } catch(System.Exception e) {
                    lblProcessed.BbcodeText  += "\n[color=red]Could not Process[/color] " + files[atImage].GetFile() + " " + e.Message;
                }
            }
        } else {
            lblProcessed.BbcodeText  += "\n[color=red]Could not Process (Filetype not supported)[/color] " + files[atImage].GetFile();
        }
        atImage++;
        progressBar.Value = ((float) atImage / (float)files.Count) * 100;
        if (atImage < files.Count) {
            tmr.Start();
        } else {
            GetNode<Button>("HbRun/BtnRun").Text = "Run Batch Process";
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
