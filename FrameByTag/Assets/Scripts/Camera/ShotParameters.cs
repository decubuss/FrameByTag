using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ShotParameters
{
    public CameraSetter.ShotType ShotType;
    public CameraSetter.HorizontalAngle HAngle;
    public CameraSetter.VerticalAngle VAngle;
    public CameraSetter.HorizontalThird Third;
    public ShotParameters(CameraSetter.ShotType shotType, CameraSetter.HorizontalAngle hAngle, CameraSetter.VerticalAngle vAngle, CameraSetter.HorizontalThird third)
    {
        ShotType = shotType;
        HAngle = hAngle;
        VAngle = vAngle;
        Third = third;
    }
}
