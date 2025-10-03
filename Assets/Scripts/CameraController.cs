using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform carPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = carPlayer.transform.position + new Vector3(0, 1, -5);
    }
}
