using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CompositionCorrector : MonoBehaviour
{
    private static List<GameObject> _thirds;
    
    public static void CorrectGroups(Camera camera, ObjectsPlacementController op, HorizontalThird usedThird)
    {
        if (op.FocusGroups.Count == 0) { return; }
        var groupToEdit = op.FocusGroups[1];
        var groupNewPosition = FreeSpaceDetection(camera, op.GroundMesh, usedThird);
        groupToEdit.transform.position = groupNewPosition.Last();
    }
    private static List<GameObject> GetThirds(Camera camera)
    {
        if (_thirds == null)
        {
            var ThirdsGO = camera.transform.Find("Thirds");
            _thirds = ThirdsGO.gameObject.GetAllChildren();
            return _thirds;
        }
        else
            return _thirds;
         
    }
    private static List<Vector3> FreeSpaceDetection(Camera camera, GameObject ground, HorizontalThird usedThird)
    {
        var freeThirds = new List<Vector3>();
        var thirdsGroup = GetThirds(camera)
                          .Where(x=>x.name!=usedThird.ToString());
        foreach (var third in thirdsGroup)
        {
            var thirdPoint = new Vector3(third.transform.position.x,
                                         third.transform.position.y - 0.5f,
                                         third.transform.position.z);

            var screenPoint = camera.WorldToScreenPoint(thirdPoint);
            var worldRay = camera.ScreenPointToRay(screenPoint);//thirdPoint);
            //Debug.DrawRay(camera.transform.position, worldRay.direction * 10000f, Color.white, 1000f);
            RaycastHit hit;
            if (Physics.Raycast(camera.transform.position, worldRay.direction, out hit, ground.layer))//, 1000f, ground.layer))
            {
                Debug.Log(hit.point);
                freeThirds.Add(hit.point);
            }
            else
                Debug.LogError("no hit");
        }
        return freeThirds;
    }
    private static Vector3 GroupPositionCalculation(Camera camera, HorizontalThird usedThird, GameObject ground)
    {
        var thirdGroup = GetThirds(camera).Where(x => x.name != usedThird.ToString());
        var thirdPoint = thirdGroup.First().transform.position;
        thirdPoint.y -= 0.5f;

        //GameObject dot = new GameObject();
        //dot.transform.position = thirdPoint;
        var screenPoint = camera.WorldToScreenPoint(thirdPoint);
        var worldRay = camera.ScreenPointToRay(screenPoint);//thirdPoint);
        Debug.DrawRay(camera.transform.position, worldRay.direction * 10000f,Color.white,1000f);
        RaycastHit hit;

        if (Physics.Raycast(camera.transform.position, worldRay.direction, out hit, ground.layer))//, 1000f, ground.layer))
        {
            Debug.Log(hit.point);
        }
        else
            Debug.LogError("no hit");
        return thirdPoint;
    }
}
