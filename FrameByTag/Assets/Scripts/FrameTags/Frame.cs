using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Frame
{
    public ShotParameters ShotParameters;
    public Dictionary<DescriptionTag, ShotElement> TagItemDict;
    public string Description;

    public Frame(ShotParameters shotParameters, Dictionary<DescriptionTag, ShotElement> tagItemDict, string description)
    {
        ShotParameters = shotParameters;
        TagItemDict = tagItemDict;
        Description = description;
    }
}
