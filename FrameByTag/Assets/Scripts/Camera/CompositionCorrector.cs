using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CompositionCorrector : MonoBehaviour
{
    private static GameObject[] _thirds;
    private static Camera Camera;
    private static ObjectsPlacementController OP;
    
    public static void CorrectGroups(Camera camera, ObjectsPlacementController op, HorizontalThird usedThird)
    {
        if (op.FocusGroups.Count == 0) { return; }
        Camera = camera;
        OP = op;
        _thirds = GetThirds(camera);
        var groupToEdit = op.FocusGroups.FirstOrDefault(x=>x.name=="Orphange");
        var groupNewPosition = CalculateGroupPos(usedThird, groupToEdit.GetAllChildren());
        groupToEdit.transform.position = new Vector3(groupNewPosition.x, 0, groupNewPosition.z);
    }
    
    private static Vector3 CalculateGroupPos(HorizontalThird thirdToIgnore, List<GameObject> groupToEdit)
    {
        if (OP == null) { return Vector3.zero; }
        GameObject chosenThird = thirdToIgnore == HorizontalThird.LastThird ? _thirds[0] : _thirds[2];

        Bounds primaryBounds = new Bounds();
        OP.FocusGroups.First()
                      .GetAllChildren()
                      .ForEach(x => primaryBounds.Encapsulate(x.GetComponent<SceneObject>().Bounds));
        Bounds secondaryBounds = new Bounds();
        groupToEdit.ForEach(x => secondaryBounds.Encapsulate(x.GetComponent<SceneObject>().Bounds));

        var deltaBounds = secondaryBounds.GetVolume() / primaryBounds.GetVolume();
        int layer;
        if (deltaBounds < 1.5)
            layer = 13;
        else if (2 <= deltaBounds && deltaBounds < 4)
            layer = 14;
        else if (4 <= deltaBounds)
            layer = 15;
        else
            layer = 15;

        return HitTheGround(layer, chosenThird.transform.position).point;
    }
    private static RaycastHit HitTheGround(int layer, Vector3 third)
    {
        var screenPoint = Camera.WorldToScreenPoint(third);
        var worldRay = Camera.ScreenPointToRay(screenPoint);
        RaycastHit[] hits = Physics.RaycastAll(Camera.transform.position, worldRay.direction, 1000f);
        var hit = hits.FirstOrDefault(x => x.collider.gameObject.layer == layer);
        Debug.DrawRay(Camera.transform.position, worldRay.direction, Color.white, 1000f);


        if (hits.Contains(hit))//, OP.GroundMesh.layer))//, 1000f, ground.layer))
        {
            Debug.Log(hit.point + " " + layer);
            return hit;
        }
        else
        {
            Debug.LogError("no hit layer " + layer);
        }
        return hit;
    }
    
    private static GameObject[] GetThirds(Camera camera)
    {
        if (_thirds == null)
        {
            _thirds = new GameObject[3];
            var ThirdsGO = camera.transform.Find("Thirds");
            var thirds = ThirdsGO.gameObject.GetAllChildren();
            _thirds[0] = thirds.FirstOrDefault(x => x.name == "FirstThird");
            _thirds[1] = thirds.FirstOrDefault(x => x.name == "Center");
            _thirds[2] = thirds.FirstOrDefault(x => x.name == "LastThird");
            //Debug.Log(_thirds[0].Name);

            return _thirds;
        }
        else
            return _thirds;

    }

}
