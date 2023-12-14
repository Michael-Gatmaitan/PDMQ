

using UnityEngine;

public class ShurikenRotater : MonoBehaviour
{
    // Start is called before the first frame update
    // Vector3 rotateValue = new(1000, 1000, 1000);
    private readonly int shurikenSpeed = 25;
    private readonly int shurikenRotationSpeed = 600;

    private Vector3 cameraForward;
    private Vector3 camPos;
    void Start()
    {
        Debug.Log("Im a shuriken");
        cameraForward = Camera.main.transform.forward;
        camPos = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            transform.eulerAngles.y,
            transform.eulerAngles.z + shurikenRotationSpeed * Time.deltaTime
        );

        transform.position += shurikenSpeed * Time.deltaTime * cameraForward;

        float distToPlayer = Vector3.Distance(camPos, transform.position);

        if (distToPlayer > 80)
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
