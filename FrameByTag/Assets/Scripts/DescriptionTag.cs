using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TagType { Spatial, Item, Referencer, Action}

public class DescriptionTag
{
    public int Index;
    public string Keyword;
    public TagType TagType;

    public DescriptionTag(int index, string keyword, TagType type)
    {
        Index = index;
        Keyword = keyword;
        TagType = type;
    }


}
