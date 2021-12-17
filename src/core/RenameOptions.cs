using Godot;
using System;
using System.Collections.Generic;

public class RenameOptions : Node
{
    public Dictionary<string, string> productFoldersDict;
    public bool moveToBaseFolders;
    public List<string> ignoreFilesList;
    public List<string> removeFilesList;
    public Dictionary<string, string> replaceWithDict;
    public List<string> removeFileNamePartsList;

    public List<FileJob> ParseFiles() 
    {
        List<string> fileList = new List<string>();
        List<FileJob> jobList = new List<FileJob>();

        foreach (KeyValuePair<string, string> folder in productFoldersDict)
        {
            string[] files;
            files = RNUtil.TryParseFiles(folder.Key, true);
            fileList.AddRange(files);
        }

        GD.Print(ignoreFilesList.Count);
        foreach (string fileOrigin in fileList)
        {
            bool ignore = false;
            foreach (string ignoreFile in ignoreFilesList)
            {
                if (ignoreFile != "" && fileOrigin.GetFile().Contains(ignoreFile)) {
                    ignore = true;
                    break;
                }
            }

            if (ignore) continue;


            string fileDest = fileOrigin;
            foreach (KeyValuePair<string, string> kvp in replaceWithDict)
            {
                if (kvp.Key == "") {
                    continue;
                }
                fileDest = fileDest.Replace(kvp.Key, kvp.Value);
            }
            if (fileOrigin != fileDest) {
                jobList.Add(new FileJob(fileOrigin, fileDest));
            }
        }
        
        return jobList;
    }
}
