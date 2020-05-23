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
        var groupNewPosition = LocationChecker(op.GroundMesh, usedThird);
        groupToEdit.transform.position = groupNewPosition.Last();
    }
    private static GameObject[] GetThirds(Camera camera)
    {
        if (_thirds == null)
        {
            _thirds = new GameObject[3];
            var ThirdsGO = camera.transform.Find("Thirds");
            var thirds = ThirdsGO.gameObject.GetAllChildren();
            _thirds[0] = thirds.FirstOrDefault(x=>x.name=="FirstThird");
            _thirds[1] = thirds.FirstOrDefault(x => x.name == "Center");
            _thirds[2] = thirds.FirstOrDefault(x => x.name == "LastThird");
            //Debug.Log(_thirds[0].Name);

            return _thirds;
        }
        else
            return _thirds;
         
    }
    private static List<Vector3> LocationChecker(GameObject ground, HorizontalThird usedThird)
    {
        var freeThirds = new List<Vector3>();

        GameObject chosenThird = usedThird==HorizontalThird.LastThird? _thirds[0]:_thirds[2];
        //var thirdsGroup = new List<GameObject>() { chosenThird };
        //foreach (var third in thirdsGroup)
        //{
        //    freeThirds.AddRange(ThirdSpaceAvailable(ground, third));
        //}
        Vector3 dot = DotChoice(chosenThird);
        freeThirds.Add(TryPlace(dot));
        return freeThirds;
    }
    private static List<Vector3> ThirdSpaceAvailable(GameObject ground, GameObject third)
    {
        var freePositions = new List<Vector3>();


        foreach (var dot in third.GetComponent<ThirdBody>().Dots)
        {
            var screenPoint = Camera.WorldToScreenPoint(dot.position);
            var worldRay = Camera.ScreenPointToRay(screenPoint);//thirdPoint);
            RaycastHit hit;
            if (Physics.Raycast(Camera.transform.position, worldRay.direction, out hit, ground.layer))//, 1000f, ground.layer))
            {
                //Debug.Log(hit.point);
                freePositions.Add(hit.point);
            }
        }
        return freePositions;
    }
    private static Vector3 DotChoice(GameObject third)
    {
        if (OP == null) { return Vector3.zero; }
        Bounds primaryBounds = new Bounds();
        OP.FocusGroups.First()
                      .GetAllChildren()
                      .ForEach(x => primaryBounds.Encapsulate(x.GetComponent<SceneObject>().Bounds));
        Bounds secondaryBounds = new Bounds();
        OP.FocusGroups.Last()
                      .GetAllChildren()
                      .ForEach(x => secondaryBounds.Encapsulate(x.GetComponent<SceneObject>().Bounds));

        var deltaBounds = secondaryBounds.GetVolume() / primaryBounds.GetVolume();
        var dots = third.GetComponent<ThirdBody>().Dots;
        if (deltaBounds < 1.5)
            return dots[0].position;
        else if (2 <= deltaBounds && deltaBounds < 4)
            return dots[1].position;
        else if (4 <= deltaBounds && deltaBounds < 6)
            return dots[2].position;
        else
            return dots[1].position;

    }
    private static Vector3 TryPlace(Vector3 dotPosition)
    {
        var screenPoint = Camera.WorldToScreenPoint(dotPosition);
        var worldRay = Camera.ScreenPointToRay(screenPoint);
        RaycastHit hit;
        Debug.DrawRay(Camera.transform.position, worldRay.direction * 10000f, Color.white, 1000f);

        if (Physics.Raycast(Camera.transform.position, worldRay.direction, out hit, 100f))//, OP.GroundMesh.layer))//, 1000f, ground.layer))
        {
            return hit.point;
        }
        else
        {
            Debug.LogError("no hit");
            return Vector3.zero;
        }
    }

}
