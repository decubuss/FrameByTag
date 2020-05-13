using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpatialApplier 
{
    public static SpatialApplier _instance;
    private static List<Spatial> Spatials = new List<Spatial>()
    {
        new Spatial("By", new string[] { "beside", "by", "by the", "next to" }, ItemCollocation.Object_Spatial_Subject, ShotHierarchyRank.Addition),
        new Spatial("In", new string[] {"at the", "in the", "in"}, ItemCollocation.Object_Spatial_Subject, ShotHierarchyRank.Addition),
        new Spatial("To", new string[] { "to the", "into" }, ItemCollocation.Object_Spatial_Subject, ShotHierarchyRank.InFocus),
        new Spatial("With", new string[] { "to" }, ItemCollocation.Object_Spatial_Subject, ShotHierarchyRank.InFocus),
        new Spatial("Nearby", new string[] { "nearby" }, ItemCollocation.Object_Subject_Spatial, ShotHierarchyRank.InFocus),
        new Spatial("Between", new string[] { "between"}, ItemCollocation.Object_Spatial_Subject, ShotHierarchyRank.InFocus),
        new Spatial("Front", new string[] { "in front of" }, ItemCollocation.Object_Spatial_Subject, ShotHierarchyRank.Addition),
        new Spatial("Behind", new string[] { "behind the" }, ItemCollocation.Object_Spatial_Subject, ShotHierarchyRank.Addition),
        new Spatial("After", new string[] {"behind", "after"}, ItemCollocation.Object_Subject_Spatial, ShotHierarchyRank.InFocus)

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
    public void Apply(DescriptionTag spatialTag, ref Dictionary<ShotElement, GameObject> spawnedElements, Dictionary<DescriptionTag, ShotElement> tagItemDict)
    {
        var spatial = GetSpatial(spatialTag.Keyword);
        var spatialSubjGroup = spatial.FindSpatialSubject(tagItemDict, spatialTag.Index);
        var spatialObjGroup = spatial.FindSpatialObject(tagItemDict, spatialTag.Index, spatialSubjGroup.Key.Index).ToDictionary(g => g.Key, g => g.Value);//.ToDictionary<DescriptionTag>();

        List<GameObject> goObjGroup = new List<GameObject>();
        foreach(var obj in spatialObjGroup)
        {
            if(spawnedElements.ContainsKey(obj.Value))
                goObjGroup.Add(spawnedElements[obj.Value]);
        }

        var goSubjGroup = spawnedElements[spatialSubjGroup.Value];

        switch (spatial.Name)
        {
            case "By":
                ObjectBySubject(goObjGroup, goSubjGroup);
                break;
            case "In":
                break;
            case "To":
                ObjectTosubject(goObjGroup, goSubjGroup);
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
                Debug.LogError("yo pierre da shit is fucked - something is tryna to look like a spatial but it is not");
                break;
        }
    }
    public static Spatial GetSpatial(string spatialName)
    {
        if(Spatials.Count == 0) { Debug.LogError("there is no spatials smh"); return null; }
        return Spatials.Find(x => x.Name == spatialName);
    }


    public void ObjectBySubject(List<GameObject> objectPointer, GameObject subjectPointer)
    {
        Vector3 objectPos = ObjectsPlacementController.GroupAveragePos(objectPointer);
        Bounds absoluteBounds = ObjectsPlacementController.GroupBounds(objectPointer);
        Vector3 objectDir = ObjectsPlacementController.GroupAverageDirection(objectPointer);

        subjectPointer.transform.position = (-objectDir * (absoluteBounds.size.z + subjectPointer.GetComponent<SceneObject>().Bounds.size.z)/2) + objectPos;
    }
    public void ObjectTosubject(List<GameObject> objectPointer, GameObject subjectPointer)
    {
        Vector3 objectPos = ObjectsPlacementController.GroupAveragePos(objectPointer);
        Bounds absoluteBounds = ObjectsPlacementController.GroupBounds(objectPointer);
        Vector3 objectDir = ObjectsPlacementController.GroupAverageDirection(objectPointer);

        subjectPointer.transform.position = (objectDir * (absoluteBounds.size.z + subjectPointer.GetComponent<SceneObject>().Bounds.size.y) / 2) + objectPos;
    }
}
