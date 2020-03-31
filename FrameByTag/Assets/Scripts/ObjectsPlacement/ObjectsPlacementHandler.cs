using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.RegularExpressions;
using System.Linq;
using OpenNLP.Tools.SentenceDetect;
using OpenNLP.Tools.Coreference.Mention;
using OpenNLP.Tools.Parser;
using System.IO;
using OpenNLP.Tools.Lang.English;
using System;

public class ObjectsPlacementHandler
{
    public ObjectsPlacementHandler()
    {
        FrameDescription.OnDescriptionChangedEvent += BreakOnTags;
    }

    private void BreakOnTags(string rawinput)
    {
        var sentences = SplitSentences(rawinput);
        foreach(var sentence in sentences)
        {
            var partedSentence = TreeParsing(rawinput);
            Debug.Log(TreeParsingFull(rawinput));
            foreach(var part in partedSentence)
            {
                Debug.Log(part.Show());

            }
        }

        //string coreferents = IdentifyCoreferents(sentences);
        //Debug.Log(coreferents);
    }


    private void IdentifyItems(string[] parts)
    {
        var sceneObjects = new List<Tuple<int, string>>();
        for(int i = 0; i < parts.Length; i++)
        {
            if (parts[i].Contains("NN"))
            {
                var result = new Tuple<int, string>(i, parts[i]);
                sceneObjects.Add(result);
            }
        }
    }
    private void IdentifyActions()
    {

    }
    private void IdentifyPlacing()
    {

    }



    static void PartsDetection(string expression, int index)
    {

        if (expression[index] != '(')
        {
            return;
        }

        Stack st = new Stack();

        for (int i = index; i < expression.Length; i++)
        {
            if (expression[i] == '(')
            {
                st.Push((int)expression[i]);
            } 
            else if (expression[i] == ')')
            {
                st.Pop();
                if (st.Count == 0)
                {
                    return;
                }
            }
        }
        
    }
    private Parse[] TreeParsing(string input)
    {
        var modelPath = Directory.GetCurrentDirectory() + @"\Models\";
        var parser = new EnglishTreebankParser(modelPath);
        var treeParsing = parser.DoParse(input);
        return treeParsing.GetTagNodes();//.Show;
    }

    private string TreeParsingFull(string input)
    {
        var modelPath = Directory.GetCurrentDirectory() + @"\Models\";
        var parser = new EnglishTreebankParser(modelPath);
        var treeParsing = parser.DoParse(input);
        return treeParsing.Show();
    }

    private string IdentifyCoreferents(IEnumerable<string> sentences)
    {
        var corefPath = Directory.GetCurrentDirectory() + @"\Models\Coref\";
        var coreferenceFinder = new TreebankLinker(corefPath);
        var parsedSentences = new List<Parse>();
        foreach (string sentence in sentences)
        {
            Parse sentenceParse = ParseSentence(sentence);
            parsedSentences.Add(sentenceParse);
        }
        return coreferenceFinder.GetCoreferenceParse(parsedSentences.ToArray());
    }

    private Parse ParseSentence(string sentence)
    {
        var _parser = new EnglishTreebankParser(Directory.GetCurrentDirectory() + @"\Models\", true, false);

        return _parser.DoParse(sentence);
    }

    private string[] SplitSentences(string paragraph)
    {
        var _sentenceDetector = new EnglishMaximumEntropySentenceDetector(Directory.GetCurrentDirectory() + @"\Models\" + "EnglishSD.nbin");

        return _sentenceDetector.SentenceDetect(paragraph);
    }
}
