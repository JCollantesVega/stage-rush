using UnityEngine;

[CreateAssetMenu(fileName = "CarData", menuName = "Game/CarData")]
public class CarData : ScriptableObject
{
    public int Id;
    public string Model;
    public GameObject Prefab;
}
