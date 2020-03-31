using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SceneObject : MonoBehaviour, INameAlternatable
{
    public string[] Keys;
    public string Name;
    string SceneObjectType;
    public string[] Poses;
    public List<UnityEditor.Animations.ChildAnimatorState> States = new List<UnityEditor.Animations.ChildAnimatorState>();
    private Animator Animator;

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
        if (Animator != null)
        { 
            if(Animator.HasState(0, Animator.StringToHash("Idle")))
            {
                Animator.speed = 0f;
                Animator.PlayInFixedTime("Idle", 0, 0.0f);
            }
       
            var ac = gameObject.GetComponent<Animator>().runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

            UnityEditor.Animations.AnimatorStateMachine sm = ac.layers[0].stateMachine;
            UnityEditor.Animations.ChildAnimatorState[] states = sm.states;
            States = states.ToList();
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

    public Dictionary<string[],string> GetAlternateNames()
    {
        Dictionary<string[], string> result = new Dictionary<string[], string>();
        result.Add(Keys, Name);

        return result;//TODO
    }
    public List<string> GetStateNames()
    {
        if (States.Count != 0)
        {
            List<string> names = new List<string>();
            foreach(var state in States)
            {
                names.Add(state.state.name);
            }
            return names;
        }
        else
        {
            return null;
        }
    }
    public void SetStateByName(string name)
    {
        if (Animator != null && Animator.HasState(0, Animator.StringToHash(name)))
        {
            SetState(name);
        }
        else
            Debug.LogError( string.Format("no animator on {0}", Name) );
    }
    private void SetState(string name)
    {
        if (Animator == null) { return; }
        Animator.PlayInFixedTime(name, 0, 0.0f);
    }
    
}
