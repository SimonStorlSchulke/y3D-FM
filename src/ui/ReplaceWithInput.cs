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

    void OnLEChanged(string new_text) {

        if (GetIndex() == GetParent().GetChildCount()-1 ) {
            ReplaceWithInput dp = this.Duplicate() as ReplaceWithInput;
            GetParent().AddChild(dp);
            dp.leReplace.Text = "";
            dp.leWith.Text = "";
        }
    }

    void OnBtnDeletePressed() {
        if (GetParent().GetChildCount() > 1) {
            QueueFree();
        }
    }
}
