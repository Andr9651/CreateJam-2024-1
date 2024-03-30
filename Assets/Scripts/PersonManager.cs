using System.Collections.Generic;
using UnityEngine;

public class PersonManager : MonoBehaviour
{

    [SerializeField] new Camera camera;
    [SerializeField] private int amount;
    [SerializeField] List<Person> agent;

    [SerializeField] Rigidbody ballPrefab;
    [SerializeField] Person personPrefab;
    [SerializeField] float throwForce;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;

        var waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

        for (int i = 0; i < amount; i++)
        {
            Instantiate(personPrefab, waypoints[Random.Range(0, waypoints.Length)].transform.position, Quaternion.identity, transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = camera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                foreach (var person in agent)
                {

                    person.SetDestination(hit.point);
                }

            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = camera.ScreenPointToRay(mousePosition);
            var body = Instantiate(ballPrefab, camera.transform.position, Quaternion.identity);

            body.AddForce(ray.direction * throwForce);
        }
    }
}
