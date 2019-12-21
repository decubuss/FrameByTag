using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ObjectsController : MonoBehaviour
{

    //need to store all objects, their states attributes and etc in a way they gotta be callable
    //and here goes functions between them or functions attached to single object
    private List<string> AvailableObjects;

    void Start()
    {
        AvailableObjectsHarvest();
    }

    void Update()
    {
        
    }

    public List<string> GetAllObjects()
    {
        return AvailableObjects;
    }

    private List<string> AvailableObjectsHarvest()
    {
        var Result = new List<string>();

        Object[] data = AssetDatabase.LoadAllAssetsAtPath("Assets/Dummies/Prefabs");
        foreach(Object Item in data)
        {
            Debug.Log(Item.name);
        }

        //var allsongs : Object[]= Resources.LoadAll("Assets/Dummies", GameObject);
        //for (var song : Object in music)
        //{
        //    PickWhatToPlay();
        //}

        return Result;
    }
}
