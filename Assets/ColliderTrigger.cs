using UnityEngine;
using UnityEngine.Events;

public class ColliderTrigger : MonoBehaviour
{
    public UnityEvent playerTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTrigger?.Invoke();
        }
    }
}
