using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CameraSetter : MonoBehaviour
{
    //public static CameraSetter instance;

    public enum ShotType
    {
        LongShot, CloseShot, MediumShot, ExtremelyLongShot, DefaultShot
    }
    public enum HorizontalThird
    {
        FirstThird, Center, LastThird, Auto
    }
    public enum VerticalAngle
    {
        BirdsEye, High, EyeLevel, Low, MouseEye
    }
    public enum HorizontalAngle
    {
        RightAngle, LeftAngle, Front, DeadFront
    }

    //public Dictionary<string[], string> AlternateName = new Dictionary<string[], string>();
    private BaseDetail _baseDetail;

    [HideInInspector]
    public Camera CurrentCamera;
    private ObjectsPlacementController OPController;
    private CameraParametersHandler CPHandler;
    private FrameAttributes Frame;

    private Vector3 RightObjectsLimit;//
    private Vector3 LeftObjectsLimit;//

    private Vector3 ShotInitialPos;

    [SerializeField]
    private HorizontalThird _third;
    [SerializeField]
    private HorizontalAngle _horizontalAngle;
    [SerializeField]
    private VerticalAngle _verticalAngle;
    [SerializeField]
    private ShotType _shot;

    // Start is called before the first frame update
    void Start()
    {
        Frame = new FrameAttributes();
        OPController = FindObjectOfType<ObjectsPlacementController>();
        CPHandler = new CameraParametersHandler(this);
        CurrentCamera = Camera.main;
        _baseDetail = new GameObject("BaseDetail").AddComponent<BaseDetail>();


        ObjectsPlacementController.OnStartupEndedEvent += StartupShot;

        //ObjectsPlacementController.OnSpawnedObjectsChange += ShotOptionsHandle;
        FrameDescription.OnDescriptionChangedEvent += CameraSetReady;
        //CameraParametersHandler.OnParametersCollectedEvent += UpdateCameraTransform;
    }
    public ShotParameters GetShotParameters()
    {
        return new ShotParameters(_shot, _horizontalAngle, _verticalAngle, _third);
    }
    private void StartupShot()
    {
        _shot = ShotType.DefaultShot;
        _third = HorizontalThird.Center;
        _verticalAngle = VerticalAngle.EyeLevel;
        _horizontalAngle = HorizontalAngle.Front;

        //BuildNewShot();
        ExecuteParameters(CPHandler.DefaultParams);
        ObjectsPlacementController.OnStartupEndedEvent -= StartupShot;
    }

    private void CameraSetReady(string input)
    {
        ObjectsPlacementController.OnContentPreparedEvent += BuildNewShot;
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
        Transform focusTransform = OPController.FocusLayer.First().transform;

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
        //Debug.Log(Mathf.Abs(thirdPoint.x - focusTransform.position.x));
        _third = third;
        CurrentCamera.transform.position = new Vector3(Mathf.Abs(thirdPoint.x - focusTransform.position.x), 0, 0) * Direction + ShotInitialPos;

    }
    private void ApplyHAngle(HorizontalAngle angle)
    {
        Vector3 calculatedRot = _baseDetail.transform.eulerAngles;
        Vector3 initalRot = _baseDetail.transform.eulerAngles;

        var focus = new Vector3(_baseDetail.transform.forward.x, 0, _baseDetail.transform.forward.z);//_baseDetail.transform.forward.z

        var camera = _baseDetail.transform.position - CurrentCamera.transform.position ;
        camera = new Vector3(camera.x, 0, camera.z);

        float Angle = Vector3.Angle(focus, camera);
        Vector3 cross = Vector3.Cross(focus, camera);
        Angle = cross.y < 0 ? -Angle : Angle;

        string debugline = "Horizontal angle: ";
        switch (angle)
        {
            case HorizontalAngle.Front:
                calculatedRot = new Vector3(initalRot.x, 180 - Angle, initalRot.z);//_baseDetail.transform.rotation.x

                debugline += angle.ToString();
                
                break;
            case HorizontalAngle.DeadFront:
                calculatedRot.y = Quaternion.Euler(-30f, 0, 0).x;
                debugline += angle.ToString();
                //rotate around focus group, recognisable +offset to default Zrotation
                //which should be parallel to the line of view(Z vector of camera)
                break;
            case HorizontalAngle.RightAngle:
                calculatedRot = new Vector3(initalRot.x, 180 - (Angle - 45), initalRot.z); 
                debugline += angle.ToString();

                break;
            case HorizontalAngle.LeftAngle:
                calculatedRot = new Vector3(initalRot.x, 180 - (Angle + 45), initalRot.z);//180 - (Angle + 45)
                debugline += angle.ToString();

                break;
            
        }

        //Debug.Log(debugline);
        _horizontalAngle = angle;
        calculatedRot.y = 180 - Angle > 180f ? calculatedRot.y * -1 : calculatedRot.y;
        _baseDetail.transform.eulerAngles = calculatedRot;
    }
    private void ApplyVAngle(VerticalAngle angle)
    {
        var CalculatedRot = _baseDetail.transform.eulerAngles;
        Vector3 initialRot = _baseDetail.transform.eulerAngles;
        string debugline = "Vertical angle: ";
        switch (angle)
        {
            case VerticalAngle.BirdsEye:
                CalculatedRot = new Vector3(-60f, initialRot.y, initialRot.z);
                debugline += angle.ToString();
                break;
            case VerticalAngle.High:
                CalculatedRot = new Vector3(-30f, initialRot.y, initialRot.z);
                debugline += angle.ToString();
                break;
            case VerticalAngle.EyeLevel:
                CalculatedRot = initialRot;
                debugline += angle.ToString();
                break;
            case VerticalAngle.Low:
                CalculatedRot = new Vector3(30f, initialRot.y, initialRot.z);
                debugline += angle.ToString();
                break;
            case VerticalAngle.MouseEye:
                CalculatedRot = new Vector3(60f, initialRot.y, initialRot.z);
                debugline += angle.ToString();
                break;
        }
        //Debug.Log(debugline);
        _verticalAngle = angle;
        _baseDetail.transform.eulerAngles = CalculatedRot;
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
        var focusFirst = OPController.FocusLayer.First();
        var focusLast = OPController.FocusLayer.Last();

        Vector3 ResultPoint = focusFirst.transform.position + ((focusLast.transform.position - focusFirst.transform.position) / 2);
        ResultPoint.y = OPController.FocusLayer.OrderByDescending(x => x.GetComponent<SceneObject>().Bounds.size.y)
                                                        .First()
                                                        .GetComponent<SceneObject>()
                                                        .Bounds.size.y / 2;
        return ResultPoint;
        //if (FObjects.Count > 1)
        //{
        //    Vector3 ResultPoint = FObjects.First().transform.position + ((FObjects.Last().transform.position - FObjects[0].transform.position) / 2);
        //    var AltResultPoint = ResultPoint;
        //    //ResultPoint.y = GetObjectSize(FObjects.OrderByDescending(x => GetObjectSize(x).y).First()).y / 2;

        //    return ResultPoint;
        //}
        //else if (FObjects.Count == 1)
        //{
        //    Vector3 ResultPoint = FObjects.First().transform.position;
        //    ResultPoint.y = FObjects[0].GetComponent<SceneObject>().Bounds.size.y / 2;
        //    return ResultPoint;
        //}
        //else
        //{
        //    return new Vector3(0, 2, 0);
        //}
    }
    private float CalculateSpringArmLength(Vector3 CenterOfFrame, float GivenCoefficient)
    {
        var TopPoint = CenterOfFrame;
        TopPoint.y = CenterOfFrame.y * 2;
        var BottomPoint = CenterOfFrame;
        BottomPoint.y = 0;

        var FObjects = OPController.FocusLayer;
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

        var FObjects = OPController.FocusLayer;
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
        foreach (GameObject FocusedObject in OPController.FocusLayer)
        {
            ResultWidth += FocusedObject.GetComponent<SceneObject>().Width;
        }
        return ResultWidth;
    }
    private float GetFObjectsHeight()
    {
        float maxHeight = 0;
        foreach(var Object in OPController.FocusLayer)//TODO: update focused objects on event
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

    public void BuildNewShot()
    {
        var shotParameters = CPHandler.ShotOptionsHandle(FrameDescription.RawFrameInput);

        ExecuteParameters(shotParameters);
    }
    public void ExecuteParameters(ShotParameters shotParameters)
    {
        _baseDetail.UpdatePosition();

        if (shotParameters.ShotType == ShotType.DefaultShot)
            DefaultShot();
        else
            this.SendMessage(shotParameters.ShotType.ToString());

        _shot = shotParameters.ShotType;
        ApplyThird(shotParameters.Third);
        ApplyVAngle(shotParameters.VAngle);
        ApplyHAngle(shotParameters.HAngle);

    }

    #region shots

    public void VeryCloseShot()
    {
        DefaultShot();
        //get object's bone
        //get object actual size
        //set camera new location depend on object's actual size 
        //set camera rotation to look at chosen bone
        Debug.Log("Extremely Close shot");
        _shot = ShotType.CloseShot;

    }

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
        _shot = ShotType.MediumShot;

    }
    public void LongShot()
    {
        DefaultShot();
        //CalcFocusedObjectsBounds();

        //ParentGO.transform.position = Vector3.Lerp(LeftObjectsLimit, RightObjectsLimit, coef);
        
        Debug.Log("Long shot");
        _shot = ShotType.LongShot;

    }

    public void ExtremelyLongShot()
    {
        //get object actual size
        //find distance between object and camera, so that environment could fit into camera view 
        DefaultShot(0.07f);

        //after that rot is 0
        //get a screen space used by focus layer
        //keep going back until used screenspace is 3-5%
        Debug.Log("Extremely Long shot");
        _shot = ShotType.ExtremelyLongShot;

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
