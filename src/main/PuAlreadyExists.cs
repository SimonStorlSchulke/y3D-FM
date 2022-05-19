using Godot;
using System;

public class PuAlreadyExists : ConfirmationDialog
{
    public static PuAlreadyExists instance;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        instance = this;
    }

    public static void ShowPopup() {
        instance.GetNode<TextEdit>("Vb/TeAlreadyExists").Text = "";
        foreach (FileJob fj in Main.instance.JobListExistingFiles) {
            instance.GetNode<TextEdit>("Vb/TeAlreadyExists").Text += fj.pathDestination.GetFile() + "\n";
        }
        instance.PopupCentered();
    }

    public void DoIt() {
        FileJob.Execute(Main.instance.JobListExistingFiles, overwrite: true);
    }


}
