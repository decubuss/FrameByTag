using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SceneObject : MonoBehaviour, INameAlternatable
{
    [SerializeField]
    public string CurrentState;
    private Animator Animator;

    public string[] AltNames;
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
                parts[i] = Helper.MakeCapitalLetter(parts[i]);// char.ToUpper(parts[i].First()) + parts[i].Remove(0, 1);
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

    private Bounds GetObjectBounds()
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
    public Dictionary<string,string> GetAlternateNames()
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        foreach(var key in AltNames.OrderByDescending(x => x.Length))
        {
            result.Add(key, Name);
        }

        return result;//TODO
    }
    public Transform GetTransformByBone(string boneName)
    {
        if(GetAnimator() == null) { Debug.LogError("no bones on: " + gameObject.name); return null; }

        var result = Animator.GetBoneTransform(HumanBodyBones.Head);
        return result;
    }

    public void SetMaterial(Material mat)
    {
        if (gameObject.GetComponentInChildren<MeshRenderer>() != null)
            gameObject.GetComponentInChildren<MeshRenderer>().material = mat;
        else if (gameObject.GetComponentInChildren<SkinnedMeshRenderer>() != null)
            gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = mat;
        else
        {
            Debug.LogError("no bounds on " + gameObject.name);
        }
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
    private void SetState(string stateName)
    {
        if (Animator == null) { return; }
        if( !Animator.HasState(0, Animator.StringToHash(stateName))) 
        { 
            Debug.LogError(string.Format("{0} has not such state:{1}", Name, stateName )); 
            return; 
        }
        Animator.speed = 0f;
        Animator.PlayInFixedTime(stateName, 0, 0.0f);
        CurrentState = stateName;
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
