using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Frame
{
    public ShotParameters ShotParameters;
    public List<ShotElement> ShotElements;
    public List<DescriptionTag> DescriptionTags;
    public string Description;

    public Frame(ShotParameters shotParameters, List<ShotElement> shotElements, List<DescriptionTag> descriptionTags,string description)
    {
        ShotParameters = shotParameters;
        ShotElements = shotElements;
        DescriptionTags = descriptionTags;
        Description = description;
    }
}
