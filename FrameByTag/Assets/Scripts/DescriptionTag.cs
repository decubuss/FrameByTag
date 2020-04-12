using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TagType { Spatial, Item, Referencer, Action}

public class DescriptionTag
{
    public int Index;
    public string Tag;
    public TagType Type;

    public DescriptionTag(int index, string tag, TagType type)
    {
        Index = index;
        Tag = tag;
        Type = type;
    }
}
