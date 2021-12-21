using Godot;
using System;

public class ErrorLog : RichTextLabel
{
    public static ErrorLog instance;

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

    public void Add(string text, LogColor color) {
        string bbc;
        switch (color)
        {
            case LogColor.BLUE:
                bbc = $"[color=#3e7beb]{text}[/color]";
                break;
            case LogColor.GREEN:
                bbc = $"[color=#3eeb6f]{text}[/color]";
                break;
            case LogColor.YELLOW:
                bbc = $"[color=#ebeb3e]{text}[/color]";
                break;
            case LogColor.ORANGE:
                bbc = $"[color=#eb953e]{text}[/color]";
                break;
            case LogColor.RED:
                bbc = $"[color=#eb3e3e]{text}[/color]";
                break;
            case LogColor.WHITE:
                bbc = $"[color=#d7d7d7]{text}[/color]";
                break;
            default:
                bbc = $"[color=#d7d7d7]{text}[/color]";
                break;
        }
        string last_chars = this.BbcodeText.Substring(Math.Max(0, this.BbcodeText.Length - 2));
        if (last_chars == "\n\n") {
            this.AppendBbcode(bbc);
        } else {
            this.AppendBbcode("\n\n" + bbc);
        }
    }
}
