using Godot;
using System;
using System.Collections.Generic;

public class RenameOptions : Node {
    public Dictionary<string, string> productFoldersDict;
    public bool moveToBaseFolders;
    public List<string> ignoreFilesList;
    public List<string> removeFilesList;
    public Dictionary<string, string> replaceWithDict;
    public List<string> removeFileNamePartsList;

    // stores file and productname of FIle
    Dictionary<string, string> fileList = new Dictionary<string, string>();
    List<FileJob> jobList;

    public List<FileJob> ParseFiles(bool refreshFileList) {
        jobList = new List<FileJob>();

        if (refreshFileList) {
            fileList = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> folder in productFoldersDict) {
                string[] files;
                files = RNUtil.TryParseFiles(folder.Key, true);
                foreach (string file in files)
                {
                    fileList.Add(file, folder.Value);
                }
            }
        }

        foreach (KeyValuePair<string, string> fileOrigin in fileList) {
            string[] productnameParts = fileOrigin.Value.Split("..");
            if (productnameParts.Length == 1) {
                productnameParts = new string[2]{productnameParts[0],""};
            }

            bool ignore = false;
            bool remove = false;
            foreach (string ignoreFile in ignoreFilesList) {
                if (ignoreFile != "" && fileOrigin.Key.GetFile().Contains(ignoreFile)) {
                    ignore = true;
                    break;
                }
            }

            foreach (string removeFile in removeFilesList) {
                if (removeFile != "" && fileOrigin.Key.GetFile().Contains(removeFile)) {
                    remove = true;
                }
            }

            if (ignore) continue;

            string fileDest;

            if (remove) {
                jobList.Add(new FileJob(fileOrigin.Key, "DELETE"));
                continue;
            }


            fileDest = fileOrigin.Key.GetFile();
            foreach (KeyValuePair<string, string> kvp in replaceWithDict) {
                if (kvp.Key == "") {
                    continue;
                }

                string parsedString = kvp.Value;
                parsedString = parsedString.Replace("%DATE%", DateTime.Now.ToString("yyyyMMdd"));
                parsedString = parsedString.Replace("<p_name_start>", productnameParts[0]);
                parsedString = parsedString.Replace("<p_name_end>", productnameParts[1]);
                


                fileDest = fileDest.Replace(kvp.Key, parsedString);

            }

            foreach (string rmPart in removeFileNamePartsList) {
                if (rmPart == "") {
                    continue;
                }

                if (fileDest.Contains(rmPart)) {
                    fileDest = fileDest.Replace(rmPart, "");
                }
            }

            if (fileOrigin.Key.GetFile() != fileDest) {
                jobList.Add(new FileJob(fileOrigin.Key, fileOrigin.Key.GetBaseDir() + "\\" + fileDest));
            }
        }

        return jobList;
    }
}
