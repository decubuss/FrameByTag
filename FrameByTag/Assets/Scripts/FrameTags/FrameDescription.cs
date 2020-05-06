using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using OpenNLP.Tools.SentenceDetect;
using OpenNLP.Tools.Coreference.Mention;
using OpenNLP.Tools.Parser;
using OpenNLP.Tools.Lang.English;
using System.IO;

public class FrameDescription : MonoBehaviour
{
    public static FrameDescription instance;
    public InputField DescriptionSource;

    public static string RawFrameInput;
    public static Parse[] ParsedParts;

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
        if (!string.IsNullOrWhiteSpace(DescriptionSource.text) && DescriptionSource.text != RawFrameInput)
        {
            RawFrameInput = DescriptionSource.text;
            string rawText = Helper.ExcludeCameraTags(RawFrameInput);
            if (!string.IsNullOrEmpty(rawText))
                ParsedParts = TreeParsing(rawText).ToArray();
            else
                ParsedParts = null;

            OnDescriptionChangedEvent?.Invoke(RawFrameInput);
        }
    }
    private Parse[] TreeParsing(string input)
    {
        var modelPath = Directory.GetCurrentDirectory() + @"\Models\";
        var parser = new EnglishTreebankParser(modelPath);
        var treeParsing = parser.DoParse(Helper.ExcludeCameraTags(input));

        return treeParsing.GetTagNodes();//.Show;
    }

}
