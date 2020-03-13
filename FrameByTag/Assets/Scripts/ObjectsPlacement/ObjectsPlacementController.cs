using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class ObjectsPlacementController : MonoBehaviour
{

    //need to store all objects, their states attributes and etc in a way they gotta be callable
    //and here goes functions between them or functions attached to single object
    private GameObject ParentGO;

    public List<GameObject> FocusLayer;
    public Vector3 FocusTransform;
    public List<GameObject> BackgroundLayer;
    public GameObject GroundMesh;

    private List<string> SpawnedObjects;

    private AvailableObjectsController AOController;

    public delegate void OnSpawnedObjectschangeDelegate();
    public static event OnSpawnedObjectschangeDelegate OnSpawnedObjectsChange;

    public delegate void OnStartupEnded();
    public static event OnStartupEnded OnStartupEndedEvent;

    void Start()
    {
        SpawnedObjects = new List<string>();
        AOController = new AvailableObjectsController();

        ParentGO = new GameObject();
        FrameDescription.OnDescriptionChange += PlacementHandle;

        SceneDefaultContentSetup();

        if (OnStartupEndedEvent != null)
            OnStartupEndedEvent();
    }

    /// <summary>
    /// Spawns default scene objects
    /// </summary>
    public void SceneDefaultContentSetup()
    {
        if (FocusLayer.Contains(AOController.GetObject("Dummy"))) { return; }

        FocusLayerSpawn("Dummy");
    }

    public void PlacementHandle(string Input)
    {
        List<string> ResultSequence = Input.Split(' ').ToList<string>();

        foreach (var AlternativeName in AOController.GetAlternateNames())
        {
            foreach (string AltName in AlternativeName.Key)
            {
                while (ResultSequence.Any(x => x == AltName) /*&& !TagSequence.Contains(AlternativeCalls.Value)*/)
                {
                    ResultSequence[ResultSequence.FindIndex(ind => ind.Equals(AltName))] = AlternativeName.Value;
                }
            }
        }
        //TODO: each function finds actual keyword
        //var ting = "";
        //foreach (var str in words)
        //{
        //    ting += str + " ";
        //}
        //Debug.Log(ting);

        UpdateBackground(ResultSequence);
        UpdateSceneObjects(ResultSequence);
        if (FocusLayer.Count == 0 && BackgroundLayer.Count == 0)
            SceneDefaultContentSetup();

        if (OnSpawnedObjectsChange != null)
            OnSpawnedObjectsChange();
    }
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

    public void ClearScene()
    {
        if(FocusLayer.Count == 0) { return; }
        foreach(var FObject in FocusLayer)
        {
            Destroy(FObject);
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

            Debug.Log(FocusLayer[FocusLayer.Count - 1].GetComponent<SceneObject>().Bounds.size.x / 2 + " + " + AOController.GetObject(ObjectToPlace).GetComponent<SceneObject>().Bounds.size.x / 2);


            var pointer = Instantiate(AOController.GetObject(ObjectToPlace), spawnPos, Quaternion.identity);
            FocusLayer.Add(pointer);
            SpawnedObjects.Add(ObjectToPlace);
        }
        
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

    public void GroupFObjects()
    {
        //ParentGO.transform.position = new Vector3(Frame.CenterOfFrame.x, 0, Frame.CenterOfFrame.z);

        foreach (var Object in FocusLayer)
        {
            Object.transform.parent = ParentGO.transform;
        }
    }
}
