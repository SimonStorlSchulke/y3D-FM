using Godot;
using System.Collections.Generic;
using ImageMagick;
using System;


public delegate void BatchProcessFunction(MagickImage img, string optionsString, string destination, bool psd = true, bool jpg = true);

/// <summary> A process to be executed on a set of Images </summary>
public struct BatchProcess
{
    public string name;
    public string description;
    public string defaultOptions;
    public BatchProcessFunction function;

    public BatchProcess(string name, string description, string defaultOptions, BatchProcessFunction function)
    {
        this.name = name;
        this.description = description;
        this.defaultOptions = defaultOptions;
        this.function = function;
    }
}


public static class BatchPresets
{
    public static string getJPGPath(string path)
    {
        string jpgDest = System.IO.Directory.GetParent(System.IO.Directory.GetParent(path).FullName).FullName;
        jpgDest += "\\" + System.IO.Path.GetFileName(path.GetBaseDir()) + "_jpg\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + path.GetFile() + ".jpg";
        return jpgDest;
    }

    /// <summary> Array of all BatchPresets. Dynamically added to the UI in <see cref="DopletComp"/> </summary>
    public static BatchProcess[] list =
    {
        new BatchProcess(
            "ECS Trim",
            "Remove whitespace from the image, change DPI to 300 pixels/inch and use 8 bits per channel. Optionally add a margin factor (in pixels) to the trimmer.",
            "margin = 0",
            (MagickImage img, string optionsString, string destination, bool psd, bool jpg) => {
                int margin = Convert.ToInt32(compileOptions(optionsString)["margin"]);
                img.Trim();
                if (margin > 0) {
                    img.Extent(img.Width + margin*2, img.Height + margin*2, Gravity.Center);
                }
                img.RePage();
                img.Density = new Density(300, 300);
                img.Depth = 8;
                img.SetProfile(ColorProfile.SRGB);

                if (psd) {
                    img.Write(destination + ".psd");
                }
                if (jpg) {
                    string jpgDest = destination.Replace("output", "output_jpg");
                    var jpgFile = new MagickImage(img);
                    jpgFile.ColorAlpha(MagickColors.White);
                    jpgFile.Quality = 65;
                    System.IO.Directory.CreateDirectory(jpgDest.GetBaseDir());
                    jpgFile.Write(jpgDest + ".jpg");
                    GD.Print(jpgDest + ".jpg");
                }
            }
        ),
        new BatchProcess(
            "Title",
            "Einfach MLI Ordner oder auch Kamera 01 Ordner reinziehen, Title Batch preset ausführen, fertig.",
            "ShadowLayerPath = Shadow\\Shadow_0000.png, trim = false",
            (MagickImage img, string optionsString, string destination, bool psd, bool jpg) => {
                var opt = compileOptions(optionsString);


                string[] paths = {img.FileName.GetBaseDir().GetBaseDir(), (string)opt["ShadowLayerPath"]};

                if (!System.IO.File.Exists(System.IO.Path.Combine(paths))) {
                    // If shadow File is not found, try if it was rendered as a "(Singleframe)"
                    paths[1] = ((string)opt["ShadowLayerPath"]).Replace("_0000", "_(Singleframe)_0000");
                }

                string filenameAo = System.IO.Path.Combine(paths);


                using (MagickImage imgAO = new MagickImage(filenameAo))
                {
                    imgAO.SetProfile(ColorProfile.SRGB);
                    img.SetProfile(ColorProfile.SRGB);
                    MagickImageCollection psdLayers = new MagickImageCollection();
                    img.Density = imgAO.Density = new Density(300, 300);
                    img.Depth = imgAO.Depth = 8;

                    MagickImage imgbase = new MagickImage(imgAO);
                    imgAO.HasAlpha = true;
                    imgAO.Label = "Shadow";
                    img.Label = "Color";
                    psdLayers.Add(imgbase);
                    psdLayers.Add(imgAO);
                    psdLayers.Add(img);


                    string fileName = img.FileName.GetBaseDir().GetBaseDir().GetBaseDir().GetFile(); //disgusting
                    string dest = destination.GetBaseDir() + "\\" + fileName  + ".psd";
                    string dateFolder = DateTime.Now.ToString("yyyyMMdd") + "\\";
                    string destJPG = dest.GetBaseDir().GetBaseDir().GetBaseDir().GetBaseDir() + "\\output_jpg\\" + dateFolder + fileName  + ".jpg"; //voodoo!

                    if (psd) {
                        psdLayers.Write(destination.GetBaseDir() + "\\" + fileName  + ".psd");
                        DopletComp.lblProcessed.BbcodeText += "\n[color=yellow]Achtung: [/color] PSD Outputs müssen nochmal in Photoshop geöffnet werden, um die Opazität des Schattens manuell einzustellen. ";
                    }
                    if (jpg) {
                        DopletComp.lblProcessed.BbcodeText += "\n[color=yellow]Achtung: [/color] Der Title-Batchprozess unterstützt keinen JPG output.";
                    }
                }
            }
        ),
        new BatchProcess(
            "GF Rendercomp",
            "Combine Background and Application | WIP",
            "",
            (MagickImage imgBG, string optionsString, string destination, bool psd, bool jpg) => {
                //var opt = compileOptions(optionsString);
                if (imgBG.FileName.GetFile().Contains("_App")) {
                    throw(new Exception("Skipping App Image (applied in Filter)"));
                }

                string[] paths = {imgBG.FileName.GetBaseDir(), imgBG.FileName.GetFile().Replace("BG", "App")};
                string filenameApp = System.IO.Path.Combine(paths);

                using (MagickImage imgApp = new MagickImage(filenameApp))
                {
                    imgApp.SetProfile(ColorProfile.SRGB);
                    imgBG.SetProfile(ColorProfile.SRGB);
                    MagickImageCollection psdLayers = new MagickImageCollection();
                    MagickImage imgBase = new MagickImage(imgBG);

                    imgBG.Density = imgApp.Density = new Density(300, 300);
                    imgBG.Depth = imgApp.Depth = 16;

                    //imgApp.Compose = imgBG.Compose = CompositeOperator.Multiply; //Alpha, No, Replace, Blend, Over, Copy
                    //imgBG.Compose

                    var mApp = new MagickImage(imgBG);
                    mApp.Composite(imgApp, CompositeOperator.CopyAlpha);

                    imgBG.Label = "Background";
                    imgApp.Label = "Application";
                    psdLayers.Add(imgBase);
                    psdLayers.Add(imgBG);
                    //psdLayers.Add(mApp);
                    psdLayers.RePage();

                    if (psd) {
                        //var s = psdLayers.Flatten();
                        psdLayers.Write(destination + ".psd");
                    }
                    if (jpg) {
                        psdLayers[0].Alpha(AlphaOption.Transparent);
                        var jpgFile = psdLayers.Flatten();
                        jpgFile.ColorAlpha(MagickColors.White);
                        jpgFile.Write(getJPGPath(destination));
                    }
                }
            }
        )
    };

    /// <summary> Compiles an optionsString like "AO = 0.8, trim = false" into a C# Dictionary </summary>
    public static Dictionary<string, object> compileOptions(string optionsString)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();

        optionsString = optionsString.Replace(" ", ""); //remove whitespace
        string[] options = optionsString.Split(","); //separate individual options

        foreach (string optionString in options)
        {
            object value = 0;
            float f;
            string[] optionPair = optionString.Split("=");

            // Assigns the options value depending on wether the input string represents a..
            if (optionPair[1].ToLower() == "true") value = true; // ..boolean true
            else if (optionPair[1].ToLower() == "false") value = false; // ..boolean false
            else if (float.TryParse(optionPair[1], out f)) value = f; // ..number
            else value = optionPair[1]; //.. or string
            dict.Add(optionPair[0], value);
        }
        return dict;
    }
}
