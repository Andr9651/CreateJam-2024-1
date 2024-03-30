using UnityEngine;
using UnityEngine.AI;

public class Person : MonoBehaviour
{
    Rigidbody rb;
    NavMeshAgent agent;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetDestination(Vector3 pos)
    {
        agent.SetDestination(pos);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (agent.enabled == true)
        {
            agent.enabled = false;
            rb.isKinematic = false;
            rb.AddForceAtPosition(-collision.impulse * 0.5f, collision.GetContact(0).point, ForceMode.Impulse);
        }
    }
}
