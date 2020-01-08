using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.IO;
using System.Linq;


public class AvailableObjectsController : ScriptableObject, INameAlternatable
{
    public List<GameObject> AvailableObjects;

    public void Init()
    {
        AvailableObjects = AvailableObjectsHarvest();
        //TODO: this one not called at all
    }

    public void AddAvailableObject(GameObject SceneObject)
    {

        //TODO: add an object, attach SceneObject to it, set name and altranatives. Name of poses if needed
    }

    public Dictionary<string[], string> GetAlternateNames()
    {
        if(AvailableObjects.Count == 0) { Debug.Log("No available objects"); return null; }

        var Result = new Dictionary<string[], string>();
        foreach (var AvblObj in AvailableObjects)
        {
            Result.Add(AvblObj.GetComponent<SceneObject>().Keys, AvblObj.GetComponent<SceneObject>().Name);
        }
        return Result;
    }


    private List<GameObject> AvailableObjectsHarvest()
    {
        var Result = new List<GameObject>();

        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Dummies/Prefabs");
        FileInfo[] info = dir.GetFiles("*.prefab");
        foreach (FileInfo fileInfo in info)
        {
            string fullPath = fileInfo.FullName.Replace(@"\", "/");
            string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");
            GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;

            Result.Add(prefab);
        }

        return Result;
    }

    public GameObject GetObject(string Name)
    {
        return AvailableObjects.FirstOrDefault(x => x.name == Name);
    }
}
