﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

using LemmaSharp;
using System.Text.RegularExpressions;
public static class Helper
{
    public static Dictionary<string, string> DictSortByLength(Dictionary<string, string> dict)
    {
        var result = new Dictionary<string, string>();
        var keys = dict.Keys.OrderByDescending(x => x.Length);
        foreach (var key in keys)
        {
            result.Add(key, dict[key]);
        }

        return result;
    }
    public static Dictionary<string, string> DictBreakDown(Dictionary<string[], string> dict)
    {
        var result = new Dictionary<string, string>();
        foreach (var spatial in dict)
        {
            foreach (var call in spatial.Key)
            {
                result.Add(call, spatial.Value);
            }
        }
        return result;
    }
    public static bool ContainsTag(string str, string tag)
    {
        if (str.StartsWith(tag + " ") || str.EndsWith(" " + tag) || str.Contains(" " + tag + " "))
            return true;
        else
            return false;
    }

    public static string ExcludeCameraTags(this string input)
    {
        string result = input;
        foreach (var tag in DictSortByLength(CameraParametersHandler.CameraParametersAltNames))
        {
            if (result.Contains(" " + tag.Key))
                result = result.Replace(" " + tag.Key, "");
            else if (result.Contains(tag.Key + " "))
                result = result.Replace(tag.Key + " ", "");
            else if (result.Contains(tag.Key))
                result = result.Replace(tag.Key, "");
        }
        //result = result.Last() == ' ' ? result.Remove(result.Length - 1) : result;
        return result;
    }
    public static string MakeCapitalLetter(this string word)
    {
        string result = word.First().ToString().ToUpper() + word.Substring(1);
        return result;
    }
    public static string AddSpacesBetweenElements(this string phrase)
    {
        string symbols = ",;:";
        var result = phrase;
        foreach (var symbol in symbols)
        {
            int correction = 0;
            Regex rgx = new Regex(symbol.ToString());
            foreach (Match match in rgx.Matches(result))
            {
                int index = match.Index;
                if (result.ElementAt(index + correction) != ' ')
                {
                    result = result.Insert(match.Index + correction, " ");
                    correction++;
                }
                if (result.ElementAt(index + 1 + correction) != ' ')
                {
                    result = result.Insert(match.Index + 1 + correction, " ");
                    correction++;
                }
            }
        }
        return result;
    }
    public static int GetWordIndex(this string input, string word)
    {
        int index = Array.IndexOf(input.Split(' '), word);
        return index;
    }

    public static bool myApproximation(float a, float b, float tolerance)
    {
        return (Mathf.Abs(a - b) < tolerance);
    }
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == aName)
                return c;
            foreach (Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }
    public static List<GameObject> GetAllChildren(this GameObject go)
    {
        var result = new List<GameObject>();
        int children = go.transform.childCount;
        for (int i = 0; i < children; ++i)
            result.Add(go.transform.GetChild(i).gameObject);
        return result;
    }
    public static string LemmatizeOne(string word)
    {
        ILemmatizer lmtz = new LemmatizerPrebuiltFull(LemmaSharp.LanguagePrebuilt.English);
        string lemma = lmtz.Lemmatize(word);
        return lemma;
    }
}
