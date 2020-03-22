using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.RegularExpressions;
using System.Linq;

public class ObjectsPlacementHandler
{
    public ObjectsPlacementHandler()
    {
        FrameDescription.OnDescriptionChangedEvent += BreakOnTags;
    }

    private void BreakOnTags(string rawinput)
    {
        //string pattern = @"[\w -]+"; //this one is just for words
        var words = Regex.Split(rawinput, @"[<>.!@#%/ ,]+");
        

        foreach(var word in words)
        {
            Debug.Log(word);
        }
    }
}
