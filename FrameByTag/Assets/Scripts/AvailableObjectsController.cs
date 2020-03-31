using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.IO;
using System.Linq;


public class AvailableObjectsController : INameAlternatable
{
    public List<GameObject> AvailableObjects = new List<GameObject>();

    public AvailableObjectsController()
    {
        AvailableObjects = AvailableObjectsHarvest();

    }
    

    public void Add(GameObject SceneObject)
    {

        //TODO: add an object, attach SceneObject to it, set name and altranatives. Name of poses if needed
    }

    public Dictionary<string[], string> GetAlternateNames()
    {
        if(AvailableObjects.Count == 0) { Debug.Log("No available objects"); return null; }

        var Result = new Dictionary<string[], string>();
        foreach (var AvblObj in AvailableObjects)
        {
            if(AvblObj.GetComponent<SceneObject>() != null)
                Result.Add(AvblObj.GetComponent<SceneObject>().Keys, AvblObj.GetComponent<SceneObject>().Name);
        }
        return Result;

        
    }
    public Dictionary<string,string> GetAltNames()
    {
        var result = new Dictionary<string, string>();
        foreach (var AvblObj in AvailableObjects)
        {
            var sceneObjComponent = AvblObj.GetComponent<SceneObject>();
            if (sceneObjComponent != null)
            {
                foreach (var key in sceneObjComponent.Keys)
                {
                    result.Add(key, sceneObjComponent.Name);
                }
            }
            //foreach(var key in AvblObj.)
        }

        return result;
    }
    private List<GameObject> AvailableObjectsHarvest()
    {

        //TODO: serialize already downloaded and check if anything unserialized is here
        var Result = new List<GameObject>();

        //foreach (var ImportedObject in Resources.LoadAll("Dummy", typeof(GameObject)))
        //{
        //    Result.Add((GameObject)ImportedObject);
        //}

        DirectoryInfo resourcesPath = new DirectoryInfo(Application.dataPath + @"\Resources");
        FileInfo[] fileInfo = resourcesPath.GetFiles("*.prefab", SearchOption.AllDirectories);

        foreach (FileInfo file in fileInfo)
        {
            string objectToLoad = file.Name.Replace(".prefab", "");
            Object loadedObject = Resources.Load(objectToLoad, typeof(GameObject));
            Result.Add((GameObject)loadedObject);
            Debug.Log(loadedObject.name);
        }

        return Result;
    }

    public GameObject GetObject(string Name)
    {
        if (AvailableObjects.Count == 0) { Debug.LogError("No objects available smh");  return new GameObject(); }

        if (AvailableObjects.FirstOrDefault(x => x.name == Name))
            return AvailableObjects.FirstOrDefault(x => x.name == Name);
        else
            return null;
    }

    private void ObjectInitialization()
    {
        //custom import:
        //get an fbx
        //if slots for animation are filled - donload them
        //get skinned mesh or mesh 
        //apply controller with states of animations if skinned
        //apply zones (boo hoo)
    }
    public List<string> GetObjectsNames()
    {
        List<string> result = new List<string>();
        foreach (var sceneObject in AvailableObjects)
        {
            result.AddRange(sceneObject.GetComponent<SceneObject>().Keys);
        }

        return result;
    }

}
