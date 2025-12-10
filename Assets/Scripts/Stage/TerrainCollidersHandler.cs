using UnityEngine;

public class TerrainCollidersHandler : MonoBehaviour
{
    void Awake()
    {
        GetComponent<TerrainCollider>().enabled = false;
        GetComponent<TerrainCollider>().enabled = true;
    }
}
