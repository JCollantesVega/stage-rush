using UnityEngine;

public class CheckPointSingle : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    public bool isStartTrigger;
    public bool isFinnishTrigger;
    public bool isTurnIndicator = false;
    public int sectorIndex;
    public int orderIndex;

    [Header("Turn indicator")]
    public Direction turnDirection;
    [Range(1, 6)]
    public int turnSeverity;


    [HideInInspector]
    public bool completed;

    void Start()
    {
        completed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CheckPointList.Instance.playerThroughtCheckpoint(this);
        }
    }

    private void OnValidate()
    {
        if (isStartTrigger && isFinnishTrigger) //evitar que un checkpoint sean de ambos tipos a la vez
        {
            isFinnishTrigger = false;
        }
    }
}


public enum Direction{Left, Right}
