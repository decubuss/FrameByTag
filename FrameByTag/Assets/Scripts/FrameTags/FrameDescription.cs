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
    private Dictionary<string[], string> ObjectsCalls;

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
        CameraSetter = FindObjectOfType<CameraSetter>();
        PlacementController = FindObjectOfType<ObjectsPlacementController>();

        StartVariationsInit();
    }

    public void OnInputChange()
    {
        if (DescriptionSource.text != "" && DescriptionSource.text != this.RawFrameInput)
        {
            RawFrameInput = DescriptionSource.text;
            var CutInput = InputProcessing();

            if (OnDescriptionChange != null)
                OnDescriptionChange(CutInput);
        }
    }
    
    private string InputProcessing()
    {
        var ResultSequence = new List<string>();
        string[] words = RawFrameInput.ToLower().Split(' ');
        var CutInput = RawFrameInput;

        foreach (var AlternativeCalls in CameraCalls)
        {
            foreach (string AltName in AlternativeCalls.Key)
            {
                if (words.Any(x => x == AltName))
                {
                    words[Array.IndexOf(words, AltName)] = AlternativeCalls.Value;
                    CutInput.Replace(AltName, "");
                }
            }
        }

        //medium shot 2 males OR male standing and man sitting medium shot



        return CutInput;
    }


    private void StartVariationsInit()
    {
        if (CameraSetter == null) { return; }
        if (AOController == null) { return; }

        CameraCalls = CameraSetter.GetAlternateNames();

    }

    
}
