using UnityEngine;

public class DestroyBarrier : MonoBehaviour
{
    public GameObject destroyedPrefab;
    public BoxCollider barrierCollider;


    void OnTriggerEnter(Collider collider)
    {
        Instantiate(destroyedPrefab, barrierCollider.transform.position, barrierCollider.transform.rotation);

        Destroy(gameObject);

    }
}
