using System;
using Unity.Mathematics;
using UnityEngine;

public class CarFX : MonoBehaviour
{

    [Header("Terrain Management")]
    public TerrainHandler asphaltProfile;
    public TerrainHandler dirtProfile;
    public TerrainHandler grassProfile;
    private float slipThreshold;

    [Header("FX Prefabs")]
    public GameObject smokePrefab;
    public GameObject tyreTrail;

    [Header("Sound Settings")]
    [Header("Engine")]
    public AudioSource engineSound;
    public float engineMaxVolume = 0.6f;
    public float engineMinVolume = 0.05f;
    public float engineTargetVol;
    public float engineMinPitch = 1f;
    public float engineMaxPitch = 3.5f;
    public float engineTargetPtich;
    public float revLimiterThreshold = 0.95f;
    public float revPulseFrequency = 20f;
    public float revPulseAmplitude = 0.5f;
    public float revPitchBoost = 0.2f;

    public float SmoothTime = 4f;

    public void UpdateWheelParticles(WheelCollider collider, ParticleSystem wheelSmoke, TrailRenderer trailRenderer, float speed)
    {
        WheelHit hit;

        if(collider.GetGroundHit(out hit))
        {
            PhysicsMaterial currentMaterial = hit.collider.sharedMaterial;

            TerrainHandler currentGrip = GetGripProfile(currentMaterial);

            if(currentGrip != null)
            {
                collider.forwardFriction = currentGrip.forwardFriction.ToWheelFrictionCurve();
                collider.sidewaysFriction = currentGrip.sidewaysFriction.ToWheelFrictionCurve();

                var main = wheelSmoke.main;
                main.startColor = currentGrip.smokeColor;
                main.startLifetime = currentGrip.startLifeTime;
                main.startSpeed = currentGrip.startSpeed;
                main.startSize = currentGrip.startSize;

                this.slipThreshold = currentGrip.slipThreshold;

                trailRenderer.material = currentGrip.material;

                float speedFactor = Mathf.InverseLerp(4f, 50f, speed);

                var emission = wheelSmoke.emission;
                emission.rateOverTime = Mathf.Lerp(0f, currentGrip.maxEmissionRate, speedFactor);
            }

        }
    }

    public void CheckWheelParticles(WheelCollider wheel, ParticleSystem particleSystem, TrailRenderer trailRenderer)
    {
        WheelHit hit;

        if (wheel.GetGroundHit(out hit))
        {
            particleSystem.transform.position = hit.point;

            var emission = particleSystem.emission;

            float sidewaysSlip = Mathf.Abs(hit.sidewaysSlip);
            float forwardSlip = Mathf.Abs(hit.forwardSlip);

            bool isSliding = sidewaysSlip > slipThreshold;

            if (isSliding)
            {
                if (!particleSystem.isEmitting)
                {
                    particleSystem.Play();
                }


                emission.enabled = true;
                trailRenderer.emitting = true;
            }
            else
            {
                emission.enabled = false;
                trailRenderer.emitting = false;
            }
        }
    }

    public void HandleWheelParticles(WheelColliders colliders, WheelParticles wheelParticles)
    {
        CheckWheelParticles(colliders.FLWheel, wheelParticles.FLWheel, wheelParticles.FLWheelTrail);
        CheckWheelParticles(colliders.FRWheel, wheelParticles.FRWheel, wheelParticles.FRWheelTrail);
        CheckWheelParticles(colliders.RLWheel, wheelParticles.RLWheel, wheelParticles.RLWheelTrail);
        CheckWheelParticles(colliders.RRWheel, wheelParticles.RRWheel, wheelParticles.RRWheelTrail);
    }

    private TerrainHandler GetGripProfile(PhysicsMaterial material)
    {
        if (material == null) return asphaltProfile;

        switch (material.name)
        {
            case "Asphalt": return asphaltProfile;
            case "Dirt": return dirtProfile;
            case "Grass": return grassProfile;
            default: return asphaltProfile;
        }
    }
    public void InitiateParticles(WheelParticles wheelParticles, WheelColliders colliders)
    {
        if (smokePrefab)
        {
            wheelParticles.FLWheel = Instantiate(smokePrefab, colliders.FLWheel.transform.position - Vector3.up * colliders.FLWheel.radius, Quaternion.identity, colliders.FLWheel.transform).GetComponent<ParticleSystem>();
            wheelParticles.FRWheel = Instantiate(smokePrefab, colliders.FRWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.FRWheel.transform).GetComponent<ParticleSystem>();
            wheelParticles.RLWheel = Instantiate(smokePrefab, colliders.RLWheel.transform.position - Vector3.up * colliders.RLWheel.radius, Quaternion.identity, colliders.RLWheel.transform).GetComponent<ParticleSystem>();
            wheelParticles.RRWheel = Instantiate(smokePrefab, colliders.RRWheel.transform.position - Vector3.up * colliders.RRWheel.radius, Quaternion.identity, colliders.RRWheel.transform).GetComponent<ParticleSystem>();
            

            wheelParticles.FLWheel.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            wheelParticles.FRWheel.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            wheelParticles.RLWheel.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            wheelParticles.RRWheel.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        }

        if (tyreTrail)
        {
            wheelParticles.FLWheelTrail = Instantiate(tyreTrail, colliders.FLWheel.transform.position - Vector3.up * colliders.FLWheel.radius, Quaternion.identity, colliders.FLWheel.transform).GetComponent<TrailRenderer>();
            wheelParticles.FRWheelTrail = Instantiate(tyreTrail, colliders.FRWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.FRWheel.transform).GetComponent<TrailRenderer>();
            wheelParticles.RLWheelTrail = Instantiate(tyreTrail, colliders.RLWheel.transform.position - Vector3.up * colliders.RLWheel.radius, Quaternion.identity, colliders.RLWheel.transform).GetComponent<TrailRenderer>();
            wheelParticles.RRWheelTrail = Instantiate(tyreTrail, colliders.RRWheel.transform.position - Vector3.up * colliders.RRWheel.radius, Quaternion.identity, colliders.RRWheel.transform).GetComponent<TrailRenderer>();


            wheelParticles.FLWheelTrail.transform.localPosition += new Vector3(0f, 0.1f, 0f);
            wheelParticles.FRWheelTrail.transform.localPosition += new Vector3(0f, 0.1f, 0f);
            wheelParticles.RLWheelTrail.transform.localPosition += new Vector3(0f, 0.1f, 0f);
            wheelParticles.RRWheelTrail.transform.localPosition += new Vector3(0f, 0.1f, 0f);
        }
    }

    public void HandleEngineSound(float rpmRatio)
    {        
        engineTargetVol = Mathf.Lerp(engineMinVolume, engineMaxVolume, rpmRatio);

        float pulse = (rpmRatio >= revLimiterThreshold)
                        ?Mathf.Sin(Time.time * revPulseFrequency) * revPulseAmplitude + (1f - revPulseAmplitude/2f)
                        : 1f;

        engineSound.volume = Mathf.Lerp(engineSound.volume, engineTargetVol * pulse, Time.deltaTime * SmoothTime);

        engineTargetPtich = Mathf.Lerp(engineMinPitch, engineMaxPitch, rpmRatio);

        engineSound.pitch = Mathf.Lerp(engineSound.pitch, engineTargetPtich + (rpmRatio >= revLimiterThreshold ? revPitchBoost : 0), Time.deltaTime * SmoothTime);

    }

}
