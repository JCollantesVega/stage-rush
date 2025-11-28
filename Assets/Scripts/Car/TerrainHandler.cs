using UnityEngine;

[System.Serializable]
public struct WheelFrictionData
{
    [Header("Wheel Settings")]
    [Range(0f, 2f)] public float extremumSlip;
    [Range(0f, 2f)] public float extremumValue;
    [Range(0f, 2f)] public float asymptoteSlip;
    [Range(0f, 2f)] public float asymptoteValue;
    [Range(0f, 2f)] public float stiffness;

    public WheelFrictionCurve ToWheelFrictionCurve()
    {
        WheelFrictionCurve curve = new WheelFrictionCurve();
        curve.extremumSlip = extremumSlip;
        curve.extremumValue = extremumValue;
        curve.asymptoteSlip = asymptoteSlip;
        curve.asymptoteValue = asymptoteValue;
        curve.stiffness = stiffness;
        return curve;
    }
}

[CreateAssetMenu(fileName = "New Surface Profile", menuName = "Vehicle/Surface Profile")]
public class TerrainHandler : ScriptableObject
{
    [Header("Surface Type")]
    public string surfaceName;

    [Header("Friction Settings")]
    public WheelFrictionData forwardFriction;
    public WheelFrictionData sidewaysFriction;

    [Header("Smoke Settings")]
    public Color smokeColor;
    public float startLifeTime;
    public float startSpeed;
    public float startSize;
    public float slipThreshold;
    public float maxEmissionRate;

    [Header("Skid Material")]
    public Material material;
}
