
using UnityEngine;

public class CameraManagement : MonoBehaviour
{
    private Transform playerTransform;

    public Vector3 localOffset = new Vector3(0, 20f, -1.5f);  
    public float smoothSpeed = 20f;
    public float angle = 65; 

    private float currentYVelocity;

    void Start()
    {
        playerTransform = CarController.Instance.transform;
    }

    void FixedUpdate()
    {
        Vector3 worldOffset = 
              playerTransform.forward * localOffset.z   
            + playerTransform.right * localOffset.x     
            + Vector3.up * localOffset.y;               

        Vector3 targetPosition = playerTransform.position + worldOffset;

        
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

        
        float targetY = playerTransform.eulerAngles.y;

        float smoothY = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            targetY,
            ref currentYVelocity,
            0.35f
        );

        transform.rotation = Quaternion.Euler(angle, smoothY, 0f);
    }
}

