using UnityEngine;
using UnityEngine.AI;

public class Person : MonoBehaviour
{
    Rigidbody rb;
    NavMeshAgent agent;
    [SerializeField] Rigidbody gib;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetDestination(Vector3 pos)
    {
        if (agent.enabled == true)
        {
            agent.SetDestination(pos);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            agent.enabled = false;
            Destroy(gameObject);

            for (int i = 0; i < 10; i++)
            {
                Vector3 posOffset = Random.onUnitSphere;
                var gibBody = Instantiate(gib, transform.position + posOffset, Quaternion.identity);
                gib.AddExplosionForce(50, other.transform.position, 5);
            }
        }
    }
}
