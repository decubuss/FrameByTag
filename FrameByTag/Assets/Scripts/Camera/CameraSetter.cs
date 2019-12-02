using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private void SetCameraPosition()
    {
        Vector3 LeftBorderPoint = Vector3.zero;
        Vector3 RightBorderPoint = Vector3.zero;
        Vector3 CenterOfFrame = Vector3.zero;
        
        CurrentCamera.transform.position = Calculate(out CenterOfFrame);
        //CurrentCamera.transform.rotation = Quaternion.Euler(CalculateCameraRotation(CenterOfFrame).eulerAngles);
        CurrentCamera.transform.rotation = CalculateCameraRotation(CenterOfFrame);
    }

        //TODO: make it possible to actually find first and last bc object are not always watch forward in camera view
        //TODO: find a way get start and end of vector on which camera looks
    

    private Vector3 Calculate(out Vector3 CenterOfFrame)
    {
        CenterOfFrame = CalculateCenterOfFrame();
        Vector3 PerpendicularDirection = Vector3.zero;
        var SpringArmLength = CalculateSpringArmLength(CenterOfFrame,out PerpendicularDirection);
        Debug.Log(PerpendicularDirection);
        return CenterOfFrame + (SpringArmLength * PerpendicularDirection);
    }

    /// <summary>
    /// Finds a middle between the most left and right objects,
    /// the Y of it is half of the biggest height along objects
    /// </summary>
    /// <returns>
    /// Returns a Vector3 position 
    /// </returns>
    private Vector3 CalculateCenterOfFrame()
    {
        if (FocusedObjects.Length > 1)
        {
            

            Vector3 ResultPoint = FocusedObjects[0].transform.position + ((FocusedObjects[FocusedObjects.Length - 1].transform.position - FocusedObjects[0].transform.position) / 2);
            ResultPoint.y = FocusedObjects.OrderByDescending(x => x.GetComponent<MeshFilter>().mesh.bounds.size.y)
                                          .First().GetComponent<MeshFilter>().mesh.bounds.size.y / 2;
            return ResultPoint;
        }
        else
        {
            Vector3 ResultPoint = FocusedObjects[0].transform.position;
            ResultPoint.y = FocusedObjects[0].GetComponent<MeshFilter>().mesh.bounds.size.y / 2;
            return ResultPoint;
        }
    }
    private float CalculateSpringArmLength(Vector3 CenterOfFrame, out Vector3 PerpendicularDirection)
    {
        var TopPoint = CenterOfFrame;
        TopPoint.y = CenterOfFrame.y * 2;
        var BottomPoint = CenterOfFrame;
        BottomPoint.y = 0;

        var SpringArmLength = 0f;
        if (FocusedObjects.Length > 1)
        {
            PerpendicularDirection = Vector3.Cross(FocusedObjects[0].transform.position - FocusedObjects[FocusedObjects.Length - 1].transform.position, Vector3.up);

            float maxHeight = 0;
            foreach (GameObject FocusedObject in FocusedObjects)
            {
                if (maxHeight < GetObjectHeight(FocusedObject))
                    maxHeight = GetObjectHeight(FocusedObject);
            }
            float Width = Vector3.Distance(FocusedObjects[0].transform.position, FocusedObjects[FocusedObjects.Length - 1].transform.position);

            if(maxHeight > Width)
            {
                SpringArmLength = (float)((Vector3.Distance(TopPoint, BottomPoint) / 2) / 0.4);
            }
            else
            {
                var ObjectsWidth = FocusedObjects[0].GetComponent<MeshFilter>().mesh.bounds.extents.y + FocusedObjects[FocusedObjects.Length - 1].GetComponent<MeshFilter>().mesh.bounds.extents.y;
                SpringArmLength = (float)((Vector3.Distance(FocusedObjects[0].transform.position, FocusedObjects[FocusedObjects.Length - 1].transform.position) + ObjectsWidth / 2) / Mathf.Tan(45));
            }

        }
        else
        {
            PerpendicularDirection = FocusedObjects[0].transform.forward;//Vector3.Cross(FocusedObjects[0].transform.up - CenterOfFrame, FocusedObjects[0].transform.right - CenterOfFrame);

            var VerticalAngle = CurrentCamera.fieldOfView;//get vertical field of view
            SpringArmLength = (float)((Vector3.Distance(TopPoint, BottomPoint) / 2) / 0.4);//Mathf.Tan(VerticalAngle / 2);
        }

        PerpendicularDirection.Normalize();
        return SpringArmLength;
    }
    private Quaternion CalculateCameraRotation(Vector3 CenterOfFrame)
    {
        var ResultRotation = Quaternion.LookRotation(CenterOfFrame - CurrentCamera.transform.position);
        ResultRotation = new Quaternion(0, ResultRotation.y, 0, ResultRotation.w);
        return ResultRotation;
    }
    private float GetObjectHeight(GameObject GObject)
    {
        return 2 * GObject.GetComponent<MeshFilter>().mesh.bounds.extents.y;

    }

    private float GetObjectWidth(GameObject GObject)
    {
        return 2 * GObject.GetComponent<MeshFilter>().mesh.bounds.extents.x;
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




    


    public void UpdateCameraTransform(/*Camera enum value*/)
    {
        //TODO: make switch which takes enum value and focus object, makes new shot variation
    }

    //TODO: make functions for each shot variant

    /// <summary>
    /// Take an extremely close shot of an object's detail/action
    /// </summary>
    private void VeryyCloseShot()
    {
        //get object's bone
        //get object actual size
        //set camera new location depend on object's actual size 
        //set camera rotation to look at chosen bone
        Debug.Log("Extremely Close shot");
    }

    /// <summary>
    /// Focus on a half of an object, mobility doesnt matter, but focus on object action
    /// </summary>
    private void MediumShot()
    {
        float AverageHeight = 0;
        Vector3 AverageLocation = Vector3.zero;
        foreach (GameObject FocusedObject in FocusedObjects)
        {
            if (FocusedObject.GetComponent<MeshFilter>().mesh == null) { return; }
            AverageHeight += FocusedObject.GetComponent<MeshFilter>().mesh.bounds.size.y;
            AverageLocation += FocusedObject.transform.position;
        }

        //get focused obeject bounding box split in 2
        //define their center
        //get object actual size
        //set camera new location depend on object's actual size
        //focus on highest center/part of bounding box 

        Debug.Log("Medium shot");
    }

    /// <summary>
    /// Focus on fully visible object and a bit of nevironment is shown
    /// </summary>
    private void LongShot()
    {
        Debug.Log("Long shot");
    }

    /// <summary>
    /// Focus on environment, where object is contatined. Object is barely visible
    /// </summary>
    private void ExtremelyLongShot()
    {
        //get object actual size
        //find distance between object and camera, so that environment could fit into camera view 
        Debug.Log("Extremely Long shot");

    }
    //TODO: is it a unique setter for each camera? or is it one object that manages them all (it will be handy to have unique)?
    //but if it is unique, that means these objects gotta be created by someone 

}
