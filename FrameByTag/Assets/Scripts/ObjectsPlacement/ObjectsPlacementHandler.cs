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
    private readonly AvailableObjectsController AOController;
    private readonly ObjectsPlacementController OPController;

    private string _lastProcessedInput;

    public delegate void OnSentenceProcessed(Dictionary<DescriptionTag, ShotElement> itemTags);
    public static event OnSentenceProcessed OnSentenceProcessedEvent;
    public ObjectsPlacementHandler(AvailableObjectsController aocontroller, ObjectsPlacementController opcontroller)
    {
        FrameDescription.OnDescriptionChangedEvent += BreakOnTags;
        AOController = aocontroller;
        OPController = opcontroller;

    }

    
    private void BreakOnTags(string rawinput)
    {
        if(Helper.ExcludeCameraTags(rawinput) == _lastProcessedInput)
        {
            OnSentenceProcessedEvent?.Invoke(null);
            return;
        }
        _lastProcessedInput = Helper.ExcludeCameraTags(rawinput);
        if (string.IsNullOrWhiteSpace(_lastProcessedInput))
        {
            OnSentenceProcessedEvent?.Invoke(null);
            return;
        }


            var input = Regex.Replace(_lastProcessedInput, @"[!?.]+", "");
        input = AddSpaces(input).ToLower();

        //TODO: add dictionary <descriptiontags,elements>
        var tagItemSeq = new Dictionary<DescriptionTag, ShotElement>();
        
        var processedInput = HandleItems(input, ref tagItemSeq);
        processedInput = HandleSpatials(processedInput, ref tagItemSeq);
        processedInput = HandleStates(processedInput, ref tagItemSeq);
        HandleRelations(processedInput, ref tagItemSeq);

        Debug.Log(processedInput);
        OnSentenceProcessedEvent?.Invoke(tagItemSeq);
    }

    private string HandleItems(string rawinput, ref Dictionary<DescriptionTag, ShotElement> itemTags)
    {
        string processedInput = rawinput;
        var altNames = Helper.DictSortByLength(AOController.GetAlternateNames());

        foreach (var altName in altNames)
        {
            if (rawinput.Contains(altName.Key) || Helper.ContainsTag(rawinput, altName.Key))//processedInput.Contains(altName.Key))
            {
                processedInput = processedInput.Replace(altName.Key, altName.Value);
                int index = Array.IndexOf(processedInput.Split(' '), altName.Value);
                var tag = new DescriptionTag(index, altName.Value, TagType.Item);
                itemTags.Add(tag, new ShotElement(altName.Value,rank: HierarchyRank.InFocus));

            }
        }

        return processedInput;
    }
    private string HandleSpatials(string input, ref Dictionary<DescriptionTag, ShotElement> itemTags)
    {
        var sceneSequence = itemTags.Keys.ToList();
        string processedInput = input;
        var sortedSpatials = Helper.DictSortByLength(OPController.GetAlternateNames());

        foreach(var spatial in sortedSpatials)
        {
            if (Helper.ContainsTag(processedInput, spatial.Key))//processedInput.Contains(spatial.Key))
            {
                processedInput = processedInput.Replace(spatial.Key, spatial.Value);
                int index = Array.IndexOf(processedInput.Split(' '), spatial.Value);
                var tag = new DescriptionTag(index, spatial.Value, TagType.Spatial);

                List<string> spatials = new List<string>() { "By" };
                var updatedItem = itemTags.Where(x => x.Value != null).Where(x => x.Key.Index > index).First();
                updatedItem.Value.Rank = spatials.Contains(spatial.Value) ? HierarchyRank.Addition : updatedItem.Value.Rank;
                itemTags[updatedItem.Key] = updatedItem.Value;

                //sceneSequence.Add(tag);
                itemTags.Add(tag, null);
            }
        }

        return processedInput;
    }
    private string HandleStates(string input, ref Dictionary<DescriptionTag, ShotElement> itemTags)
    {
        string result = input;
        var parts = TreeParsing(input).ToArray();
        int lastActionIndex = 0;
        int layer = 1;

        for (int i = 0; i < parts.Length; i++)
        {
            if(parts[i].Type == "VBZ")
            {
                i++;
                result = result.Replace(parts[i].Value, "");
            }
            if (parts[i].Type == "VBG")
            {
                var word = parts[i].Value;//TODO: lemmatize
                //TODO:  name += parts[i + 1].Label == "RB" || parts[i + 1].Label == "RP" ? parts[i + 1].Text : "";
                
                var stateWord = word.First().ToString().ToUpper() + word.Substring(1);
                result = input.Replace(word, stateWord);//TODO: insert Name instead of name
                int verbIndex = GetWordIndex(result, stateWord);//TODO: retruns -1
                var prevItems = itemTags.Where(x=>x.Key.TagType == TagType.Item)
                                               .Where(x => x.Key.Index < verbIndex && x.Key.Index >= lastActionIndex);
                foreach(var prevItem in prevItems)
                {
                    itemTags.FirstOrDefault(x => x.Value.PropName == prevItem.Key.Keyword).Value.State = stateWord;
                    itemTags.FirstOrDefault(x => x.Value.PropName == prevItem.Key.Keyword).Value.Layer = layer;
                    itemTags.FirstOrDefault(x => x.Value.PropName == prevItem.Key.Keyword).Value.Rank = HierarchyRank.InFocus;
                    //TODO: just store and assign
                }
                
                lastActionIndex = i;
                layer++;
            }

        }
        
        return result;
    }
    private void HandleRelations(string input, ref Dictionary<DescriptionTag, ShotElement> itemTags)
    {
        if (itemTags.Values.Count >= 1) 
        { 
            itemTags.Values.First().Rank = HierarchyRank.InFocus;
            itemTags.Values.First().Layer = 1; 
        }

        foreach(var element in itemTags.Values.Where(x=> x != null))
        {
            element.Rank = element.Rank == HierarchyRank.Default ? HierarchyRank.Addition : element.Rank;
            element.Layer = 1;
        }
        
        //TODO: rework this thing, something isnt right
    }
    
    
    private void GatherItemInfo(string processedinput)
    {
        var parts = processedinput.Split(' ');
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].Contains("NN"))
            {
                var result = new Tuple<int, string>(i, parts[i]);
            }
        }
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

    private string AddSpaces(string input)
    {
        string symbols = ",;:";
        var result = input;
        foreach(var symbol in symbols)
        {
            int correction = 0;
            Regex rgx = new Regex(symbol.ToString());
            foreach(Match match in rgx.Matches(result))
            {
                int index = match.Index;
                if (result.ElementAt(index) != ' ')
                {
                    result = result.Insert(match.Index + correction, " ");
                    correction++;
                }
                if (result.ElementAt(index + 1) != ' ')
                {
                    result = result.Insert(match.Index + 1 + correction, " ");
                    correction++;
                }
            }
        }
        return result;
    }
    private int GetWordIndex(string input, string substring)
    {
        int index = Array.IndexOf(input.Split(' '), substring);
        return index;
    }





    public void ShowSentences(string input)
    {
        var sentences = SplitSentences(input);
        foreach (var sentence in sentences)
        {
            var partedSentence = TreeParsing(input);
            //Debug.Log(TreeParsingFull(input));
            foreach (var part in partedSentence)
            {
                Debug.Log(part.Show());

            }
        }
    }

}
