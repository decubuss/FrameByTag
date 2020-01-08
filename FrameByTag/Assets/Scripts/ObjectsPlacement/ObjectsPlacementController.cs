using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsPlacementController : MonoBehaviour
{

    //need to store all objects, their states attributes and etc in a way they gotta be callable
    //and here goes functions between them or functions attached to single object
    private List<string> AvailableObjects;

    public GameObject[] FocusedObjects;
    public GameObject[] StaticEnvironment;
    public GameObject GroundMesh;

    private List<string> PreviousGivenObjects;
    private AvailableObjectsController AOController;
    void Start()
    {
        AOController = FindObjectOfType<FrameDescription>().AOController;
    }

    public List<string> GetAllObjects()
    {
        return AvailableObjects;
    }

    public void ClearScene()
    {

    }
    
    public void UpdateScene()
    {

    }

    public void UpdateRequiredObjects(List<string> RequiredObjectsList)
    {
        if(PreviousGivenObjects == null)
        {
            PreviousGivenObjects = RequiredObjectsList;
            DefaultPlacement(RequiredObjectsList);
        }
        else
        {
            foreach (var Tag in PreviousGivenObjects)
            {
                DefaultPlacement(RequiredObjectsList);
            }
        }

        

    }

    private void DefaultPlacement(List<string> Objects)
    {
        foreach(string SceneObject in Objects)
        {
            //AOController.GetObject(SceneObject);
            Instantiate(AOController.GetObject(SceneObject), Vector3.zero, Quaternion.identity);
        }
    }
    

    
}
