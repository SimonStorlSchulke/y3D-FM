using Godot;
using System;
using System.Collections.Generic;

public class Preview : Control
{
    RichTextLabel teOriginal;
    RichTextLabel teDestination;
    CheckBox cbGeneratePreview;
    CheckBox cbShowFullPaths;
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
    };

    public override void _Ready()
    {
        teOriginal = GetNode<RichTextLabel>("HbPreview/TeOriginal");
        teDestination = GetNode<RichTextLabel>("HbPreview/ScrollContainer/TeDestination");
        cbGeneratePreview = GetNode<CheckBox>("HbPreviewTop/CbGeneratePreview");
        cbShowFullPaths = GetNode<CheckBox>("HbPreviewTop/CbShowFullPaths");
        cbShowColorCodes = GetNode<CheckBox>("HbPreviewTop/CbShowColorCodes");
    }

    public void Show(List<FileJob> jobs)
    {
        teOriginal.BbcodeText = "";
        teDestination.BbcodeText = "";

        Font f = teOriginal.GetFont("normal_font");
        Vector2 longestString = new Vector2(0,0);

        foreach (FileJob job in jobs)
        {
            string color = "#aaaaaa";
            foreach (KeyValuePair<string, string> c in colorCodes)
            {
                if (job.pathOriginal.Contains(c.Key))
                {
                    color = c.Value;
                }
            }

            foreach (KeyValuePair<string, string> c in colorNames)
            {
                if (job.pathOriginal.Contains(c.Key))
                {
                    color = c.Value;
                }
            }
            
            Vector2 stringSize = f.GetStringSize(job.pathOriginal.GetFile());
            if (stringSize.x > longestString.x) {
                longestString = stringSize;
            }

            teOriginal.BbcodeText += GenerateBbcodeText(job.pathOriginal, cbShowColorCodes.Pressed, false, true);
            teDestination.BbcodeText += GenerateBbcodeText(job.pathDestination, cbShowColorCodes.Pressed, cbShowFullPaths.Pressed, false);
        }
        
        // Set Size of pathOriginal Box to fit longest filename
        teOriginal.RectMinSize = new Vector2(longestString.x, teOriginal.RectMinSize.y);
    }

    public string GenerateBbcodeText(string str, bool withColorCode, bool fullPath, bool generateLink)
    {

        string color = "ccc";
        if (withColorCode) {
            foreach (KeyValuePair<string, string> c in colorCodes)
            {
                if (str.Contains(c.Key))
                {
                    color = c.Value;
                }
            }

            foreach (KeyValuePair<string, string> c in colorNames)
            {
                if (str.Contains(c.Key))
                {
                    color = c.Value;
                }
            }
        }

        string pathPrev = fullPath ? str : str.GetFile();
        string bbc;
        if (generateLink) {
            bbc = $"[color=#{color}][url={str}]{pathPrev}[/url][/color]" + "\n";
        } else {
            bbc = $"[color=#{color}]{pathPrev}[/color]" + "\n";
        }
        return bbc;
    }

    public void OnFileClicked(string href)
    {
        GD.Print(href);
        System.Diagnostics.Process.Start(href);
    }

}
