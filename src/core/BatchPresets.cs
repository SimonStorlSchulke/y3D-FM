using Godot;
using System.Collections.Generic;
using ImageMagick;

public delegate void BatchProcessFunction(MagickImage img, string options = "");

public struct BatchProcess
{
    public string name;
    public string options;
    public BatchProcessFunction function;

    public BatchProcess(string name, string options, BatchProcessFunction function)
    {
        this.name = name;
        this.options = options;
        this.function = function;
    }
}

public static class BatchPresets
{
    public static BatchProcess[] list =
        {
            new BatchProcess(
                "ECS Trim",
                "Testoption = 2.0",
                (MagickImage img, string options) => {
                    img.Trim();
                    img.RePage();
                    img.Density = new Density(300, 300);
                    img.Depth = 8;
                }
            ),
            new BatchProcess(
                "Testoperator",
                "Testopt = 0.5, B = 2",
                (MagickImage img, string options) => {
                    img.Trim();
                    img.RePage();
                    img.Density = new Density(300, 300);
                    img.Depth = 8;
                }
            ),
        };
}
