using System.Linq;

/// <summary> C# Representation of the Data saved in .yfm files in the <see cref="Save"/> class. </summary>
public class SaveData
{
    public string[] baseFolders = {};
    public string[] productNames = {};
    public bool moveToBaseFolders;
    public string[] ignoreFilesList = {};
    public string[] removeFilesList = {};
    public string[] removeNamePartsList = {};
    public string[] replaceDictFrom = {};
    public string[] replaceDictTo = {};
    public string prefix;
    public string subfix;
    public string dropletPath;
    public string moveToPath;
    public bool overwrite;

    /// <summary> Creates a SaveData Object from the current <see cref="RenameOptions"/> </summar>
    public static SaveData FromOptions(RenameOptions rno) 
    {
        SaveData sd = new SaveData();
        sd.baseFolders = rno.productFoldersDict.Keys.ToArray();
        sd.productNames = rno.productFoldersDict.Values.ToArray();
        sd.moveToBaseFolders = rno.moveToBaseFolders;
        sd.overwrite = rno.overwrite;
        sd.ignoreFilesList = rno.ignoreFilesList.ToArray();
        sd.removeFilesList = rno.removeFilesList.ToArray();
        sd.removeNamePartsList = rno.removeFileNamePartsList.ToArray();
        sd.replaceDictFrom = rno.replaceWithDict.Keys.ToArray();
        sd.replaceDictTo = rno.replaceWithDict.Values.ToArray();
        sd.prefix = rno.prefix;
        sd.subfix = rno.subfix;
        sd.moveToPath = rno.moveToPath;
        return sd;
    }
}