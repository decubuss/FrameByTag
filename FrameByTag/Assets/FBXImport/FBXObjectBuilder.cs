using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBXObjectBuilder : MonoBehaviour
{
    public void BuildSceneObject(string properties)
    {
        if (properties.Contains("bones&timeline"))
        {
            //Build Skinned mesh
        }
        else
        {
            BuildStaticMesh(properties);
        }
        //not implemented
    }

    private GameObject BuildAnimatedMesh(string properties)
    {
        return new GameObject();
    }

    private GameObject BuildStaticMesh(string properties)
    {
        MeshFilter SceneObjectMesh=new MeshFilter();

        //get vertices form properties
        //get triangles from properties
        //get normals from properties
        SceneObjectMesh.mesh.vertices = new Vector3[] {new Vector3(0f,0f,0f),new Vector3(1f,0f,0f),new Vector3(0f,0f,1f),
                                                       new Vector3(0f,1f,0f)};
        SceneObjectMesh.mesh.triangles = new int[] {  };


        return new GameObject();
    }
}
