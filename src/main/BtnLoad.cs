using Godot;
using System;
using System.IO;

public class BtnLoad : MenuButton
{
    string dirCfg;
    string[] saveFiles;
    public override void _Ready() {
        UpdateItems();
    }

    public void UpdateItems() {
        GetPopup().Items = new Godot.Collections.Array();
        DirectoryInfo di = new DirectoryInfo(OS.GetExecutablePath().GetBaseDir());
        di.CreateSubdirectory("y3d_fm_configs");
        dirCfg = OS.GetExecutablePath().GetBaseDir() + "\\y3d_fm_configs\\";
        saveFiles = System.IO.Directory.GetFiles(dirCfg);

        foreach (string f in saveFiles) {
            GetPopup().AddItem(f.GetFile());
        }
        GetPopup().Connect("id_pressed", this, nameof(OnLoadSelected));
    }

    public void OnLoadSelected(int index) {

        System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(SaveData));

        System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
        string text = System.IO.File.ReadAllText(saveFiles[index]);
        doc.LoadXml(text);
        using (Stream reader = new FileStream(saveFiles[index], FileMode.Open))
        {
            SaveData sd = (SaveData)x.Deserialize(reader);
            GD.Print(sd.replaceDictFrom);
        }
    }
}
