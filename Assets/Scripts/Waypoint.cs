using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField]
    private List<Transform> links;

    [Range(0, 5)]
    [SerializeField] private float triggerDelayRange = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Person>(out var person))
        {

            if (links.Count == 0)
            {
                print("end reached");
                return;
            }

            StartCoroutine(TriggerAfterWait(() =>
            {
                person.SetDestination(links[Random.Range(0, links.Count)].position);
            }));
        }
    }

    IEnumerator TriggerAfterWait(System.Action trigger)
    {
        yield return new WaitForSeconds(Random.Range(0, triggerDelayRange));
        trigger?.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "node.png", false);
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var waypoint in links)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, waypoint.position);
        }
    }

    [MenuItem("Custom Editors/Link Waypoints")]
    public static void LinkWaypoints()
    {

        if (Selection.activeTransform.TryGetComponent<Waypoint>(out var currentWaypoint) == false)
        {
            Debug.Log("A waypoint is not the primary selection");
            return;
        }

        Debug.Log("Joining waypoints");

        List<Waypoint> waypoints = new List<Waypoint>();

        foreach (var item in Selection.transforms)
        {
            if (item.TryGetComponent<Waypoint>(out var waypoint))
            {
                waypoints.Add(waypoint);
            }
        }


        Debug.Log(waypoints.Count);

        Undo.RecordObjects(waypoints.ToArray(), "Link waypoints");

        foreach (var waypointToBeAdded in waypoints)
        {
            Debug.Log(currentWaypoint.name + " <-> " + waypointToBeAdded.name);
            if (currentWaypoint.Equals(waypointToBeAdded))
            {
                Debug.Log("same");
                continue;
            }

            var isAlreadyLinked = false;

            foreach (var linkedWaypoint in currentWaypoint.links)
            {
                if (waypointToBeAdded.Equals(linkedWaypoint))
                {
                    Debug.Log("already linked");
                    isAlreadyLinked = true;
                    continue;

                }
            }

            if (isAlreadyLinked)
            {
                continue;
            }

            currentWaypoint.links.Add(waypointToBeAdded.transform);
            waypointToBeAdded.links.Add(currentWaypoint.transform);
        }
    }

    [MenuItem("Custom Editors/Clear Waypoint links")]
    public static void ClearWaypointLinks()
    {
        List<Waypoint> waypoints = new List<Waypoint>();

        foreach (var item in Selection.transforms)
        {
            if (item.TryGetComponent<Waypoint>(out var waypoint))
            {
                waypoints.Add(waypoint);
            }
        }

        Undo.RecordObjects(waypoints.ToArray(), "Clear waypoint links");

        foreach (var waypoint in waypoints)
        {
            waypoint.links.Clear();
        }
    }
}
