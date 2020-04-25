using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class ObjectsPlacementController : MonoBehaviour, INameAlternatable
{

    //need to store all objects, their states attributes and etc in a way they gotta be callable
    //and here goes functions between them or functions attached to single object

    private AvailableObjectsController AOController;

    [SerializeField]
    public List<GameObject> FocusLayer;
    public List<GameObject> AdditiveLayer;
    public List<GameObject> BackgroundLayer;
    public GameObject GroundMesh;

    private Dictionary<ShotElement, GameObject> _lastShotElements;

    public Vector3 FocusTransform;

    private List<string> SpawnedObjects;

    public delegate void OnContentPrepared();
    public static event OnContentPrepared OnContentPreparedEvent;
    public static event OnContentPrepared OnStartupEndedEvent;

    public Dictionary<DescriptionTag, ShotElement> LastExecutedTagItemDict;
    void Start()
    {
        SpawnedObjects = new List<string>();
        AOController = new AvailableObjectsController();
        _lastShotElements = new Dictionary<ShotElement, GameObject>();
        var DescriptionHandler = new ObjectsPlacementHandler(AOController, this);

        //FrameDescription.OnDescriptionChangedEvent += PlacementHandle;
        ObjectsPlacementHandler.OnSentenceProcessedEvent += SceneByDescriptionSetup;

        SceneDefaultContentSetup();

        OnStartupEndedEvent?.Invoke();
    }

   
    public void SceneDefaultContentSetup()
    {
        if (FocusLayer.Contains(AOController.GetObject("Dummy"))) { return; }

        var dummy = new ShotElement("Dummy", 1, HierarchyRank.InFocus, "Idle");
        SpawnFocusedObject(dummy);
        LastExecutedTagItemDict = new Dictionary<DescriptionTag, ShotElement>() 
        { 
            { new DescriptionTag(0, "Dummy", TagType.Item), dummy } 
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

    private void PrepareNewScene(Dictionary<DescriptionTag, ShotElement> itemTags)
    {
        PrepareScene(itemTags);
        OnContentPreparedEvent?.Invoke();
    }
    public void PrepareScene(Dictionary<DescriptionTag, ShotElement> tagItemDict)
    {
        var tags = tagItemDict.Keys.ToList();
        var elements = tagItemDict.Values.Where(x => x != null).ToList();
        if (tagItemDict == LastExecutedTagItemDict)
        {
            OnContentPreparedEvent?.Invoke();
            return;
        }
        else
        {
            ClearScene();
        }

        int maxLayer = elements.Max(x => x.Layer);
        for(int i = 0; i <= maxLayer;  i++)
        {

            var layerElements = elements.Where(x => x.Layer == i).ToList();
            layerElements = layerElements.OrderBy(x => (int)x.Rank).ToList();
            foreach (var element in layerElements)
            {
                switch (element.Rank)
                {
                    case HierarchyRank.InFocus:
                        SpawnFocusedObject(element);
                        break;
                    case HierarchyRank.Addition:
                        SpawnAddition(element, tags);
                        break;
                    case HierarchyRank.Background:
                        break;
                }

                //if (_lastShotElements.Keys.FirstOrDefault(x => x.PropName == element.PropName) != null)
                //{
                //    var sceneElement = _lastShotElements.Keys.FirstOrDefault(x => x.PropName == element.PropName);
                //    ModifySceneObject(sceneElement, element);
                //}
                //else
                //{
                    
                //}
            }

        }

        foreach(var dictElem in tagItemDict.Keys.Where(x=>x.TagType==TagType.Spatial))
        {
            MoveBySpatial(dictElem, tagItemDict);
        }

        if (FocusLayer.Count == 0 && BackgroundLayer.Count == 0)
            SceneDefaultContentSetup();

        LastExecutedTagItemDict = tagItemDict;
    }
    private void ClearScene()
    {
        if (FocusLayer.Count != 0)
        {
            foreach (var FObject in FocusLayer)
            {
                Destroy(FObject);
            }
        }
        if (AdditiveLayer.Count != 0)
        {
            foreach (var addition in AdditiveLayer)
            {
                Destroy(addition);
            }
        }
        if (BackgroundLayer.Count != 0)
        {
            foreach (var back in BackgroundLayer)
            {
                Destroy(back);
            }
        }

        FocusLayer = new List<GameObject>();
        AdditiveLayer = new List<GameObject>();
        BackgroundLayer = new List<GameObject>();
        LastExecutedTagItemDict = new Dictionary<DescriptionTag, ShotElement>();
        _lastShotElements = new Dictionary<ShotElement, GameObject>();
    }

    private void SpawnFocusedObject(ShotElement element)
    {
        if (FocusLayer == null) { FocusLayer = new List<GameObject>(); }
        var obj = AOController.GetObject(element.PropName);

        if ( FocusLayer.FirstOrDefault( x => x.name == obj.name) == null) //&& ReferenceEquals( obj, FocusLayer.Select( x => x.name == obj.name) )
        {
            var sceneGO = Instantiate(obj);

            if (sceneGO.GetComponent<SceneObject>().CurrentState != element.State)
                sceneGO.GetComponent<SceneObject>().SetStateByName(element.State);

            Vector3 newObjectPos = Vector3.zero;
            newObjectPos.x -= FocusLayer.Count == 0 ? 0f : (FocusLayer[FocusLayer.Count - 1].GetComponent<SceneObject>().Bounds.size.x / 2
                    + sceneGO.GetComponent<SceneObject>().Bounds.size.x / 2);

            sceneGO.transform.position = newObjectPos;//TODO: not one but calculated shit
            sceneGO.name = obj.name;
            _lastShotElements.Add(element, sceneGO);
            FocusLayer.Add(sceneGO);
        }

    }
    private void ModifySceneObject(ShotElement oldelement, ShotElement newelement)
    {
        var pointer = _lastShotElements[oldelement];
        _lastShotElements.Remove(oldelement);
        _lastShotElements.Add(newelement, pointer);
        if(oldelement.State != newelement.State)
            pointer.GetComponent<SceneObject>().SetStateByName(newelement.State);
        if(oldelement.Rank != newelement.Rank) { }//TODO:
        if(oldelement.Layer != newelement.Layer) { }//TODO:

    }
    private void SpawnAddition(ShotElement element,List<DescriptionTag> tags)
    {
        if (AdditiveLayer == null) { AdditiveLayer = new List<GameObject>(); }
        int elementIndex = tags.FirstOrDefault(x => x.Keyword == element.PropName).Index;

        DescriptionTag relevantSpatial = tags.Where(x => x.Index < elementIndex
                                           && x.TagType == TagType.Spatial)
                                  .OrderByDescending(x => x.Index)
                                  .First();
        var obj = AOController.GetObject(element.PropName);
        var sceneGO = Instantiate(obj);
        if (element.State != null)
            sceneGO.GetComponent<SceneObject>().SetStateByName(element.State);
        Transform transform = ApplySpatial(obj, relevantSpatial);
        sceneGO.transform.position = transform.position;//TODO: not one but calculated shit
        sceneGO.transform.rotation = transform.rotation;
        AdditiveLayer.Add(sceneGO);

    }
    private void BackgroundSpawn(ShotElement element)
    {

    }

    private void MoveBySpatial(DescriptionTag spatial, Dictionary<DescriptionTag, ShotElement> tagItemDict)
    {
        var baseItem = tagItemDict.Where(x => x.Key.Index < spatial.Index && x.Value != null).First().Value;
        var basedItem = tagItemDict.Where(x => x.Key.Index > spatial.Index && x.Value != null).First().Value;
        var resultVector = SpatialManage(spatial.Keyword, AOController.GetSceneObject(baseItem.PropName), AOController.GetSceneObject(basedItem.PropName));

        _lastShotElements[basedItem].transform.position = _lastShotElements[baseItem].transform.position + resultVector;
    }
    private Transform ApplySpatial(GameObject newObj, DescriptionTag spatial)
    {
        Transform result = newObj.transform;
        var refObj = FocusLayer.Last();
        Vector3 refPoint = refObj.transform.position;
        Quaternion refQuat = refObj.transform.rotation;
        //TODO: damn bro you gotta find a group attached to it
        result.position = SpatialManage(spatial.Keyword, refObj.GetComponent<SceneObject>(), newObj.GetComponent<SceneObject>()) + refPoint;
        result.rotation = refQuat;
        return result;
    }
    private Vector3 SpatialManage(string spatial, SceneObject basedObject, SceneObject go2)
    {
        switch (spatial)
        {
            case "By":
                return -Vector3.forward * (basedObject.Bounds.size.z + go2.Bounds.size.z);
                break;
            case "Near":
                return -Vector3.forward * (basedObject.Bounds.size.z + go2.Bounds.size.z) * 2;
                break;
            case "To":
                return Vector3.forward * (basedObject.Bounds.size.z + go2.Bounds.size.y);
            default:
                return Vector3.zero;
                break;
        }
    }
    private void ApplyState(ShotElement element, GameObject objectOnScene)
    {

    }

    public Dictionary<string, string> GetAlternateNames()
    {
        var initialDict = new Dictionary<string[], string>
        {
            { new string[] { "beside", "nearby", "by", "by the" }, "By" },
            { new string[] { "at the background", "as the background", "as background" }, "Background" },
            { new string[] { "at the", "in the", "in"}, "In" },
            { new string[] { "to the", "to"}, "To" }
        };

        var result = Helper.DictBreakDown(initialDict);
        return result;
    }
    
    private void FocusLayerSpawn(string ObjectToPlace)
    {
        if(AOController == null || AOController.GetObject(ObjectToPlace) == null) { return; }

        if(FocusLayer.Count == 0)
        {
            var pointer = Instantiate(AOController.GetObject(ObjectToPlace), Vector3.zero, Quaternion.identity);
            FocusLayer.Add(pointer);
            SpawnedObjects.Add(ObjectToPlace);
        }
        else
        {
            Vector3 spawnPos = FocusLayer[FocusLayer.Count - 1].transform.position;

            

            var pointer = Instantiate(AOController.GetObject(ObjectToPlace), spawnPos, Quaternion.identity);
            FocusLayer.Add(pointer);
            SpawnedObjects.Add(ObjectToPlace);
        }
        
    }
    //generally spatial prepositions
    
    public Vector3 GetBaseDetailPosition()
    {
        if (FocusLayer.Count == 0) { Debug.Log("zero objects in focus"); return Vector3.zero; }
        //get action focus point

        Vector3 result = Vector3.zero;
        foreach (var FocusObject in FocusLayer)
        {
            result += FocusObject.transform.position;
            result = result / (float) FocusLayer.Count; 
        }

        return result;
    }
}
