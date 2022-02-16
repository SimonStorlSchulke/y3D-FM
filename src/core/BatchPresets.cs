using Godot;
using System.Collections.Generic;
using ImageMagick;

public delegate void BatchProcessFunction(MagickImage img, string optionsString);

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
            "Remove whitespace from the image, change DPI to 300 pixels/inch and use 8 bits per channel",
            "trim = true",
            (MagickImage img, string optionsString) => {
                bool trim = (bool)compileOptions(optionsString)["trim"];
                if (trim) img.Trim();
                img.RePage();
                img.Density = new Density(300, 300);
                img.Depth = 8;
            }
        ),
        new BatchProcess(
            "Title",
            "Comb images",
            "AO Layer Name Prefix = AO_, A= = 0.5, trim = false",
            (MagickImage img, string optionsString) => {
                var opt = compileOptions(optionsString);

                string[] paths = {img.FileName.GetBaseDir(), opt["AOLayerNamePrefix"] + img.FileName.GetFile()};
                string filenameAo = System.IO.Path.Combine(paths);

                GD.Print(filenameAo);
                using (MagickImage AO = new MagickImage(filenameAo))
                {
                    
                    //img.Composite(AO, CompositeOperator.Multiply);
                    MagickImageCollection c = new MagickImageCollection();
                    MagickImage m = new MagickImage(img);
                    c.Add(m);
                    c.Add(img);
                    c.Add(AO);
                    c.Write(@"C:\Users\simon\Pictures\bg\test.psd");
                }
                //img.Trim();   
                //img.RePage();
                //img.Density = new Density(300, 300);
                //img.Depth = 8;
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
