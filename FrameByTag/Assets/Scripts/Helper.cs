using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using LemmaSharp;
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
    public static string ExcludeCameraTags(string input)
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
        return result;
    }
    public static string MakeCapitalLetter(this string word)
    {
        string result = word.First().ToString().ToUpper() + word.Substring(1);
        return result;
    }
    public static string LemmatizeOne(string word)
    {
        ILemmatizer lmtz = new LemmatizerPrebuiltFull(LemmaSharp.LanguagePrebuilt.English);
        string lemma = lmtz.Lemmatize(word);
        return lemma;
    }
}
