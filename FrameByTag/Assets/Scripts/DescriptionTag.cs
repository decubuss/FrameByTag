using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TagType { Spatial, Item, Action, Sign}

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
    public override string ToString()
    {
        return string.Format("{0} {1} {2}", this.Index, this.Keyword, this.TagType);
    }
}


