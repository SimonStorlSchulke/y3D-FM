using Godot;
using System;

public class ErrorLog : RichTextLabel
{
    public static ErrorLog instance;

    [Export]
    NodePath NPTabs;

    public enum LogColor {
        BLUE,
        GREEN,
        YELLOW,
        ORANGE,
        RED,
        WHITE
    }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (instance == null) {
            instance = this;
        } else {
            GD.Print("Only one Instance of ErrorLog is allowed");
        }
    }

    public void PopUp() {
        GetNode<TabContainer>(NPTabs).CurrentTab = 1;
    }

    public void Add(string title, string text, LogColor colorTitle) {
        string bbc;
        switch (colorTitle)
        {
            case LogColor.BLUE:
                bbc = $"[b][color=#3e7beb]{title}[/color][/b]";
                break;
            case LogColor.GREEN:
                bbc = $"[b][color=#3eeb6f]{title}[/color][/b]";
                break;
            case LogColor.YELLOW:
                bbc = $"[b][color=#ebeb3e]{title}[/color][/b]";
                break;
            case LogColor.ORANGE:
                bbc = $"[b][color=#eb953e]{title}[/color][/b]";
                break;
            case LogColor.RED:
                bbc = $"[b][color=#eb3e3e]{title}[/color][/b]";
                break;
            case LogColor.WHITE:
                bbc = $"[b][color=#d7d7d7]{title}[/color][/b]";
                break;
            default:
                bbc = $"[b][color=#d7d7d7]{title}[/color][/b]";
                break;
        }

        bbc += "\n"+text;

        string last_chars = this.BbcodeText.Substring(Math.Max(0, this.BbcodeText.Length - 2));
        if (this.Text == "" || last_chars == "\n\n") {
            this.AppendBbcode(bbc);
        } else {
            this.AppendBbcode("\n\n" + bbc);
        }
    }
}
