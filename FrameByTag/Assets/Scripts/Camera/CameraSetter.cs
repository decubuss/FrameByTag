using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ShotType
{
    LongShot, CloseShot, MediumShot, ExtremelyLongShot, DefaultShot
}
public enum HorizontalThird
{
    FirstThird, Center, LastThird
}
public enum VerticalAngle
{
    BirdsEye, High, EyeLevel, Low, MouseEye
}
public enum HorizontalAngle
{
    RightAngle, LeftAngle, Front, DeadFront
}

public class CameraSetter : MonoBehaviour
{
    private BaseDetail BaseDetail;

    [HideInInspector]
    public Camera CurrentCamera;
    private ObjectsPlacementController OPController;
    private CameraParametersHandler CPHandler;
    private FrameAttributes Frame;

    private Vector3 ShotInitialPos;

    [SerializeField]
    private HorizontalThird _third;
    [SerializeField]
    private HorizontalAngle _horizontalAngle;
    [SerializeField]
    private VerticalAngle _verticalAngle;
    [SerializeField]
    private ShotType _shot;
    [SerializeField]
    private bool isFromBehind;
    // Start is called before the first frame update
    void Start()
    {
        Frame = new FrameAttributes();
        OPController = FindObjectOfType<ObjectsPlacementController>();
        CPHandler = new CameraParametersHandler(this);
        CurrentCamera = Camera.main;
        BaseDetail = new GameObject("BaseDetail").AddComponent<BaseDetail>();

        ObjectsPlacementController.OnStartupEndedEvent += StartupShot;
        FrameDescription.OnDescriptionChangedEvent += CameraSetReady;
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
        Vector3 focusPosition = BaseDetail.transform.position;//OPController.FocusLayer.First().transform;

        var relativePos = CurrentCamera.transform.InverseTransformPoint(focusPosition);
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

        if(!Helper.myApproximation(thirdPoint.x, focusPosition.x, 0.1f))//Mathf.Approximately())
        {
            int Direction = (thirdPoint.x < focusPosition.x) ? 1 : -1;
            CurrentCamera.transform.position = new Vector3(Mathf.Abs(thirdPoint.x - focusPosition.x), 0, 0) * Direction + ShotInitialPos;
        }
        else if(!Helper.myApproximation(thirdPoint.z, focusPosition.z, 0.1f))
        {
            int Direction = (thirdPoint.z < focusPosition.z) ? 1 : -1;
            CurrentCamera.transform.position = new Vector3(0, 0, Mathf.Abs(thirdPoint.z - focusPosition.z)) * Direction + ShotInitialPos;
        }
        _third = third;



    }
    private void ApplyHAngle(HorizontalAngle angle, float pow = 1)
    {
        Vector3 calculatedRot = BaseDetail.transform.eulerAngles;
        Vector3 initalRot = BaseDetail.transform.eulerAngles;

        var focus = new Vector3(BaseDetail.transform.forward.x, 0, BaseDetail.transform.forward.z);//_baseDetail.transform.forward.z

        var camera = BaseDetail.transform.position - CurrentCamera.transform.position ;
        camera = new Vector3(camera.x, 0, camera.z);

        float Angle = Vector3.Angle(focus, camera);
        Vector3 cross = Vector3.Cross(focus, camera);
        Angle = cross.y < 0 ? -Angle : Angle;
        string debugline = "Horizontal angle: ";

        //var pow = UnityEngine.Random.Range(0.05f, 1f);
        float yAngle = Mathf.Lerp(15f, 45f, pow);
        switch (angle)
        {
            case HorizontalAngle.Front:
                calculatedRot = new Vector3(initalRot.x, 180 - Angle, initalRot.z);//_baseDetail.transform.rotation.x
                debugline += angle.ToString();
                break;
            case HorizontalAngle.DeadFront:
                calculatedRot.y = Quaternion.Euler(-30f, 0, 0).x;
                debugline += angle.ToString();
                
                break;
            case HorizontalAngle.RightAngle:
                calculatedRot = new Vector3(initalRot.x, 180 - (Angle - yAngle), initalRot.z); 
                debugline += angle.ToString();

                break;
            case HorizontalAngle.LeftAngle:

                calculatedRot = new Vector3(initalRot.x, 180 - (Angle + yAngle), initalRot.z);
                debugline += angle.ToString();

                break;
            
        }
        _horizontalAngle = angle;
        calculatedRot = isFromBehind ? new Vector3(calculatedRot.x, calculatedRot.y + 180, calculatedRot.z) 
                                     : calculatedRot;
        //calculatedRot.y = 180 - Angle > 180f ? calculatedRot.y * -1 : calculatedRot.y;
        BaseDetail.transform.eulerAngles = calculatedRot;
    }
    private void ApplyVAngle(VerticalAngle angle, float pow = 1)
    {
        var CalculatedRot = BaseDetail.transform.eulerAngles;
        Vector3 initialRot = BaseDetail.transform.eulerAngles;
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
        BaseDetail.transform.eulerAngles = CalculatedRot;
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
    }
    private float CalculateSpringArmLength(Vector3 CenterOfFrame, float GivenCoefficient)
    {
        var TopPoint = CenterOfFrame;
        TopPoint.y = CenterOfFrame.y * 2;
        var BottomPoint = CenterOfFrame;
        BottomPoint.y = 0;

        var FObjects = OPController.FocusLayer;
        var SpringArmLength = 0f;

        float maxHeight = 0;
        foreach (GameObject inFocus in FObjects)
        {
            if (maxHeight < inFocus.GetComponent<SceneObject>().Height)
                maxHeight = inFocus.GetComponent<SceneObject>().Height;
        }
        float Width = Vector3.Distance(FObjects.First().transform.position, FObjects.Last().transform.position) + 
                      (FObjects.First().GetComponent<SceneObject>().Width + FObjects.First().GetComponent<SceneObject>().Width)/2;
        if (maxHeight > Width)
        {
            SpringArmLength = (float)((Vector3.Distance(TopPoint, BottomPoint) / 2) / GivenCoefficient);
        }
        else
        {
            SpringArmLength = (float)(Width / Mathf.Tan(45));
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
    public void BuildNewShot()
    {
        var shotParameters = CPHandler.ShotParametersHandle(FrameDescription.RawFrameInput);//ObjectsPlacementHandler.LastTaggedInput);//

        ExecuteParameters(shotParameters);
    }
    public void ExecuteParameters(ShotParameters shotParameters)
    {
        BaseDetail.UpdatePosition(CalculateCenterOfFrame());
        //BaseDetail.CameraUnbind();
        //BaseDetail.UpdatePosition(CalculateCenterOfFrame());
        
        if (shotParameters.ShotType == ShotType.DefaultShot)
            DefaultShot();
        else
            this.SendMessage(shotParameters.ShotType.ToString());
        //BaseDetail.UpdateRotation();
        //BaseDetail.CameraBind();

        isFromBehind = shotParameters.isfromBehind;
        _shot = shotParameters.ShotType;
        ApplyThird(shotParameters.Third);
        ApplyVAngle(shotParameters.VAngle);
        ApplyHAngle(shotParameters.HAngle);
        ObjectsPlacementController.OnContentPreparedEvent -= BuildNewShot;
    }

    #region shots
    public void CloseShot()
    {
        DefaultShot(2.1f);
        string boneName = "head";
        var boneTransform = OPController.FocusLayer.First().GetComponent<SceneObject>().GetTransformByBone(boneName);
        float cameraNewHeight = (boneTransform.right * -1f * 0.07f).y + boneTransform.position.y;

        CurrentCamera.transform.position = new Vector3(CurrentCamera.transform.position.x, cameraNewHeight, CurrentCamera.transform.position.z);
        ShotInitialPos = CurrentCamera.transform.position;
        _shot = ShotType.CloseShot;

    }
    public void MediumShot()
    {
        DefaultShot(0.75f);
        float cameraNewHeight = (float)(GetFObjectsHeight() * 0.75);
        CurrentCamera.transform.position = new Vector3(CurrentCamera.transform.position.x, cameraNewHeight, CurrentCamera.transform.position.z);

        ShotInitialPos = CurrentCamera.transform.position;
        _shot = ShotType.MediumShot;
    }
    public void LongShot()
    {
        DefaultShot();
        ShotInitialPos = CurrentCamera.transform.position;
        _shot = ShotType.LongShot;

    }
    public void ExtremelyLongShot()
    {
        DefaultShot(0.07f);
        _shot = ShotType.ExtremelyLongShot;
    }
    #endregion

}
