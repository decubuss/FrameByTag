using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FrameDescription : MonoBehaviour
{
    public static FrameDescription instance;
    public InputField DescriptionSource;

    public static string RawFrameInput;

    public delegate void OnDescriptionChangeDelegate(string Input);
    public static event OnDescriptionChangeDelegate OnDescriptionChangedEvent;

    void Start()
    {

        if (DescriptionSource == null)
        {
            DescriptionSource = GameObject.Find("DescriptionField").GetComponent<InputField>();
        }
    }

    public void OnInputEnter()
    {
        //TODO: MINOR - make cursor stand there after enter
        if (DescriptionSource.text != "" && DescriptionSource.text != RawFrameInput)
        {
            RawFrameInput = DescriptionSource.text;

            OnDescriptionChangedEvent?.Invoke(RawFrameInput);
        }
    }
  
}
