using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SceneObject : MonoBehaviour, INameAlternatable
{
    string SceneObjectType;
    public string[] Poses;
    [SerializeField]
    public string CurrentState;
    private Animator Animator;

    public string[] Keys;
    public string Name;
    //{
    //    get
    //    {
    //        return _name;
    //    }
    //    set
    //    {
    //        _name = value.Replace(" ", "");
    //    }
    //}
    [SerializeField]
    private Bounds _bounds;
    public Bounds Bounds
    {
        get
        {
            if (_bounds == new Bounds(Vector3.zero, Vector3.zero))
            {
                _bounds = GetObjectBounds();
                
                return _bounds;
            }
            else
            {
                return _bounds;
            }
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
                return _height;
            }
            else
                return _height;
        }
    }



    public void Start()
    {
        Animator = gameObject.GetComponent<Animator>();
        if(Name.Contains(" ") || char.IsUpper( Name.First()))//TODO: insert it in Name.set
        {
            var parts = Name.Split(' ');
            Name = "";
            for(int i = 0; i < parts.Length; i++)
            {
                parts[i] = char.ToUpper(parts[i].First()) + parts[i].Remove(0, 1);
                Name += parts[i];
            }
        }
        if (Animator != null)
        { 
            if(Animator.HasState(0, Animator.StringToHash("Idle")))
            {
                Animator.speed = 0f;
                //Animator.speed = 0f;
                //Animator.PlayInFixedTime("Idle", 0, 0.0f); TODO
            }

        }
    }

    public Bounds GetObjectBounds()
    {
        Bounds resultBounds;
        if (gameObject.GetComponent<MeshFilter>() != null)
            resultBounds= gameObject.GetComponent<MeshFilter>().mesh.bounds;
        else if (gameObject.GetComponentInChildren<MeshFilter>() != null)
            resultBounds = gameObject.GetComponentInChildren<MeshFilter>().sharedMesh.bounds;
        else if (gameObject.GetComponentInChildren<SkinnedMeshRenderer>() != null)
            resultBounds = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
        else
        {
            Debug.LogError("no bounds on " + gameObject.name);
            return new Bounds();
        }
        var scale = gameObject.transform.localScale;
        var vector = new Vector3(resultBounds.size.x * scale.x, resultBounds.size.y * scale.y, resultBounds.size.z * scale.z);
        resultBounds = new Bounds(resultBounds.center, vector);
        return resultBounds;
    }
    public bool HasState(string name)
    {
        if (Animator != null)
        {
            return Animator.HasState(0, Animator.StringToHash(name));
        }
        return false;
    }
    public Dictionary<string,string> GetAlternateNames()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        foreach(var key in Keys.OrderByDescending(x => x.Length))
        {
            result.Add(key, Name);
        }

        return result;//TODO
    }
    public Transform GetTransformByBone(string boneName)
    {
        if(GetAnimator() == null) { Debug.LogError("no bones on: " + gameObject.name); return null; }

        var result = Animator.GetBoneTransform(HumanBodyBones.Head);
        Debug.Log(result.position);
        return result;
    }
    public void SetStateByName(string name)
    {
        string stateName = ""; //TODO: LemmatizeName 
        if (GetAnimator() != null )
        {
            SetState(name);
        }
        else
            Debug.LogError( string.Format("no animator on {0}", Name) );
    }

    private void SetState(string name)
    {
        if (Animator == null) { return; }
        if( !Animator.HasState(0, Animator.StringToHash(name))) 
        { 
            Debug.LogError(string.Format("no such state:{0}", name)); 
            return; 
        }
        Animator.speed = 0f;
        Animator.PlayInFixedTime(name, 0, 0.0f);
        CurrentState = name;
    }
    private Animator GetAnimator()
    {
        if (Animator != null)
            return Animator;
        else
        {
            Animator = gameObject.GetComponent<Animator>();
            return Animator;
        }
    }
}
