using Godot;
using System;


public class Tabs : TabContainer
{
    [Export]
    NodePath NPButtonRun;
    Button btnRun;

    public override void _Ready()
    {
        btnRun = GetNode<Button>(NPButtonRun);
    }

    void OnChangeTab(int tab) {
        if (tab == 4) {
            btnRun.Hide();
        } else {
            btnRun.Show();
        }
    }
}
