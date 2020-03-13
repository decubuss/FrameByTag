using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class FbxFromFile : MonoBehaviour
{
    string fbxPath = string.Empty;
    string error = string.Empty;
    GameObject loadedObject;

    //void OnGUI()
    //{
    //    objPath = GUI.TextField(new Rect(0, 0, 256, 32), objPath);

    //    GUI.Label(new Rect(0, 0, 256, 32), "Obj Path:");
    //    if (GUI.Button(new Rect(256, 32, 64, 32), "Load File"))
    //    {
    //        //file path
    //        if (!File.Exists(objPath))
    //        {
    //            error = "File doesn't exist.";
    //        }
    //        else
    //        {
    //            if (loadedObject != null)
    //                Destroy(loadedObject);
    //            loadedObject = new OBJLoader().Load(objPath);
    //            error = string.Empty;
    //        }
    //    }

    //    if (!string.IsNullOrWhiteSpace(error))
    //    {
    //        GUI.color = Color.red;
    //        GUI.Box(new Rect(0, 64, 256 + 64, 32), error);
    //        GUI.color = Color.white;
    //    }
    //}

    public void OnPathEntered()
    {
        fbxPath = @"C:\Users\xiaom\Downloads\cube.fbx";//gameObject.GetComponent<InputField>().text;
        if (!File.Exists(fbxPath))
        {
            Debug.Log("File doesn't exist");
        }
        else
        {
            Debug.Log("File does exist");
            var loader = new FBXLoader().Load(fbxPath);//C:\Users\xiaom\Downloads\free3DmodelFBX.fbx
        }

        //check file collissions and then load it
    }

}
