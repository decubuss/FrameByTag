using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCollocation
{
    Object_Spatial_Subject, Object_Subject_Spatial
}

public class Spatial 
{
    public HierarchyRank SubjectRank;
    public ItemCollocation Collocation;
    private string _name;
    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value.MakeCapitalLetter();
        }

    }
    public string[] AltNames;
    public Spatial(string name, string[] altNames, ItemCollocation collocation, HierarchyRank subjectRank)
    {
        Name = name;
        AltNames = altNames;
        Collocation = collocation;
        SubjectRank = subjectRank;
    }
}
