using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public float gripFactor = 0.95f;
    public float accelerationFactor = 15;

    public float brakeFactor = 53.22f;
    public float turnFactor = 2.5f;
    public float maxSpeed = 20;

    float accelerationInput = 0;
    float steeringInput = 0;

    float rotationAngle = 0;

    float velocityVsUp = 0;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Accelerate();
        ApplySteering();
        KillOrthogonalVelocity();
    }

    void Accelerate()
    {
        velocityVsUp = Vector2.Dot(transform.up, rb.linearVelocity);

        if (velocityVsUp > maxSpeed && accelerationInput > 0)
        {
            return;
        }

        if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0)
        {
            return;
        }

        if (rb.linearVelocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
        {
            return;
        }

        if (accelerationInput == 0)
        {
            rb.linearDamping = Mathf.Lerp(rb.linearDamping, 3.0f, Time.fixedDeltaTime * 1.1f);
        }
        else
        {
            rb.linearDamping = 0;
        }

        Vector2 AccVector = Vector2.zero;

        if (accelerationInput > 0)
        {
            AccVector = transform.up * accelerationInput * accelerationFactor;
        }
        else
        {
            AccVector = transform.up * accelerationInput * brakeFactor;
        }


        rb.AddForce(AccVector, ForceMode2D.Force);

    }

    void ApplySteering()
    {
        float minSpeedBeforeAllowTurningFactor = (rb.linearVelocity.magnitude / 8);
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);

        rotationAngle += steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;

        rb.MoveRotation(rotationAngle);
    }

    public void setInputVector(Vector2 inputVector)
    {
        steeringInput = -inputVector.x;
        accelerationInput = inputVector.y;
    }

    public void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.linearVelocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.linearVelocity, transform.right);

        rb.linearVelocity = forwardVelocity + rightVelocity * gripFactor;
    }

}
