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
    public ShotParameters(ShotType shotType, HorizontalAngle hAngle, VerticalAngle vAngle, HorizontalThird third, bool fromBehind = false)
    {
        ShotType = shotType;
        HAngle = hAngle;
        VAngle = vAngle;
        Third = third;
        isfromBehind = fromBehind;
    }

   
}
