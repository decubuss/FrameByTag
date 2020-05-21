using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDetail : MonoBehaviour
{
    private Camera ControlledCamera;
    private Vector3 UnmodifiedPosition;
    private Quaternion UnmodifiedRotation;

    void Start()
    {
        ControlledCamera = FindObjectOfType<CameraSetter>().CurrentCamera;
    }

    public void UpdatePosition(Vector3 frameCenter)
    {
        if (!gameObject) { Debug.Log("no detail here"); return; }

        gameObject.transform.position = UnmodifiedPosition;
        gameObject.transform.rotation = UnmodifiedRotation;
        if (gameObject.transform.childCount != 0)
        {
            gameObject.transform.DetachChildren();
        }
        UnmodifiedPosition = frameCenter;//FindObjectOfType<ObjectsPlacementController>().GetBaseDetailPosition();

        gameObject.transform.position = UnmodifiedPosition;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        if (ControlledCamera == null) { ControlledCamera = FindObjectOfType<CameraSetter>().CurrentCamera; }
        ControlledCamera.transform.SetParent(gameObject.transform);

    }
    public void UpdateBosition(Vector3 frameCenter)
    {
        if (!gameObject) { Debug.Log("no detail here"); return; }
        if (ControlledCamera == null) { ControlledCamera = FindObjectOfType<CameraSetter>().CurrentCamera; }

        gameObject.transform.position = UnmodifiedPosition;
        UnmodifiedPosition = frameCenter;//FindObjectOfType<ObjectsPlacementController>().GetBaseDetailPosition();
        gameObject.transform.position = UnmodifiedPosition;
    }
    public void UpdateRotation()
    {
        gameObject.transform.rotation = UnmodifiedRotation;
        UnmodifiedRotation = ObjectsPlacementController.RotationToDirection(UnmodifiedPosition, ControlledCamera.transform.position);//Quaternion.Euler(new Vector3(0f, 0f, 0f)); ////
        gameObject.transform.rotation = UnmodifiedRotation;//Quaternion.Euler(0, 0, 0);
    }
    public void CameraBind()
    {
        ControlledCamera.transform.SetParent(gameObject.transform);
    }
    public void CameraUnbind()
    {
        if (gameObject.transform.childCount != 0)
        {
            gameObject.transform.DetachChildren();
        }
    }
}
