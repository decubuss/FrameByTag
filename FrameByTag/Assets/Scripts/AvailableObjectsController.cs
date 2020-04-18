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

    public Dictionary<string,string> GetAlternateNames()
    {
        var result = new Dictionary<string, string>();
        foreach (var AvblObj in AvailableObjects)
        {
            var sceneObjComponent = AvblObj.GetComponent<SceneObject>();
            if (sceneObjComponent != null)
            {
                foreach (var item in sceneObjComponent.GetAlternateNames())
                {
                    result.Add(item.Key,item.Value);
                }
            }
        }

        return result;
    }
    private List<GameObject> AvailableObjectsHarvest()
    {

        //TODO: serialize already downloaded and check if anything unserialized is here
        var availableObjects = new List<GameObject>();

        //DirectoryInfo resourcesPath = new DirectoryInfo(Application.dataPath + @"\Resources");
        //FileInfo[] fileInfo = resourcesPath.GetFiles("*.prefab", SearchOption.AllDirectories);

        //foreach (FileInfo file in fileInfo)
        //{
        //    string objectToLoad = file.Name.Replace(".prefab", "");
        //    Object loadedObject = Resources.Load(objectToLoad, typeof(GameObject));
        //    availableObjects.Add((GameObject)loadedObject);
        //    //Debug.Log(loadedObject.name);
        //}


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
