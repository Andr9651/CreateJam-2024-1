using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Vector3 cameraPos;
    [SerializeField] Transform dummy;
    [SerializeField] new Camera camera;
    [SerializeField] LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        cameraPos = dummy.localPosition;
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 toCamera = transform.localToWorldMatrix.MultiplyPoint(cameraPos) - transform.position;

        Physics.Raycast(new Ray(transform.position, toCamera), out var hit, toCamera.magnitude, layerMask);


        if (hit.distance == 0)
        {

            dummy.transform.localPosition = cameraPos;
        }
        else
        {

            dummy.transform.localPosition = dummy.transform.localPosition.normalized * hit.distance;
        }
    }

    private void LateUpdate()
    {
        camera.transform.SetPositionAndRotation(dummy.position, dummy.rotation);
    }
}
