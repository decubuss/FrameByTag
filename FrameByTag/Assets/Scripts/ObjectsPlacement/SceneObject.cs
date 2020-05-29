using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class ModelExtension
{
    public static Bounds GetBounds(this GameObject go)
    {
        Bounds resultBounds;
        if (go.GetComponent<MeshFilter>() != null)
            resultBounds = go.GetComponent<MeshFilter>().mesh.bounds;
        else if (go.GetComponent<MeshFilter>() != null)
            resultBounds = go.GetComponent<MeshFilter>().sharedMesh.bounds;
        else if (go.GetComponentInChildren<SkinnedMeshRenderer>() != null)
            resultBounds = go.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
        else if(go.transform.childCount>0)
        {
            resultBounds = GetOverallBounds(go.GetAllChildren());
        }
        else { Debug.LogError("no bounds on " + go.name);  resultBounds = new Bounds(); }

        var scale = go.transform.localScale;
        var vector = new Vector3(resultBounds.size.x * scale.x, resultBounds.size.y * scale.y, resultBounds.size.z * scale.z);
        resultBounds = new Bounds(resultBounds.center, vector);
        return resultBounds;
    }
    public static float GetVolume(this Bounds bounds)
    {
        float volume = bounds.extents.x * bounds.extents.y * bounds.extents.z;
        return volume;
    }
    public static Vector3 RotatePointAroundPivot(this Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * ((point - pivot) + pivot);
    }
    private static Bounds GetOverallBounds(List<GameObject> children)
    {
        Bounds resultBounds = new Bounds();
        foreach (var child in children)
        {
            resultBounds.Encapsulate(child.GetBounds());
        }
        return resultBounds;
    }
}

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

        if (gameObject.GetBounds() != null)
            resultBounds = gameObject.GetBounds();
        else if (gameObject.transform.childCount > 0)
        {
            resultBounds = GetOverallBounds(gameObject.GetAllChildren());
        }
        else
        {
            Debug.LogError("no bounds on " + gameObject.name);
            return new Bounds();
        }

        return resultBounds;
    }
    private Bounds GetOverallBounds(List<GameObject> children)
    {
        Bounds resultBounds = new Bounds();
        foreach(var child in children)
        {
            resultBounds.Encapsulate(child.GetBounds());
        }
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
        float fixedTime = Random.Range(0.0f, 0.5f);
        Animator.PlayInFixedTime(stateName, 0, fixedTime);
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
