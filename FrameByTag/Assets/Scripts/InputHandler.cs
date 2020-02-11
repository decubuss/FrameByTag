using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{

    public InputField InputSource;
    public FrameDescription FrameDescription;

    // Start is called before the first frame update
    void Start()
    {
        if(InputSource == null)
            InputSource = FindObjectOfType<InputField>();
        if (FrameDescription == null)
            FrameDescription = FindObjectOfType<FrameDescription>();
    }

    // Update is called once per frame
    void Update()
    {
        //go back to input filed and figure it all out together there
    }

    public void OnValueChange()
    {
        Debug.Log("hey there");
    }
}
