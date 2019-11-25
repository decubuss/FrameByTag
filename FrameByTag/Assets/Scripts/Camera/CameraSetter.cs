using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetter : MonoBehaviour
{
    public Camera CurrentCamera;
    public GameObject[] FocusedObjects;

    // Start is called before the first frame update
    void Start()
    {
        CurrentCamera = gameObject.GetComponent<Camera>();
        SetCameraPosition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCameraTransform(/*Camera enum value*/)
    {
        //TODO: make switch which takes enum value and focus object, makes new shot variation
    }

    //TODO: make functions for each shot variant
    
    /// <summary>
    /// Take an extremely close shot of a object detail/action
    /// </summary>
    private void Focus_ExtremelyCloseShot(/*The object, the bone of object to focus on*/)
    {
        //get object's bone
        //get object actual size
        //set camera new location depend on object's actual size 
        //set camera rotation to look at chosen bone
    }

    /// <summary>
    /// Focus on a half of an object, mobility doesnt matter, but focus on object action
    /// </summary>
    private void Focus_MediumShot(/*The object*/)
    {
        float AverageHeight = 0;
        Vector3 AverageLocation = Vector3.zero;
        foreach (GameObject FocusedObject in FocusedObjects)
        {
            if(FocusedObject.GetComponent<MeshFilter>().mesh == null) { return; }
            AverageHeight += FocusedObject.GetComponent<MeshFilter>().mesh.bounds.size.y;
            AverageLocation += FocusedObject.transform.position;
        }
        
        //get focused obeject bounding box split in 2
        //define their center
        //get object actual size
        //set camera new location depend on object's actual size
        //focus on highest center/part of bounding box 
    }

    /// <summary>
    /// Focus on fully visible object and a bit of nevironment is shown
    /// </summary>
    private void Focus_Long(/*The object*/)
    {
        //
    }

    /// <summary>
    /// Focus on environment, where object is contatined. Object is barely visible
    /// </summary>
    private void Focus_ExtremelyLong(/*The object*/)
    {
        //get object actual size
        //find distance between object and camera, so that environment could fit into camera view 


    }
    //TODO: is it a unique setter for each camera? or is it one object that manages them all (it will be handy to have unique)?
    //but if it is unique, that means these objects gotta be created by someone 

    private Vector3 CalculateCameraPosition(out Vector3 LeftBorderPoint,out Vector3 RightBorderPoint/**/)
    {
        LeftBorderPoint = Vector3.zero;
        RightBorderPoint = Vector3.zero;
        Vector3 PerpendicularDirection = Vector3.zero;
        var FocusedObjectsCount = FocusedObjects.Length - 1;


        //camera aspect
        if (FocusedObjects.Length > 1)
        {
            var LeftToRightDirection = (FocusedObjects[FocusedObjectsCount].transform.position - FocusedObjects[0].transform.position);
            LeftToRightDirection.Normalize();
            LeftBorderPoint = FocusedObjects[0].transform.position - (LeftToRightDirection * FocusedObjects[0].GetComponent<MeshRenderer>().bounds.extents.magnitude);
            RightBorderPoint = FocusedObjects[FocusedObjectsCount].transform.position + (LeftToRightDirection * FocusedObjects[FocusedObjectsCount].GetComponent<MeshRenderer>().bounds.extents.magnitude);
            PerpendicularDirection = Vector3.Cross(LeftToRightDirection, Vector3.up);
        }
        else
        {
            LeftBorderPoint = FocusedObjects[0].transform.position - (FocusedObjects[0].GetComponent<MeshFilter>().mesh.bounds.extents.x * FocusedObjects[0].transform.right);
            RightBorderPoint = FocusedObjects[0].transform.position + (FocusedObjects[0].GetComponent<MeshFilter>().mesh.bounds.extents.x * FocusedObjects[0].transform.right);
            PerpendicularDirection = FocusedObjects[0].transform.forward;
            //TODO: fix problem when object is not wide enugh / use height as well

        }

        float maxHeight = 0;
        foreach (GameObject FocusedObject in FocusedObjects)
        {
            if (maxHeight < GetObjectHeight(FocusedObject))
                maxHeight = GetObjectHeight(FocusedObject);
        }

        Vector3 CameraPosition = Vector3.zero;
        if (maxHeight > Vector3.Distance(new Vector3(LeftBorderPoint.x, 0, LeftBorderPoint.z), new Vector3(RightBorderPoint.x, 0, RightBorderPoint.z)))
        {
            var VerticalAngle = CurrentCamera.fieldOfView / CurrentCamera.aspect;

        }
        else
        {
            var Width = Vector3.Distance(new Vector3(LeftBorderPoint.x, 0, LeftBorderPoint.z), new Vector3(RightBorderPoint.x, 0, RightBorderPoint.z));
            var SpringArmLength = ((Width / 2) / Mathf.Tan(CurrentCamera.fieldOfView / 2));//works only if 90 degrees fov, find another solution or lock view angle

            CameraPosition = (LeftBorderPoint + (RightBorderPoint - LeftBorderPoint) / 2) + (PerpendicularDirection * SpringArmLength);
        }
       

        return CameraPosition;

        //TODO: make it possible to actually find first and last bc object are not always watch forward in camera view
        //TODO: find a way get start and end of vector on which camera looks
    }

    private Quaternion CalculateCameraRotation(Vector3 LeftBorderPoint, Vector3 RightBorderPoint)
    {
        Debug.Log(LeftBorderPoint + (RightBorderPoint - LeftBorderPoint) / 2);
        return Quaternion.LookRotation((LeftBorderPoint + ((RightBorderPoint - LeftBorderPoint) / 2)), CurrentCamera.transform.up);
        //something wrong
    }

    private float GetObjectWidth(GameObject GObject)
    {
        return 2 * GObject.GetComponent<MeshFilter>().mesh.bounds.extents.x;
    }
    private float GetObjectHeight(GameObject GObject)
    {
        return 2 * GObject.GetComponent<MeshFilter>().mesh.bounds.extents.y;

    }
    private float GetFObjectsWidth()
    {
        float ResultWidth = 0;

        foreach (GameObject FocusedObject in FocusedObjects)
        {
            ResultWidth += GetObjectWidth(FocusedObject);
        }
        return ResultWidth;
    }


    private void SetCameraPosition()
    {
        Vector3 LeftBorderPoint = Vector3.zero;
        Vector3 RightBorderPoint = Vector3.zero;

        CurrentCamera.transform.position = CalculateCameraPosition(out LeftBorderPoint, out RightBorderPoint);
        CurrentCamera.transform.rotation = CalculateCameraRotation(LeftBorderPoint, RightBorderPoint);

        //CurrentCamera.transform.rotation = new Quaternion(CurrentCamera.transform.rotation.x, CurrentCamera.transform.rotation.y, 0, CurrentCamera.transform.rotation.w);
    }

   
}
