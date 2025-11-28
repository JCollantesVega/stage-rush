using System.Collections;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public static CarController Instance { get; private set; }

    [Header("References")]
    public CarFX carFX;

    [Header("DEBUG UI")]
    public float currentHp;

    [Header("Car Values")]
    public float brakeForce;
    public AnimationCurve steeringCurve;
    [SerializeField] public DriveType driveType;

    [Header("Engine Config")]
    public float RPM;
    public float redLine;
    public float idleRPM;
    public int currentGear;
    public float[] gearRatios;
    public float differentialRatio;
    public float clutch;
    public AnimationCurve hpGraph;
    public GearState gearState;
    public float increaseGearRPM;
    public float decreaseGearRPM;
    public float changeGearTime = 0.5f;
    public int isEngineRunning;
    public float wheelRPM;

    [Header("Car Components")]
    public WheelColliders colliders;
    public WheelMeshes wheelMeshes;

    [HideInInspector]
    public WheelParticles wheelParticles;

    [Header("Input Controllers")]
    [SerializeField]private float gasInput;
    [SerializeField]private float brakeInput;
    [SerializeField]private float steerInput;

    public float speed{ get; private set; }

    private Vector3 centerOfMass;
    public Rigidbody rb;


    float SpeedAtRedline(float gearRatio, float finalDrive, float wheelRadius, float redLine)
    {
        return redLine * (2f * Mathf.PI * wheelRadius) * 0.06f / (gearRatio * finalDrive);
    }

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Hay mas de un coche instanciado");
        }

        // for (int i = 0; i < gearRatios.Length; i++)
        // {
        //     float v = SpeedAtRedline(gearRatios[i], differentialRatio, colliders.RRWheel.radius, redLine);
        //     Debug.Log($"Marcha {i+1}: ratio={gearRatios[i]} -> {v:F1} km/h");
        // }

        carFX.InitiateParticles(wheelParticles, colliders);
    }

    void FixedUpdate()
    {
        centerOfMass = rb.centerOfMass;
        speed = rb.linearVelocity.magnitude;

        GetInput();
        ApplyEngineForce();
        ApplyBraking();
        HandleSteering();
        carFX.HandleWheelParticles(colliders, wheelParticles);
        carFX.HandleEngineSound(RPM / redLine);
        UpdateWheels();

        Debug.Log($"Wheel RPM: FL:{colliders.FLWheel.rpm} FR:{colliders.FRWheel.rpm} RL:{colliders.RLWheel.rpm} RR:{colliders.RRWheel.rpm}");
    }

    private void GetInput()
    {
        //gasInput = (gearState == GearState.Changing) ? 0f : InputManager.Instance.Gas;
        gasInput = InputManager.Instance.Gas;
        brakeInput = InputManager.Instance.Brake;
        steerInput = InputManager.Instance.Steer;
        
        if (gearState != GearState.Changing)
        {
            if (gearState == GearState.Neutral)
            {
                if (gasInput > 0)
                {
                    gearState = GearState.Running;
                }
            }
            clutch = InputManager.Instance.Clutch > 0f ? 1 : Mathf.Lerp(clutch, 0, 1f);
        }
        else
        {
            clutch = 0;
        }

    }

    private float CalculateTorque()
    {
        float torque = 0;

        if (RPM < idleRPM + 200 && gasInput == 0 && currentGear == 0)
        {
            gearState = GearState.Neutral;
        }

        if (gearState == GearState.Running && clutch == 0)
        {
            if (RPM > increaseGearRPM)
            {
                StartCoroutine(ChangeGear(1));
            }
            else if (RPM < decreaseGearRPM)
            {
                StartCoroutine(ChangeGear(-1));
            }
        }


        if(gasInput > 0)
        {
            if (clutch > 0.1f || gearState == GearState.Neutral)
            {
                RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM, redLine * gasInput) + Random.Range(-50, 50), Time.deltaTime * 3f);
            }
            else
            {
                wheelRPM = Mathf.Abs((colliders.RRWheel.rpm + colliders.RLWheel.rpm) / 2f) * gearRatios[currentGear] * differentialRatio;

                float engineDesiredRPM = Mathf.Lerp(idleRPM, redLine, Mathf.Pow(gasInput, 2f));

                float speedFactor = Mathf.Clamp01(speed / 10f);
                float wheelWeight = Mathf.Lerp(0.8f, 0.3f, speedFactor);
                wheelWeight = Mathf.Max(wheelWeight, 0.5f * (1f-clutch));

                float rpmBlend = Mathf.Lerp(engineDesiredRPM, wheelRPM, wheelWeight);  
                RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM, rpmBlend), Time.deltaTime * 1.1f);

                RPM = Mathf.Clamp(RPM, idleRPM, redLine);

                if(RPM >= redLine)
                {
                    RPM = Mathf.Lerp(RPM, redLine - 200f, Time.deltaTime * 20f);
                }

                currentHp = hpGraph.Evaluate(RPM);

                float engineTorque = currentHp*0.7457f * 9550 / RPM * Mathf.Clamp01(1f - clutch);

                torque = engineTorque * gearRatios[currentGear] * differentialRatio;
            }
        }
        else
        {
            RPM = Mathf.Lerp(RPM, idleRPM, Time.deltaTime * 1.5f);
        }

        return torque;
    }

    private void ApplyEngineForce()
    {
        float totalForce = CalculateTorque() * gasInput;
        switch (driveType)
        {
            case DriveType.RWD:
                colliders.RLWheel.motorTorque = totalForce/2;
                colliders.RRWheel.motorTorque = totalForce/2;
                break;
            case DriveType.FWD:
                colliders.FLWheel.motorTorque = totalForce/2;
                colliders.FRWheel.motorTorque = totalForce/2;
                break;
            case DriveType.AWD:
                colliders.RLWheel.motorTorque = totalForce/4;
                colliders.RRWheel.motorTorque = totalForce/4;
                colliders.FLWheel.motorTorque = totalForce/4;
                colliders.FRWheel.motorTorque = totalForce/4;
                break;
        }
    }

    private void ApplyBraking()
    {
        colliders.FLWheel.brakeTorque = brakeInput * brakeForce * 0.72f;
        colliders.FRWheel.brakeTorque = brakeInput * brakeForce * 0.72f;
        colliders.RLWheel.brakeTorque = brakeInput * brakeForce * 0.28f;
        colliders.RRWheel.brakeTorque = brakeInput * brakeForce * 0.28f;
    }

    private void HandleSteering()
    {
        float steeringAngle = steerInput * steeringCurve.Evaluate(speed*3.6f);

        colliders.FLWheel.steerAngle = Mathf.Lerp(colliders.FLWheel.steerAngle, steeringAngle, 5f * Time.deltaTime);
        colliders.FRWheel.steerAngle = Mathf.Lerp(colliders.FRWheel.steerAngle, steeringAngle, 5f * Time.deltaTime);
    }

    private void UpdateWheel(WheelCollider wheelCollider, MeshRenderer wheelMesh, ParticleSystem wheelSmoke, TrailRenderer trailRenderer)
    {
        carFX.UpdateWheelParticles(wheelCollider, wheelSmoke, trailRenderer, speed);

        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);
        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = rotation;
    }

    private void UpdateWheels()
    {
        UpdateWheel(colliders.FLWheel, wheelMeshes.FLWheel, wheelParticles.FLWheel, wheelParticles.FLWheelTrail);
        UpdateWheel(colliders.FRWheel, wheelMeshes.FRWheel, wheelParticles.FRWheel, wheelParticles.FRWheelTrail);
        UpdateWheel(colliders.RLWheel, wheelMeshes.RLWheel, wheelParticles.RLWheel, wheelParticles.RLWheelTrail);
        UpdateWheel(colliders.RRWheel, wheelMeshes.RRWheel, wheelParticles.RRWheel, wheelParticles.RRWheelTrail);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.rotation * centerOfMass, .1f);
    }

    IEnumerator ChangeGear(int gearChange)
    {
        gearState = GearState.CheckingChange;

        if (currentGear + gearChange >= 0)
        {
            if (gearChange > 0)
            {
                yield return new WaitForSeconds(0.7f);
                if (RPM < increaseGearRPM || currentGear >= gearRatios.Length - 1)
                {
                    gearState = GearState.Running;
                    yield break;
                }
            }
            if (gearChange < 0)
            {
                yield return new WaitForSeconds(0.1f);

                if (RPM > decreaseGearRPM || currentGear <= 0)
                {
                    gearState = GearState.Running;
                    yield break;
                }
            }

            gearState = GearState.Changing;
            yield return new WaitForSeconds(changeGearTime);
            currentGear += gearChange;
        }
        if(gearState!=GearState.Neutral)
        gearState = GearState.Running;
    }


    public float getSpeedRatio()
    {
        return Mathf.Sign(speed);
    }
}


[System.Serializable]
public class WheelColliders
{
    public WheelCollider FLWheel;
    public WheelCollider FRWheel;
    public WheelCollider RLWheel;
    public WheelCollider RRWheel;
}
[System.Serializable]
public class WheelMeshes
{
    public MeshRenderer FLWheel;
    public MeshRenderer FRWheel;
    public MeshRenderer RLWheel;
    public MeshRenderer RRWheel;
}


[System.Serializable]
public class WheelParticles
{
    public ParticleSystem FLWheel;
    public ParticleSystem FRWheel;
    public ParticleSystem RLWheel;
    public ParticleSystem RRWheel;

    public TrailRenderer FLWheelTrail;
    public TrailRenderer FRWheelTrail;
    public TrailRenderer RLWheelTrail;
    public TrailRenderer RRWheelTrail;
}

public enum DriveType { RWD, FWD, AWD }

public enum GearState{ Neutral, Running, CheckingChange, Changing}