
using UnityEngine;

public class CameraManagement : MonoBehaviour
{
    private Transform playerTransform;
    private Transform cameraPosition;

    public Vector3 localOffset = new Vector3(0, 20f, -1.5f);  
    public float smoothSpeed = 20f;
    public float verticalSmoothSpeed = 5f;
    public float angle = 65; 

    private float currentYVelocity;

    void Start()
    {
        playerTransform = CarController.Instance.transform;
        cameraPosition = gameObject.GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        Vector3 worldOffset = 
              playerTransform.forward * localOffset.z   
            + playerTransform.right * localOffset.x     
            + Vector3.up * localOffset.y;               

        Vector3 targetPosition = playerTransform.position + worldOffset;

        float targetPositionX = Mathf.Lerp(cameraPosition.position.x, targetPosition.x, smoothSpeed * Time.deltaTime);
        float targetPositionZ = Mathf.Lerp(cameraPosition.position.z, targetPosition.z, smoothSpeed * Time.deltaTime);
        float targetPositionY = Mathf.Lerp(cameraPosition.position.y, targetPosition.y, verticalSmoothSpeed * Time.deltaTime);

        
        cameraPosition.position = new Vector3(targetPositionX, targetPositionY, targetPositionZ);

        float targetRotationY = playerTransform.eulerAngles.y;


        float smoothY = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            targetRotationY,
            ref currentYVelocity,
            0.35f
        );

        transform.rotation = Quaternion.Euler(angle, smoothY, 0f);
    }
}

