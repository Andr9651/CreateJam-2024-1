using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Person : MonoBehaviour
{
    Rigidbody rb;
    NavMeshAgent agent;
    [SerializeField] Rigidbody gib;
    [SerializeField] float gibForce;

    [SerializeField] Transform sprite;
    Camera cam;
    public PersonManager personManager;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        cam = Camera.main;

        sprite.GetComponent<SpriteRenderer>().color = Random.ColorHSV();
    }

    public void SetDestination(Vector3 pos)
    {
        if (agent.enabled == true)
        {
            agent.SetDestination(pos);
        }
    }

    private void LateUpdate()
    {
        sprite.rotation = cam.transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            agent.enabled = false;
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<Renderer>().enabled = false;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;

            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().kills++;
            sprite.gameObject.SetActive(false);
            personManager.SpawnPerson();

            for (int i = 0; i < 10; i++)
            {
                Vector3 posOffset = Random.onUnitSphere;
                var gibBody = Instantiate(gib, transform.position + posOffset, Quaternion.identity);
                gibBody.AddExplosionForce(gibForce, other.transform.position, 5);
                StartCoroutine(DestroyGib(gibBody.gameObject));
            }
        }
    }

    IEnumerator DestroyGib(GameObject gib)
    {

        yield return new WaitForSeconds(3);
        gib.GetComponent<MeshRenderer>().enabled = false;
        gib.GetComponent<Rigidbody>().isKinematic = true;
        gib.GetComponentInChildren<ParticleSystem>().Stop();

        yield return new WaitForSeconds(1);
        Destroy(gameObject);

    }
}
