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
    public string RawFrameInput;

    private InputField DescriptionSource;
    private CameraSetter CameraSetter;
    private ObjectsController ObjectsController;
    private Dictionary<string, List<string>> ObjectsBones;
    private Dictionary<string[], string> AlternateValues;
    //object placer
    //layers controller
    //lighting ???

    public void SetCurrentFrameInput(string FrameInput)
    {
        this.RawFrameInput = FrameInput;
    }

    void Start()
    {
        //AlternateValues = new 
        DescriptionSource = FindObjectOfType<InputField>();

        Debug.Log(DescriptionSource);

        ObjectsBones = new Dictionary<string, List<string>>();
        CameraSetter = FindObjectOfType<CameraSetter>();
        ObjectsController = FindObjectOfType<ObjectsController>();

        StartVariationsInit();

        GetObjectsBones();
        //var Type = CameraRoller.GetType();
    }

    void Update()
    {
        UpdateInput();
    }

    private void UpdateInput()
    {
        RawFrameInput = DescriptionSource.text;

        if(this.RawFrameInput != this.PreviousFrameInput)
        {
            PreviousFrameInput = RawFrameInput;
            //and then run a function that defines if there is any tags
            InputProcessing();
        }
    }

    private void InputProcessing()
    {
        //List<string> FrameTags = new List<string>();
        //var SplittedInput = RawFrameInput.Split(' ');
        //foreach(string Word in SplittedInput)//TODO: is it actually got to be a List? Figure out will ya?
        //{
        //    FrameTags.Add(Word);
        //}

        //if(ObjectsController.GetAllObjects() != null && FrameTags.Any( item => ObjectsController.GetAllObjects().Contains(item)))
        //{
        //    var Methods = CameraSetter.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);

        //    foreach (MethodInfo Method in Methods)
        //    {
        //        var Name = Method.Name;
        //        var ProcessedInput = RawFrameInput;
        //        if (RawFrameInput.Contains(Name))
        //        {
        //            CameraSetter.SendMessage(Name);
        //        }

        //    }
            
        //}

        foreach(var AlternativeCalls in AlternateValues)
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
    }

    private void GetObjectsBones()
    {
        foreach(var Object in ObjectsController.FocusedObjects)
        {
            if(Object.GetComponentInChildren<SkinnedMeshRenderer>() != null)
            {
                List<string> Bones = new List<string>();
                foreach(var bone in Object.GetComponentInChildren<SkinnedMeshRenderer>().bones)
                {
                    Bones.Add(bone.name);
                }
                ObjectsBones.Add(Object.name, Bones);
            }
        }
    }

    private void StartVariationsInit()
    {
        if (CameraSetter == null) { return; }

        AlternateValues = CameraSetter.GetAlternateNames();
    }
}
