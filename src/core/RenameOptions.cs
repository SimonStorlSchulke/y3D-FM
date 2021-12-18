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

    List<string> fileList = new List<string>();
    List<FileJob> jobList;

    public List<FileJob> ParseFiles(bool refreshFileList)
    {
        jobList = new List<FileJob>();

        if(refreshFileList) {
            fileList = new List<string>();
            foreach (KeyValuePair<string, string> folder in productFoldersDict)
            {
                string[] files;
                files = RNUtil.TryParseFiles(folder.Key, true);
                fileList.AddRange(files);
            }
        }

        foreach (string fileOrigin in fileList)
        {
            bool ignore = false;
            bool remove = false;
            foreach (string ignoreFile in ignoreFilesList)
            {
                if (ignoreFile != "" && fileOrigin.GetFile().Contains(ignoreFile)) {
                    ignore = true;
                    break;
                }
            }

            foreach (string removeFile in removeFilesList)
            {
                if (removeFile != "" && fileOrigin.GetFile().Contains(removeFile)) {
                    remove = true;
                }
            }

            if (ignore) continue;

            string fileDest;

            if (remove) {
                jobList.Add(new FileJob(fileOrigin, "DELETE"));
                continue;
            }


            fileDest = fileOrigin.GetFile();
            foreach (KeyValuePair<string, string> kvp in replaceWithDict)
            {
                if (kvp.Key == "") {
                    continue;
                }
                fileDest = fileDest.Replace(kvp.Key, kvp.Value);
            }
            if (fileOrigin.GetFile() != fileDest) {
                jobList.Add(new FileJob(fileOrigin, fileOrigin.GetBaseDir() + fileDest));
            }
        }
        
        return jobList;
    }
}
