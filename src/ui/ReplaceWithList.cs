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

}
