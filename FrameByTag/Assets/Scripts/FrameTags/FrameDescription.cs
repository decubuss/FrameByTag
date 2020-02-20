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
    
    private InputField DescriptionSource;
    private CameraSetter CameraSetter;
    private ObjectsPlacementController PlacementController;

    public AvailableObjectsController AOController;

    private Dictionary<string[], string> CameraCalls;

    private List<string> TagSequence;///TODO: basically here should be a sequence of all tags in case it influencive

    public delegate void OnDescriptionChangeDelegate(string Input);
    public static event OnDescriptionChangeDelegate OnDescriptionChange;

    void Start()
    {
        TagSequence = new List<string>();
        RawFrameInput = "";

        AOController = ScriptableObject.CreateInstance<AvailableObjectsController>();
        AOController.Init();

        if (DescriptionSource == null)
        {
            DescriptionSource = FindObjectOfType<InputField>();
        }
        
    }

    public void OnInputChange()
    {
        if (DescriptionSource.text != "" && DescriptionSource.text != this.RawFrameInput)
        {
            RawFrameInput = DescriptionSource.text;
            //var CutInput = InputProcessing();

            if (OnDescriptionChange != null)
                OnDescriptionChange(RawFrameInput);
        }
    }
    
}
