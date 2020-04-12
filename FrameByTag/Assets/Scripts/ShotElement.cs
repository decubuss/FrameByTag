using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HierarchyRank
{
    InFocus, Addition, Background, Default
}


public class ShotElement 
{
    public int Layer;
    public string PropName;
    public string State;
    public HierarchyRank Rank;

    public ShotElement(string name, int layer = 0, HierarchyRank rank = HierarchyRank.Default, string state = null)
    {
        Layer = layer;
        PropName = name;
        State = state;
        Rank = rank;
    }
}
