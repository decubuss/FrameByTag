using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraParametersHandler
{
    private CameraSetter CameraSetter;
    private static Dictionary<string[], string> _cameraParameters = new Dictionary<string[], string>(){
            { new string[] { "first third", "screen left" }, "FirstThird" },
            { new string[] { "center", "centered" }, "Center" },
            { new string[] { "last third", "screen right" }, "LastThird" },

            { new string[] { "birds eye", "from above" }, "BirdsEye" },
            { new string[] { "high" }, "High" },
            { new string[] { "eye level" }, "EyeLevel" },
            { new string[] { "low" }, "Low" },
            { new string[] { "mouse eye" }, "MouseEye" },

            { new string[] { "front" }, "Front" },
            { new string[] { "dead front" }, "DeadFront" },
            { new string[] { "right angle" }, "RightAngle" },
            { new string[] { "left angle" }, "LeftAngle" },

            { new string[] { "long shot", "ls", "full shot" }, "LongShot" },
            { new string[] { "medium shot", "ms", "mid shot", "mediumshot" }, "MediumShot" },
            { new string[] { "large shot", "open shot", "really long shot" }, "ExtremelyLongShot" }
        };
    public static Dictionary<string,string> CameraParameters
    {
        get
        {
            return Helper.DictBreakDown(_cameraParameters);
        }
    }

    public CameraParametersHandler(CameraSetter cameraSetter)
    {
        CameraSetter = cameraSetter;
    }

    /*
     private void ShotOptionsHandle(string input)
    {
        ObjectsPlacementController.OnContentPreparedEvent += UpdateCameraTransform;
        _baseDetail.UpdatePosition();

        var ShotOptionNames = CameraSetter.GetAlternateNames();
        var processedInput = input.ToLower();
        foreach (var Option in ShotOptionNames)
        {
            if (input.Contains(Option.Key))
            {
                processedInput = processedInput.Replace(Option.Key, Option.Value);
            }
        }

        foreach (var ShotType in Enum.GetValues(typeof(ShotType)).Cast<ShotType>())
        {
            if (processedInput.Contains(ShotType.ToString()))
                this.Shot = ShotType;//TODO: make apply for a shotype as thirds
            //this.SendMessage(AlternativeCalls.Value);
        }

        foreach (var ThirdType in Enum.GetValues(typeof(HorizontalThird)).Cast<HorizontalThird>())
        {
            if (processedInput.Contains(ThirdType.ToString()))
                this._third = ThirdType;

        }

        foreach (var HorizontalAngle in Enum.GetValues(typeof(HorizontalAngle)).Cast<HorizontalAngle>())
        {
            if (processedInput.Contains(HorizontalAngle.ToString()))
                this._horizontalAngle = HorizontalAngle;
        }

        foreach (var VerticalAngle in Enum.GetValues(typeof(VerticalAngle)).Cast<VerticalAngle>())
        {
            if (processedInput.Contains(VerticalAngle.ToString()))
                this._verticalAngle = VerticalAngle;
        }
        //define shot type here based on objects given from objects placer

    }
         */

}
