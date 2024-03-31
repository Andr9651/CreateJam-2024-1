using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField]
    List<Transform> links;

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
                person.SetDestination(getRandomPosition());
            }));
        }
    }

    private Vector3 getRandomPosition()
    {
        var link = links[Random.Range(0, links.Count)];
        var xScale = link.transform.localScale.x / 2;
        var zScale = link.transform.localScale.z / 2;
        var xOffset = Random.Range(-xScale, xScale);
        var zOffset = Random.Range(-zScale, zScale);

        return link.position + new Vector3(xOffset, 0, zOffset);
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

    private void OnValidate()
    {
        links = links.Distinct().Where(link => link.Equals(transform) == false).ToList();
    }

#if UNITY_EDITOR
    [MenuItem("Custom Editors/Link Waypoints")]
    public static void LinkWaypoints()
    {

        if (Selection.activeTransform.TryGetComponent<Waypoint>(out var currentWaypoint) == false)
        {
            Debug.Log("A waypoint is not the primary selection");
            return;
        }

        List<Waypoint> waypoints = new List<Waypoint>();

        foreach (var item in Selection.transforms)
        {
            if (item.TryGetComponent<Waypoint>(out var waypoint))
            {
                waypoints.Add(waypoint);
            }
        }

        var currentObj = new SerializedObject(currentWaypoint);

        var currentLinks = currentObj.FindProperty(nameof(links));

        foreach (var waypoint in waypoints)
        {

            currentLinks.InsertArrayElementAtIndex(0);
            currentLinks.GetArrayElementAtIndex(0).objectReferenceValue = waypoint.transform;

            var waypointObj = new SerializedObject(waypoint);
            var waypointLinks = waypointObj.FindProperty(nameof(links));

            waypointLinks.InsertArrayElementAtIndex(0);
            waypointLinks.GetArrayElementAtIndex(0).objectReferenceValue = currentWaypoint.transform;
            waypointObj.ApplyModifiedProperties();
        }

        currentObj.ApplyModifiedProperties();
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

        foreach (var waypoint in waypoints)
        {
            var obj = new SerializedObject(waypoint);

            print(nameof(links));

            var linkArray = obj.FindProperty(nameof(links));

            linkArray.ClearArray();
            obj.ApplyModifiedProperties();
        }
    }
#endif
}
