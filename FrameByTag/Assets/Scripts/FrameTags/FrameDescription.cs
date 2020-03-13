using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FrameDescription : MonoBehaviour
{
    // Start is called before the first frame update
    private string RawFrameInput;
    
    public InputField DescriptionSource;

    [HideInInspector]
    public ObjectsPlacementController PlacementController;

    public delegate void OnDescriptionChangeDelegate(string Input);
    public static event OnDescriptionChangeDelegate OnDescriptionChange;

    void Start()
    {
        PlacementController = FindObjectOfType<ObjectsPlacementController>();

        if (DescriptionSource == null)
        {
            DescriptionSource = GameObject.Find("DescriptionField").GetComponent<InputField>();
        }

        //TODO:subscribe on both of them to collect actual tags to store them if needed
        //OR 
        //make an entire class up for serialization needs
    }

    public void OnInputEnter()
    {
        //TODO: MINOR - make cursor stand there after enter
        if (DescriptionSource.text != "" && DescriptionSource.text != this.RawFrameInput)
        {
            
            RawFrameInput = DescriptionSource.text;

            if (OnDescriptionChange != null)
                OnDescriptionChange(RawFrameInput);
            
        }
    }
    
}
