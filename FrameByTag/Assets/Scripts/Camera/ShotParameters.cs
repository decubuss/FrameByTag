using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ShotParameters
{
    public ShotType ShotType;
    public HorizontalAngle HAngle;
    public VerticalAngle VAngle;
    public HorizontalThird Third;
    public bool isfromBehind;
    public bool isYPowered;
    public ShotParameters(ShotType shotType, HorizontalAngle hAngle, VerticalAngle vAngle, HorizontalThird third, bool fromBehind = false, bool yPowered = true)
    {
        ShotType = shotType;
        HAngle = hAngle;
        VAngle = vAngle;
        Third = third;
        isfromBehind = fromBehind;
        isYPowered = yPowered;
    }

   
}
