using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ItemCollocation
{
    Object_Spatial_Subject, Object_Subject_Spatial
}

public class Spatial 
{
    public ShotHierarchyRank SubjectRank;
    public ItemCollocation Collocation;
    private string _name;
    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value.MakeCapitalLetter();
        }

    }
    public string[] AltNames;
    public Spatial(string name, string[] altNames, ItemCollocation collocation, ShotHierarchyRank subjectRank)
    {
        Name = name;
        AltNames = altNames;
        Collocation = collocation;
        SubjectRank = subjectRank;
    }
    
    public IEnumerable<KeyValuePair<DescriptionTag, ShotElement>> FindSpatialObject(Dictionary<DescriptionTag, ShotElement> itemTags,
                                                                int spatialIndex,
                                                                int subjectIndex)
    {
        var nearestItem = this.Collocation == ItemCollocation.Object_Spatial_Subject ?
                    itemTags.Where(x => x.Value != null)
                                         .Where(x => x.Key.Index < spatialIndex)
                                         .OrderByDescending(x => x.Key.Index)
                                         .First()
                    :
                    itemTags.Where(x => x.Value != null)
                                         .Where(x => x.Key.Index < subjectIndex)
                                         .OrderByDescending(x => x.Key.Index)
                                         .Last();
        var result = itemTags.Where(x => x.Value != null && 
                                    x.Value.Layer == nearestItem.Value.Layer &&
                                    x.Key.Index < subjectIndex);//b-fore spatial subject
        return result;
    }
    public KeyValuePair<DescriptionTag, ShotElement> FindSpatialSubject(Dictionary<DescriptionTag, ShotElement> itemTags, int spatialIndex)
    {
        var nearestItem = this.Collocation == ItemCollocation.Object_Spatial_Subject ?
                    itemTags.Where(x => x.Value != null)
                                         .Where(x => x.Key.Index > spatialIndex)
                                         .OrderByDescending(x => x.Key.Index)
                                         .Last()
                    :
                    itemTags.Where(x => x.Value != null)
                                         .Where(x => x.Key.Index < spatialIndex)
                                         .OrderByDescending(x => x.Key.Index)
                                         .First();
        //var result = itemTags.Where(x => x.Value != null && x.Value.Layer == nearestItem.Value.Layer);
        return nearestItem;
    }
}
