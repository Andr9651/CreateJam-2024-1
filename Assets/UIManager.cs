using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField] TMP_Text left;
    [SerializeField] TMP_Text middle;
    [SerializeField] TMP_Text right;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        left.text = $"\"Necessary\" Sacifices: {gameManager.kills.ToString("D2")}";
        middle.text = $"Height Reached: {Mathf.FloorToInt(gameManager.heightReached).ToString("D5")}";
        right.text = $"Time: {(Time.time - gameManager.timeStarted).ToString("F3")}";
    }
}
