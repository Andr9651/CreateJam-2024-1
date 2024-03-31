using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform player;

    [SerializeField]
    public float heightReached;
    public float timeStarted;
    public int kills;
    public bool resetPos = true;


    [SerializeField] Vector3 startPos = new Vector3(0, 2, 0);

    // Start is called before the first frame update
    void Start()
    {
        ResetScore();
    }

    // Update is called once per frame
    void Update()
    {
        heightReached = Mathf.Max(heightReached, player.position.y);

        if (kills > 200)
        {
            WinSecret();
        }
    }

    public void Win()
    {
        SceneManager.LoadScene(1);
    }

    public void WinSecret()
    {
        SceneManager.LoadScene(2);
    }

    public void ResetScore()
    {
        heightReached = 0;
        kills = 0;
        timeStarted = Time.time;

        if (resetPos == false)
        {
            return;
        }
        
        player.position = startPos;
        var rb = player.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

    }
}
