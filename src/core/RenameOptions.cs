using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class RenameOptions : Node {

    // Top-Folder and Productname
    public Dictionary<string, string> productFoldersDict;
    public bool moveToBaseFolders;
    public List<string> ignoreFilesList;
    public List<string> removeFilesList;
    public Dictionary<string, string> replaceWithDict;
    public List<string> removeFileNamePartsList;

    // stores file and productname of File
    //Dictionary<string, string> fileList = new Dictionary<string, string>();

    // Files List: 1. Filepath 2. File Top-Basefolder 3. Productname
    List<Tuple<string, string, string>> list = new List<Tuple<string, string, string>>();
    List<FileJob> jobList;

    public List<FileJob> ParseFiles(bool refreshFileList) {
        jobList = new List<FileJob>();

        if (refreshFileList) {
            //fileList = new Dictionary<string, string>();
            list = new List<Tuple<string, string, string>>();
            foreach (KeyValuePair<string, string> folder in productFoldersDict) {
                string[] files;
                files = RNUtil.TryParseFiles(folder.Key, true);
                foreach (string file in files)
                {
                    //fileList.Add(file, folder.Value);
                    list.Add(new Tuple<string, string, string>(file, folder.Key, folder.Value));
                }
            }
        }

        foreach (Tuple<string, string, string> fileOrigin in list) {
            string[] productnameParts = fileOrigin.Item3.Split("..");
            if (productnameParts.Length == 1) {
                productnameParts = new string[2]{productnameParts[0],""};
            }

            bool ignore = false;
            bool remove = false;
            foreach (string ignoreFile in ignoreFilesList) {
                if (ignoreFile != "" && fileOrigin.Item1.GetFile().Contains(ignoreFile)) {
                    ignore = true;
                    break;
                }
            }

            foreach (string removeFile in removeFilesList) {
                if (removeFile != "" && fileOrigin.Item1.GetFile().Contains(removeFile)) {
                    remove = true;
                }
            }

            if (ignore) continue;

            string fileDest;

            if (remove) {
                jobList.Add(new FileJob(fileOrigin.Item1, "DELETE"));
                continue;
            }


            fileDest = fileOrigin.Item1.GetFile();
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

            
            fileDest = moveToBaseFolders ? fileOrigin.Item2 + "\\" + fileDest : fileOrigin.Item1.GetBaseDir() + "\\" + fileDest;

            if (fileOrigin.Item1 != fileDest) {
                jobList.Add(new FileJob(fileOrigin.Item1, fileDest));
            }
        }

        return jobList;
    }

    
}
