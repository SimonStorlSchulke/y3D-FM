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
        /*new BatchProcess(
            "Testoperator",
            "A = 0.5, B = 2, c= false, d = true",
            (MagickImage img, string optionsString) => {
                img.Trim();
                img.RePage();
                img.Density = new Density(300, 300);
                img.Depth = 8;
                var opt = compileOptions(optionsString);
            }
        ),*/
    };

    public static Dictionary<string, object> compileOptions(string optionsString) {
        Dictionary<string, object> dict = new Dictionary<string, object>();

        optionsString = optionsString.Replace(" ", ""); //remove whitespace
        optionsString = optionsString.ToLower(); //make all lowercase
        string[] options = optionsString.Split(","); //separate individual options

        foreach (string optionString in options) {
            object value = 0;
            float f;
            string[] optionPair = optionString.Split("=");
            if (optionPair[1] == "true") value = true;
            else if (optionPair[1] == "false") value = false;
            else if(float.TryParse(optionPair[1], out f)) value = f;
            dict.Add(optionPair[0], value);
        }
        return dict;
    }
}
