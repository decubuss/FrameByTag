using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using OpenNLP.Tools.Parser;

public static class ShotsStatsExtension
{
    private static string[] UsefulTypes = new string[] { "NNS","NNP", "NN", "VB", "VBG", "JJ", "RB" };
    public static Dictionary<ShotType, float> FirstSyntaxElement(this Dictionary<ShotType, float> shots, Parse[] sentenceParts)
    {
        var usefulParts = sentenceParts.Where(x => UsefulTypes.Contains(x.Type));
        var firstElement = usefulParts.First();
        switch (firstElement.Type)
        {
            case "NNS": 
                shots[ShotType.LongShot] += 0.1f;
                shots[ShotType.ExtremelyLongShot] += 0.1f;
                break;
            
            case "VB": case "VBG":
                shots[ShotType.MediumShot] += 0.2f;
                break;
            case "RB": case "JJ":
                shots[ShotType.CloseShot] += 0.1f;
                shots[ShotType.MediumShot] += 0.1f;
                break;
            default:  case "NN":  case "NNP":
                shots[ShotType.LongShot] += 0.2f;
                break;
        }

        return shots;
    }
    public static Dictionary<ShotType, float> SyntaxElementsInfluence(this Dictionary<ShotType, float> shots, Parse[] sentenceParts)
    {
        var usefulParts = sentenceParts.Where(x => UsefulTypes.Contains(x.Type));
        var groups = usefulParts.GroupBy(x=>x.Type).Select(g => new { Type = g.Key, Count = g.Count() });
        //var baseElement = groups.OrderByDescending(x=>x.Count).First();

        foreach (var baseElement in usefulParts)
        {
            switch (baseElement.Type)
            {
                case "NNS":
                    shots[ShotType.LongShot] += 0.1f;
                    shots[ShotType.ExtremelyLongShot] += 0.1f;
                    break;
                case "NN":
                    shots[ShotType.LongShot] += 0.2f;
                    break;
                case "VBG":
                case "VB":
                    shots[ShotType.MediumShot] += 0.2f;
                    break;
                case "RB":
                case "JJ":
                    shots[ShotType.CloseShot] += 0.1f;
                    shots[ShotType.MediumShot] += 0.1f;
                    break;
                default:
                    shots[ShotType.LongShot] += 0.2f;
                    break;
            }
        }
        

        return shots;
    }
    public static Dictionary<ShotType, float> SpatialElementsInfluence(this Dictionary<ShotType, float> shots, Parse[] sentenceParts)
    {
        string[] spatialTypes = new string[] { "IN" };
        var usefulParts = sentenceParts.Where(x => UsefulTypes.Contains(x.Type)).ToList();
        if (usefulParts.Count != 0)
        {
            shots[ShotType.ExtremelyLongShot] = 0.1f;//TODO: custom value by different spatials
            shots[ShotType.LongShot] = 0.1f;//TODO: custom value by different spatials
        }
        return shots;
    }
    public static Dictionary<ShotType, float> SentenceSubjectsInfluence(this Dictionary<ShotType, float> shots, Parse[] sentenceParts)
    {
        var sentenceSubjectsCount = 0;
        

        string[] spatialTypes = new string[] { "IN" };
        var usefulParts = sentenceParts.Where(x => UsefulTypes.Contains(x.Type)).ToList();
        if (usefulParts.Count != 0)
        {
            shots[ShotType.ExtremelyLongShot] = 0.1f;//TODO: custom value by different spatials
            shots[ShotType.LongShot] = 0.1f;//TODO: custom value by different spatials
        }
        return shots;
    }
    
}

public class CameraParametersHandler
{

    public readonly ShotParameters DefaultParams = new ShotParameters(ShotType.DefaultShot,
                                                                      HorizontalAngle.Front,
                                                                      VerticalAngle.EyeLevel,
                                                                      HorizontalThird.Center);
    
    private static readonly Dictionary<string[], string> _cameraParametersAltNames = new Dictionary<string[], string>(){
            { new string[] { "first third", "screen left" }, "FirstThird" },
            { new string[] { "center", "centered" }, "Center" },
            { new string[] { "last third", "screen right" }, "LastThird" },

            { new string[] { "birds eye", "from above" }, "BirdsEye" },
            { new string[] { "high" }, "High" },
            { new string[] { "eye level" }, "EyeLevel" },
            { new string[] { "low" }, "Low" },
            { new string[] { "mouse eye", "from below" }, "MouseEye" },

            { new string[] { "front" }, "Front" },
            { new string[] { "dead front" }, "DeadFront" },
            { new string[] { "right angle" }, "RightAngle" },
            { new string[] { "left angle" }, "LeftAngle" },

            { new string[] { "long shot", "ls", "full shot" }, "LongShot" },
            { new string[] { "medium shot", "ms", "mid shot", "mediumshot" }, "MediumShot" },
            { new string[] { "large shot", "open shot", "ws" }, "ExtremelyLongShot" },
            { new string[] {"closeup","close shot"}, "CloseShot" },
            { new string[] {"backshot"}, "Backshot" }
        };
    public static Dictionary<string, string> CameraParametersAltNames
    {
        get
        {
            return Helper.DictBreakDown(_cameraParametersAltNames);
        }
    }

    private ShotParameters GeneratedParameters;
    private ShotParameters ResultParameters;
    
    private CameraSetter CameraController;

    public CameraParametersHandler(CameraSetter cameraSetter)
    {
        CameraController = cameraSetter;
    }
    public ShotParameters ShotParametersHandle(string input)
    {
        if(string.IsNullOrEmpty(input)) { return DefaultParams; }
        GeneratedParameters = FrameDescription.ParsedParts != null ? GenerateShotByText() : DefaultParams;

        var ShotOptionNames = Helper.DictSortByLength(CameraParametersAltNames);
        var processedInput = input.ToLower();
        foreach (var Option in ShotOptionNames)
        {
            if (input.Contains( Option.Key))
            {
                processedInput = processedInput.Replace(Option.Key, Option.Value);
            }
        }

        bool isFromBehind = false;
        if (processedInput.Contains("Backshot"))
            isFromBehind = true;
        ResultParameters = new ShotParameters( HandleShotType(processedInput),
                                                HandleHorizontalAngle(processedInput),
                                                HandleVerticalAngle(processedInput),
                                                HandleThird(processedInput),
                                                isFromBehind);//those are return generated if null 

        return ResultParameters;
    }
    private ShotParameters GenerateShotByText()
    {
        var parts = FrameDescription.ParsedParts;

        Dictionary<ShotType, float> shotPriority = new Dictionary<ShotType, float> { { ShotType.CloseShot, 0.0f },
                                                                                     { ShotType.MediumShot, 0.0f },
                                                                                     { ShotType.LongShot, 0.0f },
                                                                                     { ShotType.ExtremelyLongShot, 0.0f } };
        shotPriority = shotPriority.FirstSyntaxElement(parts);
        shotPriority = shotPriority.SyntaxElementsInfluence(parts);
        shotPriority = shotPriority.SpatialElementsInfluence(parts);
        shotPriority = shotPriority.SentenceSubjectsInfluence(parts);

        #region debug
        foreach (var part in parts)
        {
            //Debug.Log(string.Format("{0} <<{1}>> {2}", part.Type, part.Value, part.Parent));
        }
        #endregion debug

        var resultShot = new ShotParameters(shotPriority.OrderByDescending(x => x.Value).First().Key,
                                            DefaultParams.HAngle,
                                            DefaultParams.VAngle,
                                            DefaultParams.Third); //DefaultParams;//new ShotParameters();

        return resultShot;
    }

    private ShotType HandleShotType(string processedInput)
    {
        var values = Enum.GetValues(typeof(ShotType)).Cast<ShotType>().OrderByDescending(x=>x.ToString().Length);
        foreach (var shotType in values)
        {
            if (processedInput.Contains(shotType.ToString()))
                return shotType;
        }
        return GeneratedParameters.ShotType;
    }
    private HorizontalAngle HandleHorizontalAngle(string processedInput)
    {
        foreach (var horizontalAngle in Enum.GetValues(typeof(HorizontalAngle)).Cast<HorizontalAngle>())
        {
            if (processedInput.Contains(horizontalAngle.ToString()))
                return horizontalAngle;
        }
        return GeneratedParameters.HAngle;
    }
    private VerticalAngle HandleVerticalAngle(string processedInput)
    {
        foreach (var verticalAngle in Enum.GetValues(typeof(VerticalAngle)).Cast<VerticalAngle>())
        {
            if (processedInput.Contains(verticalAngle.ToString()))
                return verticalAngle;
        }
        return GeneratedParameters.VAngle;
    }
    private HorizontalThird HandleThird(string processedInput)
    {
        foreach (var thirdType in Enum.GetValues(typeof(HorizontalThird)).Cast<HorizontalThird>())
        {
            if (processedInput.Contains(thirdType.ToString()))
                return thirdType;
        }
        return GeneratedParameters.Third;
    }
    
    
}
