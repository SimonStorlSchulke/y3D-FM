using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class RenameOptions : Node
{
    // Dict with Top-Folder (key) and Productname (value)
    public Dictionary<string, string> productFoldersDict;
    public string date;
    public bool moveToBaseFolders;
    public string moveToPath;
    public List<string> ignoreFilesList;
    public List<string> removeFilesList;
    public Dictionary<string, string> replaceWithDict;
    public List<string> removeFileNamePartsList;
    public string prefix;
    public string subfix;
    public bool overwrite;

    // Files List: 1. Filepath 2. File Top-Basefolder 3. Productname
    List<Tuple<string, string, string>> list = new List<Tuple<string, string, string>>();
    List<FileJob> jobList;

    string ParseKeywords(string input, string[] productnameParts)
    {
        string parsedString = input;
        string useDate = date == "" ? DateTime.Now.ToString("yyyyMMdd") : date;
        parsedString = parsedString.Replace("<date>", useDate);
        parsedString = parsedString.Replace("<p_name_start>", productnameParts[0]);
        parsedString = parsedString.Replace("<p_name_end>", productnameParts[1]);
        return parsedString;
    }

    public bool ignoreFile(string unparsedFileName, string originalFileName)
    {
        foreach (string ignoreFile in ignoreFilesList)
        {
            string toCompare = ignoreFile.Contains("<date>") ? unparsedFileName : originalFileName;

            if (ignoreFile != "" && toCompare.Contains(ignoreFile))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary> rempoves all parts of a string where there are 8 letters in a row 
    /// (like 20220821) and replaces them with &lt; date &gt; </summary>
    public string UnParseDate(string s)
    {
        Regex rgx = new Regex(@"\d{8}");
        Match mat = rgx.Match(s);
        return (mat.ToString() == "") ? s : s.Replace(mat.ToString(), "<date>");
    }

    ///<summary> parses the filelist with the current RenameOptions and returns a jobList (list of <see cref="FileJob"/>) </summary>
    public List<FileJob> ParseFiles(bool refreshFileList)
    {
        jobList = new List<FileJob>();

        if (refreshFileList)
        {
            list = new List<Tuple<string, string, string>>();
            foreach (KeyValuePair<string, string> folder in productFoldersDict)
            {
                string[] files;
                files = RNUtil.TryParseFiles(folder.Key, true);
                foreach (string file in files)
                {
                    list.Add(new Tuple<string, string, string>(file, folder.Key, folder.Value));
                }
            }
        }

        foreach (Tuple<string, string, string> fileOrigin in list)
        {

            string originalFileName = fileOrigin.Item1.GetFile();
            string unparsedFileName = UnParseDate(fileOrigin.Item1.GetFile());


            string[] productnameParts = fileOrigin.Item3.Split("..");
            if (productnameParts.Length == 1)
            {
                productnameParts = new string[2] { productnameParts[0], "" };
            }

            bool ignore = false;
            bool remove = false;
            foreach (string ignoreFile in ignoreFilesList)
            {
                // TODO replace with ignoreFIle method
                string toCompare = ignoreFile.Contains("<date>") ? unparsedFileName : originalFileName;
                if (ignoreFile != "" && (toCompare.Contains(ignoreFile) || toCompare.Contains("Thumbs.db")))
                {
                    ignore = true;
                    break;
                }
            }

            foreach (string removeFile in removeFilesList)
            {
                string toCompare = removeFile.Contains("<date>") ? unparsedFileName : originalFileName;
                if (removeFile != "" && toCompare.Contains(removeFile))
                {
                    remove = true;
                }
            }

            if (ignore) continue;

            string fileDest;

            if (remove)
            {
                jobList.Add(new FileJob(fileOrigin.Item1, "DELETE"));
                continue;
            }

            fileOrigin.Item1.GetFile().Split("."); //Name and Extension

            fileDest = System.IO.Path.GetFileNameWithoutExtension(fileOrigin.Item1).GetFile();
            foreach (KeyValuePair<string, string> kvp in replaceWithDict)
            {
                if (kvp.Key == "")
                {
                    continue;
                }

                string toCompare = kvp.Key.Contains("<date>") ? UnParseDate(fileDest) : fileDest;

                fileDest = toCompare.Replace(kvp.Key, ParseKeywords(kvp.Value, productnameParts));

            }

            foreach (string toRemove in removeFileNamePartsList)
            {
                if (toRemove == "")
                {
                    continue;
                }

                if (fileDest.Contains(toRemove))
                {
                    fileDest = fileDest.Replace(toRemove, "");
                }
            }


            if (prefix != "")
            {
                string parsedPrefix = ParseKeywords(prefix, productnameParts);
                fileDest = parsedPrefix + fileDest;
            }


            if (subfix != "")
            {
                string parsedSubfix = ParseKeywords(subfix, productnameParts);
                fileDest = fileDest + parsedSubfix;
            }

            bool toBaseFolder = System.IO.Directory.Exists(fileOrigin.Item2) || moveToPath != "" ? moveToBaseFolders : false;

            string moveDir = moveToPath == "" ? fileOrigin.Item2 : moveToPath;

            fileDest = toBaseFolder ? moveDir + "\\" + fileDest : fileOrigin.Item1.GetBaseDir() + "\\" + fileDest;
            fileDest += System.IO.Path.GetExtension(fileOrigin.Item1);

            if (fileOrigin.Item1 != fileDest)
            {
                jobList.Add(new FileJob(fileOrigin.Item1, fileDest));
                var evens = from num in Enumerable.Range(0, 20) where num % 2 == 0 select num;
                var evan = from num in Enumerable.Range(0, 20) where num % 2 == 0 select num;

            }
        }

        return jobList;
    }
}
