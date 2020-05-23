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
    private Vector3 CenterOfFrame;

    private Vector3 CameraInitialPos;

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

    private List<GameObject> FocusGroup;

    void Start()
    {
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
        CurrentCamera.transform.rotation = CalculateCameraRotation(CenterOfFrame);
        CameraInitialPos = CurrentCamera.transform.position;
    }
    private void ApplyThird(HorizontalThird third)
    {
        var thirdsGO = CurrentCamera.transform.Find("Thirds");
        Vector3 focusPosition = BaseDetail.transform.position;

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

        if(!Helper.myApproximation(thirdPoint.x, focusPosition.x, 0.1f))
        {
            int Direction = (thirdPoint.x < focusPosition.x) ? 1 : -1;
            CurrentCamera.transform.position = new Vector3(Mathf.Abs(thirdPoint.x - focusPosition.x), 0, 0) * Direction + CameraInitialPos;
        }
        else if(!Helper.myApproximation(thirdPoint.z, focusPosition.z, 0.1f))
        {
            int Direction = (thirdPoint.z < focusPosition.z) ? 1 : -1;
            CurrentCamera.transform.position = new Vector3(0, 0, Mathf.Abs(thirdPoint.z - focusPosition.z)) * Direction + CameraInitialPos;
        }
        _third = third;



    }
    private void ApplyHAngle(HorizontalAngle angle, float pow = 1)
    {
        Vector3 calculatedRot = BaseDetail.transform.eulerAngles;
        Vector3 initalRot = BaseDetail.transform.eulerAngles;

        var focus = new Vector3(BaseDetail.transform.forward.x, 0, BaseDetail.transform.forward.z);

        var cameraPos = BaseDetail.transform.position - CurrentCamera.transform.position ;
        cameraPos = new Vector3(cameraPos.x, 0, cameraPos.z);

        float deltaAngle = Vector3.Angle(focus, cameraPos);
        Vector3 cross = Vector3.Cross(focus, cameraPos);
        deltaAngle = cross.y < 0 ? -deltaAngle : deltaAngle;

        float yAngle = Mathf.Lerp(15f, 45f, pow);
        switch (angle)
        {
            case HorizontalAngle.Front:
                calculatedRot = new Vector3(initalRot.x, 180 - deltaAngle, initalRot.z);
                break;
            case HorizontalAngle.DeadFront:
                calculatedRot.y = Quaternion.Euler(-30f, 0, 0).x;
                break;
            case HorizontalAngle.RightAngle:
                calculatedRot = new Vector3(initalRot.x, 180 - (deltaAngle - yAngle), initalRot.z); 
                break;
            case HorizontalAngle.LeftAngle:
                calculatedRot = new Vector3(initalRot.x, 180 - (deltaAngle + yAngle), initalRot.z);
                break;
        }
        _horizontalAngle = angle;
        calculatedRot = isFromBehind ? new Vector3(calculatedRot.x, calculatedRot.y + 180, calculatedRot.z) 
                                     : calculatedRot;
        BaseDetail.transform.eulerAngles = calculatedRot;
    }
    private void ApplyVAngle(VerticalAngle angle, float pow = 1)
    {
        var CalculatedRot = BaseDetail.transform.eulerAngles;
        Vector3 initialRot = BaseDetail.transform.eulerAngles;
        switch (angle)
        {
            case VerticalAngle.BirdsEye:
                CalculatedRot = new Vector3(-60f, initialRot.y, initialRot.z);
                break;
            case VerticalAngle.High:
                CalculatedRot = new Vector3(-30f, initialRot.y, initialRot.z);
                break;
            case VerticalAngle.EyeLevel:
                CalculatedRot = initialRot;
                break;
            case VerticalAngle.Low:
                CalculatedRot = new Vector3(30f, initialRot.y, initialRot.z);
                break;
            case VerticalAngle.MouseEye:
                CalculatedRot = new Vector3(60f, initialRot.y, initialRot.z);
                break;
        }
        _verticalAngle = angle;
        BaseDetail.transform.eulerAngles = CalculatedRot;
    }
    private Vector3 CalculateCameraPosition(float springArmCoef)
    {
        //FocusGroup = OPController.FocusGroups.Count != 0 ?
        //                                            OPController.FocusGroups.First().GetAllChildren() :
        //                                            OPController.FocusLayer;
        var exception = OPController.FocusGroups.FirstOrDefault(x => x.name == "Orphange");
        if (exception != null)
            FocusGroup = OPController.FocusLayer.Except(exception.GetAllChildren()).ToList();
        else
            FocusGroup = OPController.FocusLayer;

        CenterOfFrame = CalculateCenterOfFrame(FocusGroup);
        var SpringArmLength = CalculateSpringArmLength(CenterOfFrame, springArmCoef, FocusGroup);

        Vector3 PerpendicularDirection = CalculatePerpendicularDirection(FocusGroup);
        return CenterOfFrame + (SpringArmLength * PerpendicularDirection);
    }

    #region calculations
    /// <summary>
    /// Finds a middle between the most left and right objects,
    /// the Y of it is half of the biggest height along objects
    /// </summary>
    /// <returns>
    /// Returns a Vector3 position 
    /// </returns>
    private Vector3 CalculateCenterOfFrame(List<GameObject> focusGroup = null)
    {
        Vector3 resultPoint;
        if(focusGroup == null)
        {
            focusGroup = OPController.FocusGroups.Count != 0 ?
                                                    OPController.FocusGroups.First().GetAllChildren() :
                                                    OPController.FocusLayer;
        }

        var focusFirst = focusGroup.First();
        var focusLast = focusGroup.Last();

        Vector3 ResultPoint = focusFirst.transform.position + ((focusLast.transform.position - focusFirst.transform.position) / 2);
        ResultPoint.y = focusGroup.OrderByDescending(x => x.GetComponent<SceneObject>().Bounds.size.y)
                                                            .First()
                                                            .GetComponent<SceneObject>()
                                                            .Bounds.size.y / 2;
        
        return ResultPoint;
    }
    private float CalculateSpringArmLength(Vector3 CenterOfFrame, float GivenCoefficient, List<GameObject> focusGroup)
    {
        var TopPoint = CenterOfFrame;
        TopPoint.y = CenterOfFrame.y * 2;
        var BottomPoint = CenterOfFrame;
        BottomPoint.y = 0;

        float SpringArmLength;

        float maxHeight = 0;
        foreach (GameObject inFocus in focusGroup)
        {
            if (maxHeight < inFocus.GetComponent<SceneObject>().Height)
                maxHeight = inFocus.GetComponent<SceneObject>().Height;
        }
        float Width = Vector3.Distance(focusGroup.First().transform.position, focusGroup.Last().transform.position) + 
                      (focusGroup.First().GetComponent<SceneObject>().Width + focusGroup.First().GetComponent<SceneObject>().Width)/2;
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
    private Vector3 CalculatePerpendicularDirection(List<GameObject> focusGroup)
    {
        var DirectionVector = focusGroup.Count >=2 ? 
                            Vector3.Cross(focusGroup.First().transform.position - focusGroup.Last().transform.position, Vector3.up) :
                            focusGroup[0].transform.forward;
        DirectionVector.Normalize();
        return DirectionVector;
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
        foreach (var FocusedObject in FocusGroup)
        {
            ResultWidth += FocusedObject.GetComponent<SceneObject>().Width;
        }
        return ResultWidth;
    }
    private float GetFObjectsHeight()
    {
        float maxHeight = 0;
        foreach(var Object in FocusGroup)
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
        var shotParameters = CPHandler.ShotParametersHandle(FrameDescription.RawFrameInput);
        ExecuteParameters(shotParameters);
    }
    public void ExecuteParameters(ShotParameters shotParameters)
    {
        BaseDetail.UpdatePosition(CalculateCenterOfFrame());
                
        if (shotParameters.ShotType == ShotType.DefaultShot)
            DefaultShot();
        else
            this.SendMessage(shotParameters.ShotType.ToString());
        
        isFromBehind = shotParameters.isfromBehind;
        _shot = shotParameters.ShotType;
        ApplyThird(shotParameters.Third);
        ApplyVAngle(shotParameters.VAngle);
        ApplyHAngle(shotParameters.HAngle);
        ObjectsPlacementController.OnContentPreparedEvent -= BuildNewShot;
        if(OPController.FocusGroups.FirstOrDefault(x=>x.name=="Orphange"))
            CompositionCorrector.CorrectGroups(CurrentCamera, OPController, _third);
    }

    #region shots
    public void CloseShot()
    {
        DefaultShot(2.1f);
        string boneName = "head";
        var boneTransform = OPController.FocusLayer.First().GetComponent<SceneObject>().GetTransformByBone(boneName);
        float cameraNewHeight = (boneTransform.right * -1f * 0.07f).y + boneTransform.position.y;

        CurrentCamera.transform.position = new Vector3(CurrentCamera.transform.position.x, cameraNewHeight, CurrentCamera.transform.position.z);
        CameraInitialPos = CurrentCamera.transform.position;
        _shot = ShotType.CloseShot;
    }
    public void MediumShot()
    {
        DefaultShot(0.75f);
        float cameraNewHeight = (float)(GetFObjectsHeight() * 0.75);
        CurrentCamera.transform.position = new Vector3(CurrentCamera.transform.position.x, cameraNewHeight, CurrentCamera.transform.position.z);

        CameraInitialPos = CurrentCamera.transform.position;
        _shot = ShotType.MediumShot;
    }
    public void LongShot()
    {
        DefaultShot();
        CameraInitialPos = CurrentCamera.transform.position;
        _shot = ShotType.LongShot;

    }
    public void ExtremelyLongShot()
    {
        DefaultShot(0.07f);
        _shot = ShotType.ExtremelyLongShot;
    }
    #endregion

}
