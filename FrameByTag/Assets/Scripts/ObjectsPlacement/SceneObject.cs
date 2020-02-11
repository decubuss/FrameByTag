using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObject : MonoBehaviour
{
    public string[] Keys;
    public string Name;
    string SceneObjectType;
    public string[] Poses;

    private Bounds _bounds;
    public Bounds Bounds
    {
        get
        {
            if (_bounds == null)
            {
                _bounds = GetObjectBounds();
                return _bounds;
            }
            else
                return _bounds;
        }

    }

    private float _width;
    public float Width
    {
        get
        {
            if (_width == 0f)
            {
                _width = GetObjectBounds().extents.x * 2;
                return _width;
            }
            else
                return _width;
        }
    }

    private float _height;
    public float Height
    {
        get
        {
            if (_height == 0f)
            {
                _height = GetObjectBounds().extents.y * 2;//NULL: make it go back
                Debug.Log(_height);
                return _height;
            }
            else
                return _height;
        }
    }

    public void Start()
    {
        var AnimPointer = gameObject.GetComponent<Animator>();
        if (AnimPointer != null)
        {
            AnimPointer.speed = 0f;
            AnimPointer.PlayInFixedTime("Idle01", 0, 0.0f);
        }
    }

    public Bounds GetObjectBounds()
    {
        if (gameObject.GetComponent<MeshFilter>() != null)
            return gameObject.GetComponent<MeshFilter>().mesh.bounds;
        else if (gameObject.GetComponentInChildren<SkinnedMeshRenderer>() != null)
            return gameObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
        else
            return new Bounds();
    }

}
