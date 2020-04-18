﻿using System;
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

    public List<ShotElement> _lastExecutedElements;
    public List<DescriptionTag> _lastExecutedTags;
    void Start()
    {
        SpawnedObjects = new List<string>();
        AOController = new AvailableObjectsController();
        _lastShotElements = new Dictionary<ShotElement, GameObject>();
        var DescriptionHandler = new ObjectsPlacementHandler(AOController, this);

        //FrameDescription.OnDescriptionChangedEvent += PlacementHandle;
        ObjectsPlacementHandler.OnSentenceProcessedEvent += SceneDescriptionSetup;

        SceneDefaultContentSetup();

        OnStartupEndedEvent?.Invoke();
    }

   
    public void SceneDefaultContentSetup()
    {
        if (FocusLayer.Contains(AOController.GetObject("Dummy"))) { return; }

        SpawnFocusedObject(new ShotElement("Dummy", 1, HierarchyRank.InFocus, "Idle"));
    }
    private void SceneDescriptionSetup(List<DescriptionTag> tags, List<ShotElement> elements)
    {
        if (tags == null && elements == null)
        {
            OnContentPreparedEvent?.Invoke();
            return;
        }
        PrepareScene(tags, elements);
    }

    public void PrepareScene(List<DescriptionTag> tags,List<ShotElement> elements)
    {
        if (tags == _lastExecutedTags && elements == _lastExecutedElements )
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

            var layerElements = elements.Where(x => x.Layer == i);
            foreach (var element in layerElements)
            {
                if(_lastShotElements.Keys.FirstOrDefault(x => x.PropName == element.PropName) != null)
                {
                    var sceneElement = _lastShotElements.Keys.FirstOrDefault(x => x.PropName == element.PropName);
                    ModifySceneObject(sceneElement, element);
                }
                else
                {
                    Debug.Log(string.Format("{0} {1} {2} {3}", element.Layer, element.PropName, element.Rank, element.State));
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
                }
            }
        }

        if (FocusLayer.Count == 0 && BackgroundLayer.Count == 0)
            SceneDefaultContentSetup();

        _lastExecutedElements = elements;
        _lastExecutedTags = tags;

        OnContentPreparedEvent?.Invoke();
    }
    private void ClearScene()
    {
        if (FocusLayer.Count == 0) { return; }
        foreach (var FObject in FocusLayer)
        {
            Destroy(FObject);
        }
        FocusLayer = new List<GameObject>();
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
            sceneGO.transform.position = FocusLayer.Count == 0 ? Vector3.zero : Vector3.one;//TODO: not one but calculated shit
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
        int elementIndex = tags.FirstOrDefault(x => x.Tag == element.PropName).Index;

        DescriptionTag relevantSpatial = tags.Where(x => x.Index < elementIndex
                                           && x.Type == TagType.Spatial)
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
    private Transform ApplySpatial(GameObject newObj, DescriptionTag spatial)
    {
        Transform result = newObj.transform;
        var refObj = FocusLayer.Last();
        Vector3 refPoint = refObj.transform.position;
        Quaternion refQuat = refObj.transform.rotation;
        //TODO: damn bro you gotta find a group attached to it
        result.position = SpatialManage(spatial.Tag, refObj.GetComponent<SceneObject>(), newObj.GetComponent<SceneObject>()) + refPoint;
        result.rotation = refQuat;
        return result;
    }
    private Vector3 SpatialManage(string spatial, SceneObject go1, SceneObject go2)
    {
        switch (spatial)
        {
            case "By":
                return -Vector3.forward * (go1.Bounds.size.z + go2.Bounds.size.z);
                break;
            case "Near":
                return -Vector3.forward * (go1.Bounds.size.z + go2.Bounds.size.z) * 2;
                break;
            default:
                return Vector3.zero;
                break;
        }
    }
    private void ApplyState(ShotElement element, GameObject objectOnScene)
    {

    }
    

    #region OldKingdom
    private void UpdateBackground(List<string> TagSequence)
    {
        TagSequence.ToArray();
        foreach(var Tag in TagSequence)
        {
            if(Tag == "background" && TagSequence.IndexOf("background") >= 2)
            {
                if(TagSequence[TagSequence.IndexOf("background")-1] == "at")
                {
                    string objectname = TagSequence[TagSequence.IndexOf("background") - 2];
                    Debug.Log(objectname + "at back");

                    BackgroundLayer.Add(AOController.GetObject(objectname));
                }
                else
                {
                    Debug.Log(TagSequence[TagSequence.IndexOf("background") - 1] + "at back");//TODO: define any Available object in nearest area and attach to background
                    //background is set realtively to camera smh
                    //taking a frame center
                    //understand where is "far" if defined obj is large size

                }
            }
        }
    }
    public void UpdateScene()
    {
        if (FocusLayer.Count == 0) { return; } //temporary
        foreach (var FObject in FocusLayer)
        {
            Destroy(FObject);
        }
    }
    public void UpdateSceneObjects(List<string> RequestedObjectsList)
    {
        if(SpawnedObjects.Count == 0)
        {
            foreach(var ReqObj in RequestedObjectsList)
            {
                FocusLayerSpawn(ReqObj);
            }
        }
        else
        {
            foreach (var SpawnedObject in SpawnedObjects)
            {
                RemoveSceneObject(SpawnedObject);
            }

            foreach (var Object in RequestedObjectsList)
            {
                FocusLayerSpawn(Object);
            }

            //find a way to spawn multiple objects. take in account that there can be actions between them

            //var ting = "";
            //foreach (var str in SpawnedObjects)
            //{
            //    ting += str + " "; //maybe also + '\n' to put them on their own line.
            //}
            //Debug.Log(ting);
        }

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

            spawnPos.x = spawnPos.x - (FocusLayer[FocusLayer.Count - 1].GetComponent<SceneObject>().Bounds.size.x / 2 
                    + AOController.GetObject(ObjectToPlace).GetComponent<SceneObject>().Bounds.size.x / 2);

            var pointer = Instantiate(AOController.GetObject(ObjectToPlace), spawnPos, Quaternion.identity);
            FocusLayer.Add(pointer);
            SpawnedObjects.Add(ObjectToPlace);
        }
        
    }
    //generally spatial prepositions
    public Dictionary<string, string> GetAlternateNames()
    {
        var initialDict = new Dictionary<string[], string>
        {
            { new string[] { "beside", "nearby", "by", "by the" }, "By" },
            { new string[] { "at the background", "as the background", "as background" }, "Background" },
            { new string[] { "at the", "in the", "in"}, "In" }
        };

        var result = Helper.DictBreakDown(initialDict);
        return result;
    }
    private GameObject FindSceneObjectByName(string name)
    {
        //PrefabUtility.GetCorrespondingObjectFromSource(FObject).name == name ||
        foreach (var FObject in FocusLayer)
        {
            if (FObject.name.Replace("(Clone)","") == name)
            {
                return FObject;
            }
        }
        foreach(var SObject in BackgroundLayer)
        {
            if (SObject.name == name)
            {
                return SObject;
            }
        }

        return null;
    }
    private void RemoveSceneObject(string name)
    {
        var pointer = FindSceneObjectByName(name);
        FocusLayer.Remove(pointer);
        Destroy(pointer);
    }
    
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
    #endregion
}
