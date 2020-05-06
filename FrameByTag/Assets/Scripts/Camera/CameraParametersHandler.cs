using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using OpenNLP.Tools.Parser;

public static class ShotsStatsExtension
{
    private static string[] UsefulTypes = new string[] { "NNS", "NN", "VB", "VBG", "JJ", "RB" };
    public static Dictionary<CameraSetter.ShotType, float> FirstSyntaxElement(this Dictionary<CameraSetter.ShotType, float> shots, Parse[] sentenceParts)
    {
        var usefulParts = sentenceParts.Where(x => UsefulTypes.Contains(x.Type));
        var firstElement = usefulParts.First();

        switch (firstElement.Type)
        {
            case "NNS":
                shots[CameraSetter.ShotType.LongShot] += 0.1f;
                shots[CameraSetter.ShotType.ExtremelyLongShot] += 0.1f;
                break;
            case "NN":
                shots[CameraSetter.ShotType.LongShot] += 0.2f;
                break;
            case "VB":
                shots[CameraSetter.ShotType.MediumShot] += 0.2f;
                break;
            case "VBG":
                shots[CameraSetter.ShotType.MediumShot] += 0.2f;
                break;
            case "JJ":
                shots[CameraSetter.ShotType.CloseShot] += 0.1f;
                shots[CameraSetter.ShotType.MediumShot] += 0.1f;
                break;
            case "RB":
                shots[CameraSetter.ShotType.CloseShot] += 0.1f;
                shots[CameraSetter.ShotType.MediumShot] += 0.1f;
                break;
            default:
                shots[CameraSetter.ShotType.LongShot] += 0.2f;
                break;
        }

        return shots;
    }
    public static Dictionary<CameraSetter.ShotType, float> SyntaxElementsInfluence(this Dictionary<CameraSetter.ShotType, float> shots, Parse[] sentenceParts)
    {
        var usefulParts = sentenceParts.Where(x => UsefulTypes.Contains(x.Type));
        var groups = usefulParts.GroupBy(x=>x.Type).Select(g => new { Type = g.Key, Count = g.Count() });
        var baseElement = groups.OrderByDescending(x=>x.Count).First();

        switch (baseElement.Type)
        {
            case "NNS":
                shots[CameraSetter.ShotType.LongShot] += 0.1f;
                shots[CameraSetter.ShotType.ExtremelyLongShot] += 0.1f;
                break;
            case "NN":
                shots[CameraSetter.ShotType.LongShot] += 0.2f;
                break;
            case "VB":
                shots[CameraSetter.ShotType.MediumShot] += 0.2f;
                break;
            case "VBG":
                shots[CameraSetter.ShotType.MediumShot] += 0.2f;
                break;
            case "JJ":
                shots[CameraSetter.ShotType.CloseShot] += 0.1f;
                shots[CameraSetter.ShotType.MediumShot] += 0.1f;
                break;
            case "RB":
                shots[CameraSetter.ShotType.CloseShot] += 0.1f;
                shots[CameraSetter.ShotType.MediumShot] += 0.1f;
                break;
            default:
                shots[CameraSetter.ShotType.LongShot] += 0.2f;
                break;
        }

        return shots;
    }
    public static Dictionary<CameraSetter.ShotType, float> SpatialElementsInfluence(this Dictionary<CameraSetter.ShotType, float> shots, Parse[] sentenceParts)
    {
        return new Dictionary<CameraSetter.ShotType, float>();
    }
    public static Dictionary<CameraSetter.ShotType, float> SentenceSubjectsInfluence(this Dictionary<CameraSetter.ShotType, float> shots, Parse[] sentenceParts)
    {
        return new Dictionary<CameraSetter.ShotType, float>();
    }
    
}

public class CameraParametersHandler
{

    public readonly ShotParameters DefaultParams = new ShotParameters(CameraSetter.ShotType.DefaultShot,
                                                                      CameraSetter.HorizontalAngle.Front,
                                                                      CameraSetter.VerticalAngle.EyeLevel,
                                                                      CameraSetter.HorizontalThird.Center);

    
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
    
    private CameraSetter CameraController;

    public CameraParametersHandler(CameraSetter cameraSetter)
    {
        CameraController = cameraSetter;
    }
    public ShotParameters ShotOptionsHandle(string input)
    {
        if(string.IsNullOrEmpty(input)) { return DefaultParams; }
        _generatedParameters = FrameDescription.ParsedParts != null ? GenerateShotByText() : DefaultParams;

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
                                                HandleThird(processedInput) );//those are return generated if null 

        return _resultParameters;
    }
    private ShotParameters GenerateShotByText()
    {
        var parts = FrameDescription.ParsedParts;

        Dictionary<CameraSetter.ShotType, float> shotPriority = new Dictionary<CameraSetter.ShotType, float> { { CameraSetter.ShotType.CloseShot, 0.0f },
                                                                                                                  { CameraSetter.ShotType.MediumShot, 0.0f },
                                                                                                                  { CameraSetter.ShotType.LongShot, 0.0f },
                                                                                                                  { CameraSetter.ShotType.ExtremelyLongShot, 0.0f } };
        shotPriority = shotPriority.FirstSyntaxElement(parts);
        shotPriority = shotPriority.SyntaxElementsInfluence(parts);
        //gather location pointers
        shotPriority = shotPriority.SpatialElementsInfluence(parts);
        //gather sentence subjects
        //shotPriority = shotPriority.SentenceSubjectsInfluence(parts);

        Debug.Log(shotPriority[CameraSetter.ShotType.LongShot]);
        foreach(var part in parts)
        {
            Debug.Log(string.Format("{0} {1} {2}", part.Type, part.Value, part.Parent));
        }

        var resultShot = new ShotParameters(shotPriority.OrderByDescending(x => x.Value).First().Key,
                                            DefaultParams.HAngle,
                                            DefaultParams.VAngle,
                                            DefaultParams.Third); //DefaultParams;//new ShotParameters();

        return resultShot;
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
    
    
}
