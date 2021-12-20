using Godot;
using System.Collections.Generic;
using System.Linq;

public class SaveData
{
    public string[] baseFolders;
    public string[] productNames;
    public bool moveToBaseFolders;
    public string[] ignoreFilesList;
    public string[] removeFilesList;
    public string[] removeNamePartsList;
    public string[] replaceDictFrom;
    public string[] replaceDictTo;
    public string dropletPath;
    public string mpsDir;
    public static SaveData FromOptions(RenameOptions rno) {
        SaveData sd = new SaveData();
        sd.baseFolders = rno.productFoldersDict.Keys.ToArray();
        sd.productNames = rno.productFoldersDict.Values.ToArray();
        sd.moveToBaseFolders = rno.moveToBaseFolders;
        sd.ignoreFilesList = rno.ignoreFilesList.ToArray();
        sd.removeFilesList = rno.removeFilesList.ToArray();
        sd.removeFilesList = rno.removeFileNamePartsList.ToArray();
        sd.replaceDictFrom = rno.replaceWithDict.Keys.ToArray();
        sd.replaceDictTo = rno.replaceWithDict.Values.ToArray();
        return sd;
    }
}