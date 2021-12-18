using Godot;
using System;
using System.Collections.Generic;

public class Preview : Control {
    GridContainer table;
    CheckBox cbShowFullPathsOrig;
    CheckBox cbShowFullPathsDest;
    CheckBox cbShowColorCodes;
    Dictionary<string, string> colorCodes = new Dictionary<string, string>() {
        {"-20", "b8ff00"},
        {"-21", "000000"},
        {"-22", "ff0000"},
        {"-23", "0093fa"},
        {"-24", "ffec00"},
        {"-25", "00af5b"},
        {"-26", "a000ff"},
        {"-27", "b98554"},
        {"-28", "b1b1b1"},
        {"-29", "ffffff"},
        {"-30", "ffa400"},
    };

    Dictionary<string, string> colorNames = new Dictionary<string, string>() {
        {"gruengelb", "b8ff00"},
        {"schwarz", "000000"},
        {"rot", "ff0000"},
        {"blau", "0093fa"},
        {"gelb", "ffec00"},
        {"gruen", "00af5b"},
        {"magenta", "a000ff"},
        {"braun", "b98554"},
        {"grau", "b1b1b1"},
        {"weiss", "ffffff"},
        {"orange", "ffa400"},
        {"DELETE", "a84242"},
    };

    [Export]
    PackedScene tbPresetL1;

    [Export]
    PackedScene tbPresetL2;

    public override void _Ready() {
        cbShowFullPathsOrig = GetNode<CheckBox>("HbPreviewTop/CbShowFullPathsOrig");
        cbShowFullPathsDest = GetNode<CheckBox>("HbPreviewTop/CbShowFullPathsDest");
        cbShowColorCodes = GetNode<CheckBox>("HbPreviewTop/CbShowColorCodes");
        table = GetNode<GridContainer>("Scrl/Grid");
    }

    public void Show(List<FileJob> jobs) {
        
        foreach (ToolButton item in table.GetChildren()) {
            item.QueueFree();
        }

        int i=0;
        foreach (FileJob job in jobs) {
            
            ToolButton tbOrig = (i%2==0) ? tbPresetL1.Instance<ToolButton>() : tbPresetL2.Instance<ToolButton>();
            ToolButton tbDest = tbOrig.Duplicate() as ToolButton;

            tbOrig.Text = cbShowFullPathsOrig.Pressed ? job.pathOriginal : job.pathOriginal.GetFile();
            tbDest.Text = cbShowFullPathsDest.Pressed ? job.pathDestination : job.pathDestination.GetFile();
            
            if (cbShowColorCodes.Pressed) {
                tbOrig.AddColorOverride("font_color", checkForColorCode(job.pathOriginal.GetFile()));
                tbDest.AddColorOverride("font_color", checkForColorCode(job.pathDestination.GetFile()));
            }

            tbOrig.Connect("pressed", this, nameof(OnFileClicked), new Godot.Collections.Array(){job.pathOriginal});
            tbDest.Connect("pressed", this, nameof(OnFileClicked), new Godot.Collections.Array(){job.pathOriginal});

            table.AddChild(tbOrig);
            table.AddChild(tbDest);

            i += 1;
        }

    }


    public void OnFileClicked(string href) {
        try {
            System.Diagnostics.Process.Start(href);
        } catch(System.Exception e) {
            //Error handling
        }
    }

    Color checkForColorCode(string str) {
        string color = "cccccc";
        foreach (KeyValuePair<string, string> c in colorCodes) {
            if (str.Contains(c.Key)) {
                color = c.Value;
                break;
            }
        }
        foreach (KeyValuePair<string, string> c in colorNames) {
            if (str.Contains(c.Key)) {
                color = c.Value;
                break;
            }
        }
        return new Color(color);
    }

}
