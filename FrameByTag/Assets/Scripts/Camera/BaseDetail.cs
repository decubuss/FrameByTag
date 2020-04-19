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

    public void UpdatePosition()
    {
        if (!gameObject) { Debug.Log("no detail here"); return; }

        gameObject.transform.position = UnmodifiedPosition;
        gameObject.transform.rotation = UnmodifiedRotation;
        if (gameObject.transform.childCount != 0)
        {
            gameObject.transform.DetachChildren();
        }
        UnmodifiedPosition = FindObjectOfType<ObjectsPlacementController>().GetBaseDetailPosition();

        gameObject.transform.position = UnmodifiedPosition;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        if (ControlledCamera == null) { ControlledCamera = FindObjectOfType<CameraSetter>().CurrentCamera; }
        ControlledCamera.transform.SetParent(gameObject.transform);

    }
}
