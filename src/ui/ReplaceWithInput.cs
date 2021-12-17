using Godot;
using System;

public class ReplaceWithInput : Node
{
    public LineEdit leReplace;
    public LineEdit leWith;

    public override void _Ready()
    {
        leReplace = GetNode<LineEdit>("LEReplace");
        leWith = GetNode<LineEdit>("LEWith");
    }

    void OnBtnDeletePressed() {
        
    }
}
