using UnityEngine;

public class PreviewManager : MonoBehaviour
{
    private Rigidbody rb;
    public WheelCollider FL, FR, RL, RR;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnCollisionEnter(Collision collision)
    {
        FL.enabled = false;
        FR.enabled = false;
        RL.enabled = false;
        RR.enabled = false;
        
        FL.enabled = true;
        FR.enabled = true;
        RL.enabled = true;
        RR.enabled = true;
    }
}
