using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CameraSetter : MonoBehaviour, INameAlternatable
{
    private Camera CurrentCamera;//

    private ObjectsPlacementController ObjectsController;//

    private FrameAttributes Frame;
    
    private GameObject ParentGO;//

    private Vector3 RightObjectsLimit;//
    private Vector3 LeftObjectsLimit;//

    // Start is called before the first frame update
    void Start()
    {
        Frame = ScriptableObject.CreateInstance<FrameAttributes>();
        ObjectsController = FindObjectOfType<ObjectsPlacementController>();
        CurrentCamera = gameObject.GetComponent<Camera>();

        ParentGO = new GameObject();
        DefaultShot();

    }

    private void DefaultShot()
    {
        //CurrentCamera.transform.position = Vector3.zero;
        //CurrentCamera.transform.rotation = Quaternion.identity;
        CurrentCamera.transform.position = CalculateCameraPosition();
        CurrentCamera.transform.rotation = CalculateCameraRotation(Frame.CenterOfFrame);
    }

    public Dictionary<string[],string> GetAlternateNames()
    {
        var result = new Dictionary<string[], string>();
        result.Add(new string[] { "long shot", "ls", "full shot" }, "LongShot");
        result.Add(new string[] { "medium shot", "ms", "mid shot", "mediumshot" }, "MediumShot");
        return result;
    }

    private Vector3 CalculateCameraPosition()
    {
        Frame.CenterOfFrame = CalculateCenterOfFrame();
        Vector3 PerpendicularDirection = Vector3.zero;
        var SpringArmLength = CalculateSpringArmLength(Frame.CenterOfFrame, 0.37f);

        PerpendicularDirection = CalculatePerpendicularDirection();
        return Frame.CenterOfFrame + (SpringArmLength * PerpendicularDirection);
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
        var FObjects = ObjectsController.FocusedObjects;
        if (FObjects.Count > 1)
        {
            Vector3 ResultPoint = FObjects[0].transform.position + ((FObjects[FObjects.Count - 1].transform.position - FObjects[0].transform.position) / 2);
            var AltResultPoint = ResultPoint;

            if(FObjects.Where(x => x.GetComponent<MeshFilter>() != null).LastOrDefault() != null )
            {
                ResultPoint.y = FObjects.Where(x => x.GetComponent<MeshFilter>() != null)
                                          .OrderByDescending(x => x.GetComponent<MeshFilter>().mesh.bounds.size.y)
                                          .First().GetComponent<MeshFilter>().mesh.bounds.size.y / 2;
            }

            if(FObjects.Where(x => x.GetComponentInChildren<SkinnedMeshRenderer>() != null).LastOrDefault() != null)
            {
                AltResultPoint.y = FObjects.Where(x => x.GetComponentInChildren<SkinnedMeshRenderer>() != null)
                                         .OrderByDescending(x => x.GetComponentInChildren<SkinnedMeshRenderer>().bounds.size.y)
                                         .First().GetComponentInChildren<SkinnedMeshRenderer>().bounds.size.y / 2;
            }
            
            if(AltResultPoint.y >= ResultPoint.y)
            {
                return AltResultPoint;
            }
            else
            {
                return ResultPoint;
            }

        }
        else if (FObjects.Count == 1)
        {
            Vector3 ResultPoint = FObjects[0].transform.position;
            if(FObjects[0].GetComponent<MeshFilter>() != null)
                ResultPoint.y = FObjects[0].GetComponent<MeshFilter>().mesh.bounds.size.y / 2;
            else
            {
                ResultPoint.y = FObjects[0].GetComponentInChildren<SkinnedMeshRenderer>().bounds.size.y / 2;
            }
            return ResultPoint;
        }
        else
        {
            return new Vector3(0, 2, 0);
        }
    }
    private float CalculateSpringArmLength(Vector3 CenterOfFrame, float GivenCoefficient)
    {
        var TopPoint = CenterOfFrame;
        TopPoint.y = CenterOfFrame.y * 2;
        var BottomPoint = CenterOfFrame;
        BottomPoint.y = 0;

        var FObjects = ObjectsController.FocusedObjects;
        var SpringArmLength = 0f;
        if (FObjects.Count > 1)
        {
            float maxHeight = 0;
            foreach (GameObject FocusedObject in FObjects)
            {
                if (maxHeight < GetObjectHeight(FocusedObject))
                    maxHeight = GetObjectHeight(FocusedObject);
            }
            float Width = Vector3.Distance(FObjects[0].transform.position, FObjects[FObjects.Count - 1].transform.position);

            if(maxHeight > Width)
            {
                {
                    SpringArmLength = (float)((Vector3.Distance(TopPoint, BottomPoint) / 2) / GivenCoefficient);
                }
            }
            else
            {
                var ObjectsWidth = GetBounds(FObjects[0]).extents.y + GetBounds(FObjects[FObjects.Count - 1]).extents.y;
                SpringArmLength = (float)((Vector3.Distance(FObjects[0].transform.position, FObjects[FObjects.Count - 1].transform.position) + ObjectsWidth / 2) / Mathf.Tan(45));
            }

        }
        else
        {
            var VerticalAngle = CurrentCamera.fieldOfView;//get vertical field of view
            SpringArmLength = (float)((Vector3.Distance(TopPoint, BottomPoint) / 2) / GivenCoefficient);//Mathf.Tan(VerticalAngle / 2);
        }

        return SpringArmLength;
    }
    private Vector3 CalculatePerpendicularDirection()
    {
        var DirectionVector = Vector3.zero;

        var FObjects = ObjectsController.FocusedObjects;
        if (FObjects.Count > 1)
        {
            DirectionVector = Vector3.Cross(FObjects[0].transform.position - FObjects[FObjects.Count - 1].transform.position, Vector3.up);
            DirectionVector.Normalize();
            return DirectionVector;
        }
        else if(FObjects.Count == 1)
        {
            DirectionVector = FObjects[0].transform.forward;
            DirectionVector.Normalize();
            return DirectionVector;
        }
        else
        {
            return Vector3.forward.normalized;
        }
    }
    private Quaternion CalculateCameraRotation(Vector3 CenterOfFrame)
    {
        var ResultRotation = Quaternion.LookRotation(CenterOfFrame - CurrentCamera.transform.position);
        ResultRotation = new Quaternion(0, ResultRotation.y, 0, ResultRotation.w);
        return ResultRotation;
    }

    private float GetObjectHeight(GameObject GObject)
    {
        if(GObject.GetComponent<MeshFilter>() != null)
        {
            return 2 * GObject.GetComponent<MeshFilter>().mesh.bounds.extents.y;
        }
        else //if(GObject.GetComponentInChildren<SkinnedMeshRenderer>() != null)
        {
            return 2 * GObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds.extents.y;
        }

    }
    private float GetObjectWidth(GameObject GObject)
    {
        if (GObject.GetComponent<MeshFilter>() != null)
        {
            return 2 * GObject.GetComponent<MeshFilter>().mesh.bounds.extents.x;
        }
        else //if(GObject.GetComponentInChildren<SkinnedMeshRenderer>() != null)
        {
            return 2 * GObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds.extents.x;
        }

        //return 2 * GObject.GetComponent<MeshFilter>().mesh.bounds.extents.x;
    }
    private float GetFObjectsWidth()
    {
        float ResultWidth = 0;
        foreach (GameObject FocusedObject in ObjectsController.FocusedObjects)
        {
            ResultWidth += GetObjectWidth(FocusedObject);
        }
        return ResultWidth;
    }
    private float GetFObjectsHeight()
    {
        float maxHeight = 0;
        foreach(var Object in ObjectsController.FocusedObjects)//TODO: update focused objects on event
        {
            if(maxHeight < GetObjectHeight(Object))
            {
                maxHeight = GetObjectHeight(Object);
            }
        }
        return maxHeight;
    }

    private void CalcFocusedObjectsBounds()
    {
        if(Frame.LeftCorner != null && Frame.RightCorner != null)
        {
            float ObjectsVariationsWidth = Vector3.Distance(Frame.LeftCorner, Frame.RightCorner) - GetFObjectsWidth();
            Vector3 WidthCenter = new Vector3(Frame.CenterOfFrame.x, 0, Frame.CenterOfFrame.z);
            RightObjectsLimit = WidthCenter + (Frame.RightCorner - WidthCenter).normalized * (ObjectsVariationsWidth / 2);
            LeftObjectsLimit = WidthCenter + (Frame.LeftCorner - WidthCenter).normalized * (ObjectsVariationsWidth / 2);
        }
        //else but not neccessary
    }
    private void CalcFrameBounds()
    {
        var TheDot = CurrentCamera.ViewportToWorldPoint(new Vector3(1, 0, CurrentCamera.nearClipPlane));
        var TheSot = CurrentCamera.ViewportToWorldPoint(new Vector3(0, 0, CurrentCamera.nearClipPlane));

        Ray Rightray = new Ray(CurrentCamera.transform.position, (TheDot - CurrentCamera.transform.position).normalized);
        Ray Leftray = new Ray(CurrentCamera.transform.position, (TheSot - CurrentCamera.transform.position).normalized);

        RaycastHit Rhit = new RaycastHit();
        RaycastHit Lhit = new RaycastHit();

        if (Physics.Raycast(Rightray, out Rhit, 100000))
        {
            Frame.RightCorner = Rhit.point;
        }
        if (Physics.Raycast(Leftray, out Lhit, 100000))
        {
            Frame.LeftCorner = Lhit.point;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if(ParentGO != null)
            Gizmos.DrawSphere(ParentGO.transform.position, 0.1f);
        Gizmos.DrawSphere(LeftObjectsLimit, 0.1f);
        Gizmos.DrawSphere(RightObjectsLimit, 0.1f);
        Gizmos.color = Color.blue;
        //Gizmos.DrawSphere(Frame.LeftCorner, 0.1f);
        //Gizmos.DrawSphere(Frame.RightCorner, 0.1f);

    }

    private void GroupFObjects()
    {
        ParentGO.transform.position = new Vector3(Frame.CenterOfFrame.x, 0, Frame.CenterOfFrame.z);

        foreach (var Object in ObjectsController.FocusedObjects)
        {
            Object.transform.parent = ParentGO.transform;
        }
    }

    private Bounds GetBounds(GameObject go)
    {
        if (go.GetComponent<MeshFilter>() != null)
        {
            return go.GetComponent<MeshFilter>().mesh.bounds;
        }
        else 
        {
            return  go.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
        }
    }
    




    public void UpdateCameraTransform(/*Camera enum value*/)
    {
        //TODO: make switch which takes enum value and focus object, makes new shot variation
    }

    //TODO: make functions for each shot variant

    /// <summary>
    /// Take an extremely close shot of an object's detail/action
    /// </summary>
    public void VeryCloseShot()
    {
        DefaultShot();
        //get object's bone
        //get object actual size
        //set camera new location depend on object's actual size 
        //set camera rotation to look at chosen bone
        Debug.Log("Extremely Close shot");
    }

    /// <summary>
    /// Focus on a half of an object, mobility doesnt matter, but focus on object action
    /// </summary>
    public void MediumShot()
    {
        DefaultShot();
        float CameraNewHeight = (float)(GetFObjectsHeight() * 0.75);

        Vector3 NewFrameCenter = new Vector3(Frame.CenterOfFrame.x, CameraNewHeight, Frame.CenterOfFrame.z);
        var PerpendicularDirection = CalculatePerpendicularDirection();
        var SpringArmLength = CalculateSpringArmLength(NewFrameCenter, 0.8f);

        CurrentCamera.transform.position = NewFrameCenter + (PerpendicularDirection * SpringArmLength);

        Debug.Log("Medium shot");
    }

    /// <summary>
    /// Focus on fully visible object and a bit of nevironment is shown
    /// </summary>
    public void LongShot()
    {
        DefaultShot();
        CalcFrameBounds();
        CalcFocusedObjectsBounds();
        GroupFObjects();

        //make it random and test it
        var coef = UnityEngine.Random.Range(0f,1f);
        
        ParentGO.transform.position = Vector3.Lerp(LeftObjectsLimit, RightObjectsLimit, coef);
        
        Debug.Log("Long shot");
    }

    /// <summary>
    /// Focus on environment, where object is contatined. Object is barely visible
    /// </summary>
    public void ExtremelyLongShot()
    {
        //get object actual size
        //find distance between object and camera, so that environment could fit into camera view 
        Debug.Log("Extremely Long shot");

    }
    //TODO: is it a unique setter for each camera? or is it one object that manages them all (it will be handy to have unique)?
    //but if it is unique, that means these objects gotta be created by someone 

}
