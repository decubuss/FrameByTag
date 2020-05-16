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
    public static string LastTaggedInput;
    private string LastRawInput;

    public delegate void OnSentenceProcessed(Dictionary<DescriptionTag, ShotElement> itemTags);
    public static event OnSentenceProcessed OnSentenceProcessedEvent;
    public ObjectsPlacementHandler()
    {
        FrameDescription.OnDescriptionChangedEvent += ShotElementsBreakdown;
    }
    
    private void ShotElementsBreakdown(string rawinput)
    {
        if(Helper.ExcludeCameraTags(rawinput) == LastRawInput)
        {
            OnSentenceProcessedEvent?.Invoke(null);
            return;
        }
        LastRawInput = Helper.ExcludeCameraTags(rawinput);
        if (string.IsNullOrWhiteSpace(LastRawInput))
        {
            OnSentenceProcessedEvent?.Invoke(null);
            return;
        }

        var input = Regex.Replace(LastRawInput, @"[!?.]+", "");
        input = input.AddSpacesBetweenElements();//AddSpaces(input).ToLower();

        //TODO: add dictionary <descriptiontags,elements>
        var tagItemSeq = new Dictionary<DescriptionTag, ShotElement>();
        
        var processedInput = HandleItems(input, ref tagItemSeq);
        processedInput = HandleSpatials(processedInput, ref tagItemSeq);//TODO: checkout
        processedInput = HandleStates(processedInput, ref tagItemSeq);
        //HandleRelations(processedInput, ref tagItemSeq);
        foreach (var tag in tagItemSeq.Values)
        {
            if (tag != null)
                Debug.Log(tag.ToString());
        }


        LastTaggedInput = processedInput;
        //Debug.Log(processedInput);
        OnSentenceProcessedEvent?.Invoke(tagItemSeq);
    }
    private string HandleItems(string rawinput, ref Dictionary<DescriptionTag, ShotElement> itemTags)
    {
        string processedInput = rawinput;
        var altNames = Helper.DictSortByLength(AvailableObjectsController.GetAlternateNames());

        foreach (var altName in altNames)
        {
            if (rawinput.Contains(altName.Key) || Helper.ContainsTag(rawinput, altName.Key))//processedInput.Contains(altName.Key))
            {
                processedInput = processedInput.Replace(altName.Key, altName.Value);
                int index = Array.IndexOf(processedInput.Split(' '), altName.Value);
                var tag = new DescriptionTag(index, altName.Value, TagType.Item);

                itemTags.Add(tag, new ShotElement(altName.Value, rank: ShotHierarchyRank.InFocus));
            }
        }

        return processedInput;
    }
    private string HandleSpatials(string input, ref Dictionary<DescriptionTag, ShotElement> itemTags)
    {
        var sceneSequence = itemTags.Keys.ToList();
        string markedSpatials = input;
        var spatialDict = Helper.DictSortByLength(SpatialApplier.GetAlternateNames());

        foreach (var spatialName in spatialDict)
        {
            if (Helper.ContainsTag(markedSpatials, spatialName.Key))//processedInput.Contains(spatial.Key))
            {
                markedSpatials = markedSpatials.Replace(spatialName.Key, spatialName.Value);
                int spatialIndex = Array.IndexOf(markedSpatials.Split(' '), spatialName.Value);
                var tag = new DescriptionTag(spatialIndex, spatialName.Value, TagType.Spatial);

                var spatial = SpatialApplier.GetSpatial(spatialName.Value);

                var subjectItemGroup = spatial.FindSpatialSubject(itemTags, spatialIndex);
                var objectItemGroup = spatial.FindSpatialObject(itemTags, spatialIndex, spatialIndex);

                subjectItemGroup.Value.Rank = spatial.SubjectRank;
                subjectItemGroup.Value.Layer = subjectItemGroup.Value.Rank == ShotHierarchyRank.Addition ? 
                                                                                objectItemGroup.First().Value.Layer : 
                                                                                objectItemGroup.First().Value.Layer + 1;
                itemTags[subjectItemGroup.Key] = subjectItemGroup.Value;

                itemTags.Add(tag, null);
            }
        }

        return markedSpatials;
    }
    private string HandleStates(string input, ref Dictionary<DescriptionTag, ShotElement> itemTags)
    {
        string result = input;
        var parts = FrameDescription.ParsedParts;
        int lastActionIndex = 0;
        int layer = 0;

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

                var stateWord = word.MakeCapitalLetter();//word.First().ToString().ToUpper() + word.Substring(1);
                result = input.Replace(word, stateWord);//TODO: insert Name instead of name
                int verbIndex = result.GetWordIndex(stateWord);//GetWordIndex(result, stateWord);
                var prevItems = itemTags.Where(x=>x.Key.TagType == TagType.Item)
                                               .Where(x => x.Key.Index < verbIndex && x.Key.Index >= lastActionIndex);
                foreach(var prevItem in prevItems)
                {
                    itemTags.FirstOrDefault(x => x.Value.PropName == prevItem.Key.Keyword).Value.State = stateWord;
                    itemTags.FirstOrDefault(x => x.Value.PropName == prevItem.Key.Keyword).Value.Layer = layer;
                    itemTags.FirstOrDefault(x => x.Value.PropName == prevItem.Key.Keyword).Value.Rank = ShotHierarchyRank.InFocus;
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
            itemTags.Values.First().Rank = ShotHierarchyRank.InFocus;
            itemTags.Values.First().Layer = 1; 
        }

        foreach(var element in itemTags.Values.Where(x=> x != null))
        {
            element.Rank = element.Rank == ShotHierarchyRank.Default ? ShotHierarchyRank.Addition : element.Rank;
            element.Layer = 1;
        }
        
        //TODO: rework this thing, something isnt right
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






}
