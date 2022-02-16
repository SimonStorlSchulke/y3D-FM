using Godot;
using System.Collections.Generic;
using ImageMagick;
using System;

public delegate void BatchProcessFunction(MagickImage img, string optionsString, bool psd = true, bool jpg = true);

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
    public static BatchProcess[] list =
    {
        new BatchProcess(
            "ECS Trim",
            "Remove whitespace from the image, change DPI to 300 pixels/inch and use 8 bits per channel. Optionally add a margin factor (in pixels) to the trimmer.",
            "margin = 0",
            (MagickImage img, string optionsString, bool psd, bool jpg) => {
                string destPath = DopletComp.GetSaveDestination(img.FileName, jpg: jpg, sort: true); // needs to be calles first
                int margin = Convert.ToInt32(compileOptions(optionsString)["margin"]);
                img.Trim();
                if (margin > 0) {
                    img.Extent(img.Width + margin*2, img.Height + margin*2, Gravity.Center);
                }
                img.RePage();
                img.Density = new Density(300, 300);
                img.Depth = 8;

                if (psd) {
                    img.Write(destPath + ".psd");
                }
                if (jpg) {
                    var jpgFile = new MagickImage(img);
                    jpgFile.ColorAlpha(MagickColors.White);
                    jpgFile.Write(destPath + ".jpg");
                }
            }
        ),
        new BatchProcess(
            "Title",
            "To make sure the layers are found, the AO Layers need to be in the same folder and have the same name as the Colorlayers but with an 'AO_' Prefix. The 'Prepare Title' Rename Preset should do that.",
            "AO Layer Name Prefix = AO_, AO = 0.5, trim = false",
            (MagickImage img, string optionsString, bool psd, bool jpg) => {
                string destPath = DopletComp.GetSaveDestination(img.FileName, jpg: jpg, sort: true); // needs to be calles first
                var opt = compileOptions(optionsString);
                if (img.FileName.GetFile().Contains((string)opt["AOLayerNamePrefix"])) {
                    throw(new Exception("Skipping AO Image"));
                }

                string[] paths = {img.FileName.GetBaseDir(), opt["AOLayerNamePrefix"] + img.FileName.GetFile()};
                string filenameAo = System.IO.Path.Combine(paths);

                using (MagickImage imgAO = new MagickImage(filenameAo))
                {
                    MagickImageCollection psdLayers = new MagickImageCollection();
                    img.Density = imgAO.Density = new Density(300, 300);
                    img.Depth = imgAO.Depth = 8;

                    MagickImage imgbase = new MagickImage(img);
                    MagickImage imgShadow = new MagickImage(imgAO);
                    imgShadow.BlackThreshold(new Percentage(100));
                    imgAO.HasAlpha = false;
                    imgAO.Negate();
                    
                    imgShadow.Composite(imgAO, CompositeOperator.CopyAlpha);
                    imgAO.Negate();
                    imgAO.HasAlpha = true;
                    imgAO.Composite(img, CompositeOperator.CopyAlpha);
                    
                    imgAO.Compose = CompositeOperator.Multiply;
                    imgShadow.Label = "Shadow";
                    img.Label = "Color";
                    imgAO.Label = "AO";
                    psdLayers.Add(imgbase);
                    psdLayers.Add(imgShadow);
                    psdLayers.Add(img);
                    psdLayers.Add(imgAO);
                    psdLayers.TrimBounds();
                    psdLayers.RePage();
                    if (psd) {
                        psdLayers.Write(destPath + ".psd");
                    }
                    if (jpg) {
                        var jpgFile = psdLayers.Flatten();
                        jpgFile.ColorAlpha(MagickColors.White);
                        jpgFile.Write(destPath + ".jpg");
                    }
                }
            }
        ),
    };

    public static Dictionary<string, object> compileOptions(string optionsString) {
        Dictionary<string, object> dict = new Dictionary<string, object>();

        optionsString = optionsString.Replace(" ", ""); //remove whitespace
        string[] options = optionsString.Split(","); //separate individual options

        foreach (string optionString in options) {
            object value = 0;
            float f;
            string[] optionPair = optionString.Split("=");
            if (optionPair[1] == "true") value = true;
            else if (optionPair[1] == "false") value = false;
            else if(float.TryParse(optionPair[1], out f)) value = f;
            else value = optionPair[1];
            dict.Add(optionPair[0], value);
        }
        return dict;
    }
}
