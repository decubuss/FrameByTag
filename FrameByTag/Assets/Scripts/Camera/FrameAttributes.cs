using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameAttributes : ScriptableObject
{
    public Vector3 LeftCorner;
    public Vector3 RightCorner;
    public Vector3 CenterOfFrame;

    public FrameAttributes()
    {
        LeftCorner = Vector3.zero;
        RightCorner = Vector3.zero;
        CenterOfFrame = Vector3.zero;
    }
}
