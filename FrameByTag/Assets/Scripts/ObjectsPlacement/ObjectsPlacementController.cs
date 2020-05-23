using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class ObjectsPlacementController : MonoBehaviour
{
    private AvailableObjectsController AOController;
    private SpatialApplier SpatialApplier;

    [SerializeField]
    public List<GameObject> FocusGroups;
    public List<GameObject> FocusLayer;
    public List<GameObject> AdditiveLayer;
    public List<GameObject> BackgroundLayer;
    public GameObject GroundMesh;

    private Dictionary<ShotElement, GameObject> LastShotElements;

    public delegate void OnContentPrepared();
    public static event OnContentPrepared OnContentPreparedEvent;
    public static event OnContentPrepared OnStartupEndedEvent;

    public Dictionary<DescriptionTag, ShotElement> LastExecutedTagItemDict;
    void Start()
    {
        AOController = new AvailableObjectsController();
        SpatialApplier = new SpatialApplier();
        LastShotElements = new Dictionary<ShotElement, GameObject>();
        var DescriptionHandler = new ObjectsPlacementHandler();

        ObjectsPlacementHandler.OnSentenceProcessedEvent += SceneByDescriptionSetup;

        SceneDefaultContentSetup();

        OnStartupEndedEvent?.Invoke();
    }
    public void SceneDefaultContentSetup()
    {
        string defName = "Doll";
        if (FocusLayer.Contains(AvailableObjectsController.GetObject(defName))) { return; }

        var dummy = new ShotElement(defName, 1, ShotHierarchyRank.InFocus, "Idle");
        SpawnFocusedObject(dummy);
        LastExecutedTagItemDict = new Dictionary<DescriptionTag, ShotElement>()
        {
            { new DescriptionTag(0, defName, TagType.Item), dummy }
        };

    }
    private void SceneByDescriptionSetup(Dictionary<DescriptionTag, ShotElement> itemTags)
    {
        if (itemTags == null)
        {
            OnContentPreparedEvent?.Invoke();
            return;
        }
        PrepareNewScene(itemTags);
    }
    private void PrepareNewScene(Dictionary<DescriptionTag, ShotElement> tagItemDict)
    {
        PrepareScene(tagItemDict);
        OnContentPreparedEvent?.Invoke();
    }
    public void PrepareScene(Dictionary<DescriptionTag, ShotElement> tagItemDict)
    {
        var tags = tagItemDict.Keys.ToList();
        var elements = tagItemDict.Values.Where(x => x != null).ToList();
        if (tagItemDict == LastExecutedTagItemDict || elements.Count == 0)
            return;//OnContentPreparedEvent?.Invoke();
        else
            ClearScene();
        SpawnAllElements(tags, elements);

        foreach (var dictElem in tagItemDict.Keys.Where(x => x.TagType == TagType.Spatial))
        {
            MoveBySpatial(dictElem, tagItemDict);
        }

        var unparenetedGroup = LastShotElements.Where(x=>x.Value.transform.parent == null)//(x => x.Key.Layer > 1)
                                    .ToDictionary(g => g.Key,g=>g.Value)
                                    .Values
                                    .ToList();

        if (FocusGroups.Count > 1)// && FrameDescription.ParsedParts.FirstOrDefault(x => x.Text == ",") != null)
        {
            GameObject orphange = FocusGroups.Last();
            orphange.name = "Orphange";
            if (FocusGroups.Count > 1)
                orphange.transform.position = new Vector3(-10000f, -10000, -10000f);
            FocusGroups.Add(orphange);
        }
            //}
            //else if (FocusGroups.Count == 0)
            //{
            //    GameObject orphange = GroupUp(unparenetedGroup);
            //    FocusGroups.Add(orphange);
            //}



         if (FocusLayer.Count == 0 && BackgroundLayer.Count == 0)
            SceneDefaultContentSetup();
        LastExecutedTagItemDict = tagItemDict;
    }

    private void SpawnAllElements(List<DescriptionTag> tags, List<ShotElement> elements)
    {
        int maxLayer = elements.Max(x => x.Layer);
        for (int i = 0; i <= maxLayer; i++)
        {
            var layerElements = elements.Where(x => x.Layer == i).ToList();
            layerElements = layerElements.OrderBy(x => (int)x.Rank).ToList();
            var Layer = new List<GameObject>();
            foreach (var element in layerElements)
            {
                switch (element.Rank)
                {
                    case ShotHierarchyRank.InFocus:
                        Layer.Add(SpawnFocusedObject(element));
                        break;
                    case ShotHierarchyRank.Addition:
                        Layer.Add(SpawnAddition(element, tags));
                        break;
                    case ShotHierarchyRank.Background:
                        break;
                }
            }

            GameObject orphange = GroupUp(Layer);
            FocusGroups.Add(orphange);

        }
    }

    private void ClearScene()
    {
        if (FocusLayer.Count != 0)
        {
            FocusLayer.ForEach(x => Destroy(x));
        }
        if (AdditiveLayer.Count != 0)
        {
            AdditiveLayer.ForEach(x => Destroy(x));
        }
        if (BackgroundLayer.Count != 0)
        {
            BackgroundLayer.ForEach(x => Destroy(x));
        }
        if (FocusGroups.Count != 0)
        {
            FocusGroups.ForEach(x => Destroy(x));
        }

        FocusLayer = new List<GameObject>();
        AdditiveLayer = new List<GameObject>();
        BackgroundLayer = new List<GameObject>();
        LastExecutedTagItemDict = new Dictionary<DescriptionTag, ShotElement>();
        LastShotElements = new Dictionary<ShotElement, GameObject>();
        FocusGroups = new List<GameObject>(); 
    }

    private GameObject SpawnFocusedObject(ShotElement element)
    {
        if (FocusLayer == null) { FocusLayer = new List<GameObject>(); }
        var obj = AvailableObjectsController.GetObject(element.PropName);

        if (FocusLayer.FirstOrDefault(x => x.name == obj.name) == null) //&& ReferenceEquals( obj, FocusLayer.Select( x => x.name == obj.name) )
        {
            var sceneGO = Instantiate(obj);
            sceneGO.SetActive(true);
            if (sceneGO.GetComponent<SceneObject>().CurrentState != element.State)
                sceneGO.GetComponent<SceneObject>().SetStateByName(element.State);

            Vector3 newObjectPos = Vector3.zero;
            newObjectPos.x -= FocusLayer.Count == 0 ? 0f : (FocusLayer[FocusLayer.Count - 1].GetComponent<SceneObject>().Bounds.size.x / 2
                    + sceneGO.GetComponent<SceneObject>().Bounds.size.x / 2) /2;

            sceneGO.transform.position = newObjectPos;//TODO: not one but calculated shit
            sceneGO.name = obj.name;
            LastShotElements.Add(element, sceneGO);
            FocusLayer.Add(sceneGO);
            return sceneGO;
        }
        return null;
    }
    private void ModifySceneObject(ShotElement oldelement, ShotElement newelement)
    {
        var pointer = LastShotElements[oldelement];
        LastShotElements.Remove(oldelement);
        LastShotElements.Add(newelement, pointer);
        if (oldelement.State != newelement.State)
            pointer.GetComponent<SceneObject>().SetStateByName(newelement.State);
        if (oldelement.Rank != newelement.Rank) { }//TODO:
        if (oldelement.Layer != newelement.Layer) { }//TODO:

    }
    private GameObject SpawnAddition(ShotElement element, List<DescriptionTag> tags)
    {
        if (AdditiveLayer == null) { AdditiveLayer = new List<GameObject>(); }

        var obj = AvailableObjectsController.GetObject(element.PropName);
        var sceneGO = Instantiate(obj);
        if (element.State != null)
            sceneGO.GetComponent<SceneObject>().SetStateByName(element.State);
        AdditiveLayer.Add(sceneGO);
        LastShotElements.Add(element, sceneGO);

        return sceneGO;
    }
    private void BackgroundSpawn(ShotElement element)
    {

    }

    private void MoveBySpatial(DescriptionTag spatialTag, Dictionary<DescriptionTag, ShotElement> tagItemDict)
    {
        var group = this.SpatialApplier.Apply(spatialTag, ref LastShotElements, tagItemDict);
        FocusGroups.Add(GroupUp(group));
    }

    private void ApplyState(ShotElement element, GameObject objectOnScene)
    {

    }

    private GameObject GroupUp(List<GameObject> children)
    {
        var parent = new GameObject();
        Vector3 pos = GroupAveragePos(children);
        parent.transform.position = pos;
        foreach(var child in children)
        {
            child.transform.parent = parent.transform;
        }

        return parent;
    }
    public static Bounds GroupBounds(List<GameObject> group)
    {
        Bounds result = new Bounds();
        foreach (var go in group)
        {
            result.Encapsulate(go.transform.position);
        }
        return result;
    }
    public static Vector3 GroupAveragePos(List<GameObject> group)
    {
        if (group.Count == 0)
            return Vector3.zero;

        float x = 0f;
        float y = 0f;
        float z = 0f;

        foreach (var go in group)
        {
            x += go.transform.position.x;
            y += go.transform.position.y;
            z += go.transform.position.z;
        }
        int count = group.Count;
        return new Vector3(x / count,
                           y / count,
                           z / count);
    }
    public static Vector3 GroupAverageDirection(List<GameObject> group)
    {
        if (group.Count == 0)
            return Vector3.zero;

        Vector3 avg = Vector3.zero;
        foreach (var go in group)
        {
            avg += go.transform.forward;
        }
        return (avg / group.Count).normalized;
    }
    public static Quaternion RotationToDirection(Vector3 origin, Vector3 destination)
    {
        var direction = (destination - origin).normalized;
        var lookRotation = Quaternion.LookRotation(direction);
        return lookRotation;
    }
    
    
    //find the vector pointing from our position to the target
    
    
}
