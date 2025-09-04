using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circuit : MonoBehaviour
{
    public List<GameObject> waypoints;
    public GameObject waypointPrefab;


    void OnDrawGizmos()
    {
        DrawGizmos(false);
    }

    void OnDrawGizmosSelected()
    {
        DrawGizmos(true);
    }
    
    public void PlaceWayPoint(Vector3 startPosition, Vector3Int direction, int length)
    {

        var rotation = Quaternion.identity;

        if (direction.x != 0)
        {
            rotation = Quaternion.Euler(0, 90, 0);
        }
        for (int i = 0; i < length; i++)
        {
            var position = Vector3Int.RoundToInt(startPosition + direction * i);
            //if (roadDictionary.ContainsKey(position))
            //{
            //    continue;
            //}
            var point = Instantiate(waypointPrefab, position, rotation, transform);
            waypoints.Add(point);

            if (i == 0 || i == length - 1)
            {
            }
        }
    }
    void DrawGizmos(bool selected)
    {
        if (selected == false) return;
        if (waypoints.Count > 1)
        {
            Vector3 prev = waypoints[0].transform.position;
            for (int i = 1; i < waypoints.Count; i++)
            {
                Vector3 next = waypoints[i].transform.position;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
            Gizmos.DrawLine(prev, waypoints[0].transform.position);
        }
    }

}
