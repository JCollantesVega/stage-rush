using System.Collections;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public static CarController Instance { get; private set; }

    [Header("References")]
    public CarFX carFX; //referencia al objeto que maneja los efectos del coche

    [Header("DEBUG UI")]
    public float currentHp; //caballos de fuerza que da el coche en función de las RPM

    [Header("Car Values")]
    public float brakeForce; //fuerza de frenado aplicada
    public AnimationCurve steeringCurve; //curva de giro según velocidad
    [SerializeField] public DriveType driveType; //tipo de tracción(delantera, trasera o 4 ruedas)

    [Header("Engine Config")]
    public float RPM; //revoluciones por minuto del motor
    public float redLine; //revoluciones máximas del motor
    public float idleRPM; //revoluciones de ralentí
    public int currentGear; //marcha actual engranada
    public float[] gearRatios; //relación de marcha para calcular velocidad
    public float differentialRatio; //relaciçon de diferencial
    public float clutch; //embrague
    public AnimationCurve hpGraph; //curva de potencia por RPM
    public GearState gearState; //Estado de marcha
    public float increaseGearRPM; //revoluciones a la que se sube de marcha
    public float decreaseGearRPM; //revoluciones a la que se baja de marcha
    public float changeGearTime = 0.5f; //tiempo que se tarada en cambiar de marcha
    public float wheelRPM; //velocidad a la que giran las ruedas(usado para debug)

    [Header("Car Components")]
    public WheelColliders colliders; //grupo de WheelColliders
    public WheelMeshes wheelMeshes; //grupo de mallas de rueda(modelo 3d)

    [HideInInspector]
    public WheelParticles wheelParticles; //grupo de particulas y efectos de cada rueda

    [Header("Input Controllers")]
    [SerializeField]private float gasInput;
    [SerializeField]private float brakeInput;
    [SerializeField]private float steerInput;

    public float speed{ get; private set; } //velocidad del coche en u/s(se escala a m/s)

    public Rigidbody rb; //referencia al RigidBody del coche


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
        speed = rb.linearVelocity.magnitude;

        GetInput();
        ApplyEngineForce();
        ApplyBraking();
        HandleSteering();
        carFX.HandleWheelParticles(colliders, wheelParticles);
        carFX.HandleEngineSound(RPM / redLine);
        carFX.HandleBrakeLights();
        UpdateWheels();

        Debug.Log($"Wheel RPM: FL:{(int)colliders.FLWheel.rpm} FR:{(int)colliders.FRWheel.rpm} RL:{(int)colliders.RLWheel.rpm} RR:{(int)colliders.RRWheel.rpm}");
    }


    //toma y procesa inputs obtenidos de la instancia InputManager
    private void GetInput()
    {
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


    //Calcula el torque que se manda a las ruedas
    private float CalculateTorque()
    {
        //variable que se devuelve
        float torque = 0;

        //si las RPM se encuentran dentro de un rango alrededor de ralentí y está la primera marcha engranada
        //la marcha será punto muerto
        if (RPM < idleRPM + 200 && gasInput == 0 && currentGear == 0)
        {
            gearState = GearState.Neutral;
        }

        if (gearState == GearState.Running && clutch == 0) //si hay una marcha engranada y no se está pulsando embrague
        {
            //sube o baja de marcha según el caso
            if (RPM >= increaseGearRPM)
            {
                StartCoroutine(ChangeGear(1));
            }
            else if (RPM <= decreaseGearRPM)
            {
                StartCoroutine(ChangeGear(-1));
            }
        }


        if(gasInput > 0) //al acelerar
        {
            if (clutch > 0.1f || gearState == GearState.Neutral)
            {
                //si se está pulsando el embrague o está en punto muerto, las RPM suben sin aplicar fuerza a als ruedas
                RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM, redLine * gasInput) + Random.Range(-50, 50), Time.deltaTime * 3f);
                if(RPM >= redLine)
                {
                    RPM = Mathf.Lerp(RPM, redLine - 200f, Time.deltaTime * 20f);
                }
            }
            else
            {
                //obtiene la velocidad de las ruedas
                wheelRPM = Mathf.Abs(GetAllWheelsRPM()) * gearRatios[currentGear] * differentialRatio;

                //float engineDesiredRPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM, redLine * gasInput) + Random.Range(-50, 50), Time.deltaTime * 3f);
                RPM = Mathf.Lerp(RPM, Mathf.Max(idleRPM, wheelRPM), Time.deltaTime * 3f);

                //RPM = Mathf.Clamp(RPM, idleRPM, redLine);

                if(RPM >= redLine)
                {
                    RPM = Mathf.Lerp(RPM, redLine - 200f, Time.deltaTime * 20f);
                }

                //obtener potencia según las RPM obtenidas
                currentHp = hpGraph.Evaluate(RPM);

                //convertir de Hp a N/m
                float engineTorque = currentHp * 0.7457f * 9550 / RPM * Mathf.Clamp01(1f - clutch);

                //cálculo final de potencia entregada
                torque = engineTorque * gearRatios[currentGear] * differentialRatio;
            }
        }
        else
        {
            RPM = Mathf.Lerp(RPM, idleRPM, Time.deltaTime * 1.5f);
        }

        return torque;
    }

    private float GetAllWheelsRPM()
    {
        float WheelRpm = 0;
        switch(driveType)
        {
            case DriveType.RWD:
                WheelRpm = (colliders.RRWheel.rpm + colliders.RLWheel.rpm) / 2f;
                break;
            case DriveType.FWD:
                WheelRpm = (colliders.FRWheel.rpm + colliders.FLWheel.rpm) / 2f;
                break;
            case DriveType.AWD:
            WheelRpm = (colliders.RRWheel.rpm + colliders.RLWheel.rpm + colliders.FRWheel.rpm + colliders.FLWheel.rpm) / 4f;
            break;
        }

        return WheelRpm;
    }

    //aplicar fuerza a las ruedas según el tipo de transmisión
    private void ApplyEngineForce()
    {
        float totalForce = CalculateTorque();
        switch (driveType)
        {
            case DriveType.RWD:
                colliders.RLWheel.motorTorque = totalForce / 2 * gasInput;
                colliders.RRWheel.motorTorque = totalForce / 2 * gasInput;
                break;
            case DriveType.FWD:
                colliders.FLWheel.motorTorque = totalForce / 2 * gasInput;
                colliders.FRWheel.motorTorque = totalForce / 2 * gasInput;
                break;
            case DriveType.AWD:
                colliders.RLWheel.motorTorque = totalForce / 4 * gasInput;
                colliders.RRWheel.motorTorque = totalForce / 4 * gasInput;
                colliders.FLWheel.motorTorque = totalForce / 4 * gasInput;
                colliders.FRWheel.motorTorque = totalForce / 4 * gasInput;
                break;
        }
    }

    //aplicar fuerza de frenado según input y brakeBias(balance de frenado)
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

    //actualiza posición y rotación de las ruedas según wheel collider
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