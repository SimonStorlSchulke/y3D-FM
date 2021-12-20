using Godot;
using System;
using System.IO;

public class Save : AcceptDialog
{
    [Export]
    NodePath NPRenameOptions;
    [Export]
    NodePath NPBtnSave;

    [Export]
    NodePath NPBtnLoad;
    RenameOptions rno;

    public override void _Ready()
    {
        rno = GetNode<RenameOptions>(NPRenameOptions);
        GetNode<Button>(NPBtnSave).Connect("pressed", this, nameof(PU));
        Connect("confirmed", this, nameof(OnSaveConfigPressed));
    }

    public void PU() {
        PopupCentered();
    }

    public void OnSaveConfigPressed() {
        DirectoryInfo di = new DirectoryInfo(OS.GetExecutablePath().GetBaseDir());
        di.CreateSubdirectory("y3d_fm_configs");
        string path = OS.GetExecutablePath().GetBaseDir() + "\\y3d_fm_configs\\" + GetNode<LineEdit>("SaveDialog/LineEdit").Text + ".yfm";
        SaveToFile(path);
    }

    public void SaveToFile(string path) {
        SaveData sd = SaveData.FromOptions(GetNode<RenameOptions>(NPRenameOptions));

        System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(sd.GetType());
        
        Utf8StringWriter st = new Utf8StringWriter();
        x.Serialize(st, sd);
        using (StreamWriter writer = new StreamWriter(path))  
        {
            writer.Write(st);
        }
        GetNode<BtnLoad>(NPBtnLoad).UpdateItems();
    }
}

public class Utf8StringWriter : StringWriter
{
    public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;
}