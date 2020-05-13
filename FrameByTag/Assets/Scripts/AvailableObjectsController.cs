﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.IO;
using System.Linq;


public class AvailableObjectsController 
{
    public static List<GameObject> AvailableObjects = new List<GameObject>();
    private static Dictionary<string, string> AvailableNamesDict;
    public AvailableObjectsController()
    {
        AvailableObjects = AvailableObjectsHarvest();
    }
    public static Dictionary<string,string> GetAlternateNames()
    {
        if(AvailableNamesDict != null && AvailableNamesDict.Count != 0) { return AvailableNamesDict; }

        AvailableNamesDict = new Dictionary<string, string>();
        foreach (var AvblObj in AvailableObjects)
        {
            var sceneObjComponent = AvblObj.GetComponent<SceneObject>();
            if (sceneObjComponent != null)
            {
                foreach (var item in sceneObjComponent.GetAlternateNames())
                {
                    AvailableNamesDict.Add(item.Key,item.Value);
                }
            }
        }

        return AvailableNamesDict;
    }
    private List<GameObject> AvailableObjectsHarvest()
    {

        //TODO: serialize already downloaded and check if anything unserialized is here
        var availableObjects = new List<GameObject>();

        foreach(var downld in Resources.LoadAll("Prefabs"))
        {
            GameObject go = (GameObject)downld;
            if (go.GetComponent<SceneObject>())
            {
                availableObjects.Add(go);
            }
        }

        return availableObjects;
    }
    public GameObject GetObject(string name)
    {
        if (AvailableObjects.Count == 0) { Debug.LogError("No objects available smh"); return new GameObject(); }

        if (AvailableObjects.FirstOrDefault(x => x.name == name))
            return AvailableObjects.FirstOrDefault(x => x.name == name);
        else
        {
            Debug.LogError("no object with name " + name);
            return null;
        }
    }
    public SceneObject GetSceneObject(string name)
    {
        return this.GetObject(name).GetComponent<SceneObject>();
    }

    public void Add(GameObject SceneObject)
    {

        //TODO: add an object, attach SceneObject to it, set name and altranatives. Name of poses if needed
    }
    public List<string> GetObjectsNames()
    {
        List<string> result = new List<string>();
        foreach (var sceneObject in AvailableObjects)
        {
            result.AddRange(sceneObject.GetComponent<SceneObject>().AltNames);
        }

        return result;
    }
    //private void ObjectInitialization()
    //{
    //custom import:
    //get an fbx
    //if slots for animation are filled - donload them
    //get skinned mesh or mesh 
    //apply controller with states of animations if skinned
    //apply zones (boo hoo)
    //}
}
