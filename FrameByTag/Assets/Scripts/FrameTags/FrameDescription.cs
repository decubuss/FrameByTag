using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FrameDescription : MonoBehaviour
{
    // Start is called before the first frame update
    private string PreviousFrameInput;
    private string RawFrameInput;

    private InputField DescriptionSource;
    private CameraSetter CameraSetter;
    private ObjectsPlacementController PlacementController;

    public AvailableObjectsController AOController;

    private Dictionary<string[], string> CameraCalls;
    private Dictionary<string[], string> ObjectsCalls;

    private List<string> ObjectTags;///TODO: basically here should be a sequence of all tags in case it influencive

    void Start()
    {
        ObjectTags = new List<string>();

        AOController = ScriptableObject.CreateInstance<AvailableObjectsController>();
        AOController.Init();

        DescriptionSource = FindObjectOfType<InputField>();
        CameraSetter = FindObjectOfType<CameraSetter>();
        PlacementController = FindObjectOfType<ObjectsPlacementController>();

        StartVariationsInit();

        //var Type = CameraRoller.GetType();
    }

    void Update()
    {
        
        UpdateInput();
    }

    private void UpdateInput()
    {
        if (DescriptionSource.text != "")
        {
            RawFrameInput = DescriptionSource.text;

            if (this.RawFrameInput != this.PreviousFrameInput)
            {
                PreviousFrameInput = RawFrameInput;
                //and then run a function that defines if there is any tags
                InputProcessing();
            }
            
        }
            
    }

    private void InputProcessing()
    {

        foreach(var AlternativeCalls in CameraCalls)
        {
            RawFrameInput.ToLower();
            foreach(string AltName in AlternativeCalls.Key)
            {
                if (RawFrameInput.Contains(AltName))
                {
                    CameraSetter.SendMessage(AlternativeCalls.Value);
                }
            }

            
        }

        //medium shot 2 males OR male standing and man sitting medium shot

        foreach (var AlternativeCalls in ObjectsCalls)
        {
            RawFrameInput.ToLower();
            foreach (string AltName in AlternativeCalls.Key)
            {
                if (RawFrameInput.Contains(AltName) && !ObjectTags.Contains(AlternativeCalls.Value) )
                {
                    ObjectTags.Add(AlternativeCalls.Value);
                    Debug.Log("added: " + AlternativeCalls.Value);
                    PlacementController.UpdateRequiredObjects(ObjectTags);
                }
                //else if(!RawFrameInput.Contains(AltName) && ObjectTags.Contains(AltName))
                //{
                //    ObjectTags.Remove(AlternativeCalls.Value);
                //    Debug.Log("removed: " + AlternativeCalls.Value);
                //}

                
            }

        }

        


    }

    private void StartVariationsInit()
    {
        if (CameraSetter == null) { return; }
        if (AOController == null) { return; }

        CameraCalls = CameraSetter.GetAlternateNames();
        ObjectsCalls = AOController.GetAlternateNames();
    }

    
}
