using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using OpenNLP.Tools.Parser;



public class CameraParametersHandler
{
    private CameraSetter CameraController;
    private static Dictionary<string[], string> _cameraParametersAltNames = new Dictionary<string[], string>(){
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
            { new string[] { "large shot", "open shot", "ws" }, "ExtremelyLongShot" },
            { new string[] {"closeup","close shot"}, "CloseShot" }
        };
    public static Dictionary<string, string> CameraParametersAltNames
    {
        get
        {
            return Helper.DictBreakDown(_cameraParametersAltNames);
        }
    }

    private ShotParameters _generatedParameters;
    private ShotParameters _resultParameters;
    public readonly ShotParameters DefaultParams = new ShotParameters(CameraSetter.ShotType.DefaultShot,
                                      CameraSetter.HorizontalAngle.Front,
                                      CameraSetter.VerticalAngle.EyeLevel,
                                      CameraSetter.HorizontalThird.Center);

    public CameraParametersHandler(CameraSetter cameraSetter)
    {
        CameraController = cameraSetter;
    }
    public ShotParameters ShotOptionsHandle(string input)
    {
        if(input=="" || input == null)
        {
            return DefaultParams;
        }
        _generatedParameters = GenerateShotByText(input);
        var ShotOptionNames = Helper.DictSortByLength(CameraParametersAltNames);
        var processedInput = input.ToLower();
        foreach (var Option in ShotOptionNames)
        {
            if (input.Contains( Option.Key))
            {
                processedInput = processedInput.Replace(Option.Key, Option.Value);
            }
        }
        
        _resultParameters = new ShotParameters( HandleShotType(processedInput),
                                                          HandleHorizontalAngle(processedInput),
                                                          HandleVerticalAngle(processedInput),
                                                          HandleThird(processedInput));

        return _resultParameters;
    }

    private CameraSetter.ShotType HandleShotType(string processedInput)
    {
        var values = Enum.GetValues(typeof(CameraSetter.ShotType)).Cast<CameraSetter.ShotType>().OrderByDescending(x=>x.ToString().Length);
        foreach (var shotType in values)
        {
            if (processedInput.Contains(shotType.ToString()))
                return shotType;
        }
        return _generatedParameters.ShotType;
    }
    private CameraSetter.HorizontalAngle HandleHorizontalAngle(string processedInput)
    {
        foreach (var horizontalAngle in Enum.GetValues(typeof(CameraSetter.HorizontalAngle)).Cast<CameraSetter.HorizontalAngle>())
        {
            if (processedInput.Contains(horizontalAngle.ToString()))
                return horizontalAngle;
        }
        return _generatedParameters.HAngle;
    }
    private CameraSetter.VerticalAngle HandleVerticalAngle(string processedInput)
    {
        foreach (var verticalAngle in Enum.GetValues(typeof(CameraSetter.VerticalAngle)).Cast<CameraSetter.VerticalAngle>())
        {
            if (processedInput.Contains(verticalAngle.ToString()))
                return verticalAngle;
        }
        return _generatedParameters.VAngle;
    }
    private CameraSetter.HorizontalThird HandleThird(string processedInput)
    {
        foreach (var thirdType in Enum.GetValues(typeof(CameraSetter.HorizontalThird)).Cast<CameraSetter.HorizontalThird>())
        {
            if (processedInput.Contains(thirdType.ToString()))
                return thirdType;
        }
        return _generatedParameters.Third;
    }
    private ShotParameters GenerateShotByText(string text)
    {
        var parts = FrameDescription.ParsedParts;

        //gather first element
        FirstSyntaxElement(parts);
        //gather percentage of elements
        //gather location pointers
        //gather sentence subjects
        var resultShot = DefaultParams;//new ShotParameters();

        return resultShot;
    }
    private void FirstSyntaxElement(Parse[] sentenceParts)
    {
        var firstPart= sentenceParts.ToList();
        Parse firstSign = sentenceParts.FirstOrDefault(x => x.Value == ",");
        if (firstSign != null)
        {
            int signIndex = Array.IndexOf(sentenceParts, firstSign);
            firstPart = sentenceParts.ToList().GetRange(0, signIndex);
        }
        
        var parents = new List<Parse>(); 
        foreach(var part in sentenceParts)
        {
            parents.Add(part.Parent);
            //Debug.Log(part.Value + " : " + part.Parent + " : " + part.Type);
        }
        //var parents
    }
}
