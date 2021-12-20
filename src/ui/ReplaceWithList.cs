using Godot;
using System;
using System.Collections.Generic;

public class ReplaceWithList : Node
{
    public Dictionary<string, string> GetReplaceWithDict()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        foreach (ReplaceWithInput RW in GetChildren())
        {
            if(!dict.ContainsKey(RW.leReplace.Text)) {
                dict.Add(RW.leReplace.Text, RW.leWith.Text);
            }
        }
        return dict;
    }

    public void PopulateFromData(string[] from, string[] to) {
        ReplaceWithInput inp = GetChild(0).Duplicate() as ReplaceWithInput;
        foreach (Node c in GetChildren()) {
            c.QueueFree();
        }

        for (int i = 0; i < from.Length; i++) {
            ReplaceWithInput dp = inp.Duplicate() as ReplaceWithInput;
            dp.leReplace.Text = from[i];
            dp.leWith.Text = to[i];
            AddChild(dp);
        }

    }

}
