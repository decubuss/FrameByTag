using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class ObjectsController : MonoBehaviour
{

    //need to store all objects, their states attributes and etc in a way they gotta be callable
    //and here goes functions between them or functions attached to single object
    private List<string> AvailableObjects;

    public GameObject[] FocusedObjects;
    public GameObject[] StaticEnvironment;
    public GameObject GroundMesh;

    void Start()
    {
        AvailableObjectsHarvest();
    }

    public List<string> GetAllObjects()
    {
        return AvailableObjects;
    }

    private List<string> AvailableObjectsHarvest()
    {
        var Result = new List<string>();


        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Dummies/Prefabs");
        FileInfo[] info = dir.GetFiles("*.prefab");
        foreach (FileInfo fileInfo in info)
        {
            string fullPath = fileInfo.FullName.Replace(@"\", "/");
            string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");
            GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;

            //AvailableObjects.Add(prefab.name);
        }


        return Result;
    }
}
