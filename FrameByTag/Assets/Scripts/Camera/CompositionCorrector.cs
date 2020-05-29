using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CompositionCorrector
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

        OP.FocusGroups.First().SetActive(false);
        var result = HitTheGround(layer, chosenThird.transform.position).point;

        if (result == Vector3.zero)
        {
            
            result = HitTheGround(layer, _thirds[1].transform.position).point;
        }

        OP.FocusGroups.First().SetActive(true);
        return result;
    }
    private static RaycastHit HitTheGround(int layer, Vector3 third)
    {
        var screenPoint = Camera.WorldToScreenPoint(third);// + new Vector3(0,1,0)
        var worldRay = Camera.ScreenPointToRay(screenPoint);
        var hits = Physics.RaycastAll(Camera.transform.position, worldRay.direction * 1000f, 10000f, ~layer, QueryTriggerInteraction.Collide);
        //Debug.DrawRay(Camera.transform.position, worldRay.direction * 1000, Color.white, 1000f);

        //RaycastHit hit = hits.FirstOrDefault(x => x.collider.gameObject.layer == layer);
        

        return hits.FirstOrDefault(x => x.collider.gameObject.layer == layer);
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

    public static void CameraZoomFix()
    {
        var focusGroup = OP.FocusGroups.First();

    }

}
