using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpatialApplier 
{
    public static SpatialApplier _instance;
    private static List<Spatial> Spatials = new List<Spatial>()
    {
        new Spatial("By", new string[] { "beside", "by", "by the", "next to" }, ItemCollocation.Object_Spatial_Subject, HierarchyRank.Addition),
        new Spatial("In", new string[] {"at the", "in the", "in"}, ItemCollocation.Object_Spatial_Subject, HierarchyRank.Addition),
        new Spatial("To", new string[] { "to the", "into" }, ItemCollocation.Object_Spatial_Subject, HierarchyRank.InFocus),
        new Spatial("With", new string[] { "to" }, ItemCollocation.Object_Spatial_Subject, HierarchyRank.InFocus),
        new Spatial("Nearby", new string[] { "nearby" }, ItemCollocation.Object_Subject_Spatial, HierarchyRank.InFocus),
        new Spatial("Between", new string[] { "between"}, ItemCollocation.Object_Spatial_Subject, HierarchyRank.InFocus),
        new Spatial("Front", new string[] { "in front of" }, ItemCollocation.Object_Spatial_Subject, HierarchyRank.Addition),
        new Spatial("Behind", new string[] { "behind the" }, ItemCollocation.Object_Spatial_Subject, HierarchyRank.Addition),
        new Spatial("After", new string[] {"behind", "after"}, ItemCollocation.Object_Subject_Spatial, HierarchyRank.InFocus)

    };
    private static Dictionary<string[], string> AltNames = new Dictionary<string[], string>();
    public SpatialApplier()
    {
        foreach(var spatial in Spatials)
        {
            AltNames.Add(spatial.AltNames, spatial.Name);
        }
    }
    //TODO: constructor which assembles list of alt names
    //TODO: functions of EACH spatial for 1 day
    public static Dictionary<string, string> GetAlternateNames()
    {
        var result = Helper.DictBreakDown(AltNames);

        return result;
    }
    public static void Apply(string spatialName)
    {
        switch (spatialName)
        {
            case "By":
                break;
            case "In":
                break;
            case "To":
                break;
            case "With":
                break;
            case "Nearby":
                break;
            case "Between":
                break;
            case "Front":
                break;
            case "Behind":
                break;
            case "After":
                break;
            default:
                break;
        }
    }
    public static Spatial GetSpatial(string spatialName)
    {
        if(Spatials.Count == 0) { Debug.LogError("there is no spatials smh"); return null; }
        return Spatials.Find(x => x.Name == spatialName);
    }

    private void AverageLayerPosition() { }
}
