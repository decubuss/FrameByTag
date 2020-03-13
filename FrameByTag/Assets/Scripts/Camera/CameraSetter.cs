using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CameraSetter : MonoBehaviour, INameAlternatable
{
    private enum ShotType
    {
        LongShot, CloseShot, MediumShot, ExtremelyLongShot, DefaultShot
    }

    private enum HorizontalThird
    {
        FirstThird, Center, LastThird, Auto
    }
    private enum VerticalAngle
    {
        BirdsEye, High, EyeLevel, Low, MouseEye
    }
    private enum HorizontalAngle
    {
        RightAngle, LeftAngle, Center, DeadCenter
    }

    //public Dictionary<string[], string> AlternateName = new Dictionary<string[], string>();



    private Camera CurrentCamera;
    private ObjectsPlacementController ObjectsController;

    private FrameAttributes Frame;

    private Vector3 RightObjectsLimit;//
    private Vector3 LeftObjectsLimit;//

    private ShotType Shot;
    private Vector3 ShotInitialPos;

    private HorizontalThird _third;
    private VerticalAngle _verticalAngle;

    // Start is called before the first frame update
    void Start()
    {
        Frame = new FrameAttributes();
        ObjectsController = FindObjectOfType<ObjectsPlacementController>();
        CurrentCamera = Camera.main;

        ObjectsPlacementController.OnStartupEndedEvent += StartupShot;

        //ObjectsPlacementController.OnSpawnedObjectsChange += UpdateCameraTransform;
        //ObjectsPlacementController.OnSpawnedObjectsChange += ShotOptionsHandle;
        FrameDescription.OnDescriptionChange += ShotOptionsHandle;
    }



    private void StartupShot()
    {
        DefaultShot();
        Shot = ShotType.DefaultShot;
        _third = HorizontalThird.Center;
        ApplyThird(_third);
    }

    private void ShotOptionsHandle(string input)
    {
        var Keywords = GetAlternateNames();
        var processedInput = input.ToLower();
        foreach (var Keyword in Keywords)
        {
            foreach(string AltName in Keyword.Key)
            {
                if (input.Contains(AltName))
                {
                    processedInput = processedInput.Replace(AltName, Keyword.Value);
                }
            }
        }

        foreach (var ShotType in Enum.GetValues(typeof(ShotType)).Cast<ShotType>())
        {
            if (processedInput.Contains(ShotType.ToString()))
                this.Shot = ShotType;//TODO: make apply for a shotype as thirds
            //this.SendMessage(AlternativeCalls.Value);
        }

        foreach (var ThirdType in Enum.GetValues(typeof(HorizontalThird)).Cast<HorizontalThird>() )
        {
            if (processedInput.Contains(ThirdType.ToString()))
                this._third = ThirdType;

        }

        //foreach (var HorizontalAngle in Enum.GetValues(typeof(HorizontalAngle)).Cast<HorizontalThird>())
        //{
        //    if (processedInput.Contains(HorizontalAngle.ToString()))
        //        _third = ThirdType;

        //}

        //foreach (var VerticalAngle in Enum.GetValues(typeof(VerticalAngle)).Cast<HorizontalThird>())
        //{
        //    if (processedInput.Contains(VerticalAngle.ToString()))
        //        _third = ThirdType;

        //}
        //define shot type here based on objects given from objects placer

        UpdateCameraTransform();
    }

    private void DefaultShot(float springArmLenCoef = 0.37f)
    {
        CurrentCamera.transform.position = CalculateCameraPosition(springArmLenCoef);
        CurrentCamera.transform.rotation = CalculateCameraRotation(Frame.CenterOfFrame);
        ShotInitialPos = CurrentCamera.transform.position;
    }

    

    private void ApplyThird(HorizontalThird third)
    {
        var thirdsGO = CurrentCamera.transform.Find("Thirds");
        Transform focusTransform = ObjectsController.FocusLayer.First().transform;

        var relativePos = CurrentCamera.transform.InverseTransformPoint(focusTransform.position);
        thirdsGO.localPosition = new Vector3(0, 0, relativePos.z);

        float scaleCoef = relativePos.z / 2;
        thirdsGO.localScale = Vector3.one * scaleCoef;

        Vector3 thirdPoint = Vector3.zero;
        switch (third)
        {
            case HorizontalThird.FirstThird:
                thirdPoint = thirdsGO.Find("FirstThird").position;
                break;
            case HorizontalThird.Center:
                thirdPoint = thirdsGO.Find("Center").position;
                break;
            case HorizontalThird.LastThird:
                thirdPoint = thirdsGO.Find("LastThird").position;
                break;
        }

        int Direction = (thirdPoint.x < focusTransform.position.x) ? 1 : -1;
        Debug.Log(Mathf.Abs(thirdPoint.x - focusTransform.position.x));
        CurrentCamera.transform.position = new Vector3(Mathf.Abs(thirdPoint.x - focusTransform.position.x), 0, 0) * Direction + ShotInitialPos;

    }
    private void ApplyHAngle(HorizontalAngle angle) { }
    private void ApplyVAngle(VerticalAngle angle)
    {
        CalcFocusedObjectsBounds();
        var lineSplitted = Vector3.Distance(LeftObjectsLimit, RightObjectsLimit) / 3;
        Vector3 CalculatedPos = Vector3.zero;
        switch (angle)
        {
            case VerticalAngle.BirdsEye:
                //AI???
                //rotate around focus group, massive quat +offset to rotation around z, which should be parallel to the line of view(Z vector of camera)
                break;
            case VerticalAngle.High:
                //rotate around focus group, recognisable +offset to default Zrotation
                //which should be parallel to the line of view(Z vector of camera)
                break;
            case VerticalAngle.EyeLevel:
                //rotate around focus group, no offest since default Zroation is eye level
                //, which should be parallel to the line of view(Z vector of camera)
                break;
            case VerticalAngle.Low:
                //rotate around focus group, recognisable -offset to default Zrotation
                //, which should be parallel to the line of view(Z vector of camera)
                break;
            case VerticalAngle.MouseEye:
                //rotate around focus group, massive -offset to default Zrotation, but keep it away from ground intersection
                //, which should be parallel to the line of view(Z vector of camera)
                break;
        }

        //CalculatedPos = (CurrentCamera.transform.position - CalculatedPos).normalized * Vector3.Distance(CurrentCamera.transform.position, CalculatedPos);

        Debug.Log(CalculatedPos);
        CurrentCamera.transform.position = CalculatedPos;
    }

    private Vector3 CalculateCameraPosition(float springArmCoef)
    {
        Frame.CenterOfFrame = CalculateCenterOfFrame();
        Vector3 PerpendicularDirection = Vector3.zero;
        var SpringArmLength = CalculateSpringArmLength(Frame.CenterOfFrame, springArmCoef);

        PerpendicularDirection = CalculatePerpendicularDirection();
        return Frame.CenterOfFrame + (SpringArmLength * PerpendicularDirection);
    }

    #region calculations
    /// <summary>
    /// Finds a middle between the most left and right objects,
    /// the Y of it is half of the biggest height along objects
    /// </summary>
    /// <returns>
    /// Returns a Vector3 position 
    /// </returns>
    private Vector3 CalculateCenterOfFrame()
    {
        var FObjects = ObjectsController.FocusLayer;
        if (FObjects.Count > 1)
        {
            Vector3 ResultPoint = FObjects[0].transform.position + ((FObjects[FObjects.Count - 1].transform.position - FObjects[0].transform.position) / 2);
            var AltResultPoint = ResultPoint;
            //ResultPoint.y = GetObjectSize(FObjects.OrderByDescending(x => GetObjectSize(x).y).First()).y / 2;
            ResultPoint.y = FObjects.OrderByDescending(x => x.GetComponent<SceneObject>().Bounds.size.y)
                                                        .First()
                                                        .GetComponent<SceneObject>()
                                                        .Bounds.size.y / 2;
            return ResultPoint;
        }
        else if (FObjects.Count == 1)
        {
            Vector3 ResultPoint = FObjects[0].transform.position;
            ResultPoint.y = FObjects[0].GetComponent<SceneObject>().Bounds.size.y / 2;
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

        var FObjects = ObjectsController.FocusLayer;
        var SpringArmLength = 0f;
        if (FObjects.Count > 1)
        {
            float maxHeight = 0;
            foreach (GameObject FocusedObject in FObjects)
            {
                if (maxHeight < FocusedObject.GetComponent<SceneObject>().Height)
                    maxHeight = FocusedObject.GetComponent<SceneObject>().Height;
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
                var ObjectsWidth = FObjects[0].GetComponent<SceneObject>().Bounds.extents.y 
                                   + FObjects[FObjects.Count - 1].GetComponent<SceneObject>().Bounds.extents.y;
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

        var FObjects = ObjectsController.FocusLayer;
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
    private float GetFObjectsWidth()
    {
        float ResultWidth = 0;
        foreach (GameObject FocusedObject in ObjectsController.FocusLayer)
        {
            ResultWidth += FocusedObject.GetComponent<SceneObject>().Width;
        }
        return ResultWidth;
    }
    private float GetFObjectsHeight()
    {
        float maxHeight = 0;
        foreach(var Object in ObjectsController.FocusLayer)//TODO: update focused objects on event
        {
            if(maxHeight < Object.GetComponent<SceneObject>().Height)
            {
                maxHeight = Object.GetComponent<SceneObject>().Height;
            }
        }
        return maxHeight;
    }
    #endregion


    private void CalcFocusedObjectsBounds()
    {
        CalcFrameBounds();

        if (Frame.LeftCorner != null && Frame.RightCorner != null)
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
        var LeftCorner = CurrentCamera.ViewportToWorldPoint(new Vector3(1, 0, CurrentCamera.nearClipPlane));
        var Rightcorner = CurrentCamera.ViewportToWorldPoint(new Vector3(0, 0, CurrentCamera.nearClipPlane));

        Ray Rightray = new Ray(CurrentCamera.transform.position, (LeftCorner - CurrentCamera.transform.position).normalized);
        Ray Leftray = new Ray(CurrentCamera.transform.position, (Rightcorner - CurrentCamera.transform.position).normalized);

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

        //put a middle dot down, then ray to center of viewtoworld point
        //now you have half-height of screen
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        //Gizmos.DrawSphere(LeftObjectsLimit, 0.1f);
        //Gizmos.DrawSphere(RightObjectsLimit, 0.1f);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawSphere(Frame.LeftCorner, 0.1f);
        //Gizmos.DrawSphere(Frame.RightCorner, 0.1f);


        //var Center=
        //Gizmos.DrawSphere()

    }

    public Dictionary<string[], string> GetAlternateNames()
    {
        var result = new Dictionary<string[], string>();
        result.Add(new string[] { "first third", "screen left"}, "FirstThird");
        result.Add(new string[] { "center", "centered"}, "Center");
        result.Add(new string[] { "last third", "screen right" }, "LastThird");
        result.Add(new string[] { "long shot", "ls", "full shot" }, "LongShot");
        result.Add(new string[] { "medium shot", "ms", "mid shot", "mediumshot" }, "MediumShot");
        result.Add(new string[] { "large shot", "open shot", "really long shot"}, "ExtremelyLongShot");
        return result;
    }

    public void UpdateCameraTransform(/*Camera enum value*/)
    {
        //TODO: make switch which takes enum value and focus object, makes new shot variation
        //TODO: get shot type by objects compos
        //but for now lets just use previous type by default

        if (Shot == ShotType.DefaultShot)
            DefaultShot();
        else
            this.SendMessage(Shot.ToString());

        ApplyThird(_third);
    }

    #region shots
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
        Shot = ShotType.CloseShot;

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

        var result = NewFrameCenter + (PerpendicularDirection * SpringArmLength);
        CurrentCamera.transform.position = result;

        Debug.Log("Medium shot");
        Shot = ShotType.MediumShot;

    }

    /// <summary>
    /// Focus on fully visible object and a bit of nevironment is shown
    /// </summary>
    public void LongShot()
    {
        DefaultShot();
        //CalcFocusedObjectsBounds();

        //ParentGO.transform.position = Vector3.Lerp(LeftObjectsLimit, RightObjectsLimit, coef);
        
        Debug.Log("Long shot");
        Shot = ShotType.LongShot;

    }

    /// <summary>
    /// Focus on environment, where object is contatined. Object is barely visible
    /// </summary>
    public void ExtremelyLongShot()
    {
        //get object actual size
        //find distance between object and camera, so that environment could fit into camera view 
        DefaultShot(0.17f);

        //after that rot is 0
        //get a screen space used by focus layer
        //keep going back until used screenspace is 3-5%
        Debug.Log("Extremely Long shot");
        Shot = ShotType.ExtremelyLongShot;

    }
    //TODO: is it a unique setter for each camera? or is it one object that manages them all (it will be handy to have unique)?
    //but if it is unique, that means these objects gotta be created by someone 
    #endregion





    
     /*private void ApplyThird(HorizontalThird third)
    {
        CalcFocusedObjectsBounds();
        var lineSplitted = Vector3.Distance(LeftObjectsLimit, RightObjectsLimit) / 3;
        Vector3 CalculatedPos = Vector3.zero;
        switch (third)
        {
            case HorizontalThird.Auto:
                //AI???
                break;
            case HorizontalThird.FirstThird:
                CalculatedPos = ShotInitialPos + CurrentCamera.transform.right * lineSplitted;
                break;
            case HorizontalThird.Center:
                CalculatedPos = ShotInitialPos;
                break;
            case HorizontalThird.LastThird:
                CalculatedPos = ShotInitialPos + (-CurrentCamera.transform.right * lineSplitted);
                break;
        }

        //CalculatedPos = (CurrentCamera.transform.position - CalculatedPos).normalized * Vector3.Distance(CurrentCamera.transform.position, CalculatedPos);

        var objWorldPos = ObjectsController.FocusLayer.First().transform.position;
        var objScreenPos = CurrentCamera.WorldToScreenPoint(objWorldPos);

        Debug.Log(objScreenPos);

        //Debug.Log(CalculatedPos);
        CurrentCamera.transform.position = CalculatedPos;
    }//store the change to undo/make it static

     */
}
