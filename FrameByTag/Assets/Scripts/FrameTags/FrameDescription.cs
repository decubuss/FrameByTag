using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

public class FrameDescription : MonoBehaviour
{
    // Start is called before the first frame update
    private string PreviousFrameInput;
    public string CurrentFrameInput;

    private CameraSetter CameraSetter;
    private ObjectsController ObjectsController;
    //object placer
    //layers controller
    //lighting ???

    public void SetCurrentFrameInput(string FrameInput)
    {
        this.CurrentFrameInput = FrameInput;
    }

    void Start()
    {
        CameraSetter = FindObjectOfType<CameraSetter>();
        ObjectsController = FindObjectOfType<ObjectsController>();
        //var Type = CameraRoller.GetType();
    }

    void Update()
    {
        UpdateInput();
    }

    private void UpdateInput()
    {
        if(this.CurrentFrameInput != this.PreviousFrameInput)
        {
            PreviousFrameInput = CurrentFrameInput;
            //and then run a function that defines if there is any tags
            InputProcessing();
        }
    }

    private void InputProcessing()
    {
        List<string> FrameTags = new List<string>();
        var SplittedInput = CurrentFrameInput.Split(' ');
        foreach(string Word in SplittedInput)//TODO: is it actually got to be a List? Figure out will ya?
        {
            FrameTags.Add(Word);
        }

        if(ObjectsController.GetAllObjects() != null && FrameTags.Any( item => ObjectsController.GetAllObjects().Contains(item)))
        {
            var Methods = CameraSetter.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (MethodInfo Method in Methods)
            {
                var Name = Method.Name;
                var ProcessedInput = CurrentFrameInput;
                if (CurrentFrameInput.Contains(Name))
                {
                    CameraSetter.SendMessage(Name);
                }
                //Debug.Log(Name);

            }
            //char[] TrimChars = [',', ' '];
            //var TagsSequence = CurrentFrameInput.Trim(TrimChars);
        }

    }
}
