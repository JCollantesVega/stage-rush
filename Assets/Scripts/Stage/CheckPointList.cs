using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

public class CheckPointList : MonoBehaviour
{
    public static CheckPointList Instance { get; private set; }

    [HideInInspector]
    public CheckPointSingle[] checkPointSingles { get; private set; }

    private int lastCheckpointPassed = -1;

    public Action<Direction, int> PaceNoteHandler;
    

    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        checkPointSingles = gameObject.GetComponentsInChildren<CheckPointSingle>();
    }

    public void playerThroughtCheckpoint(CheckPointSingle checkPointSingle)
    {

        if(checkPointSingle.orderIndex == lastCheckpointPassed + 1)
        {
            lastCheckpointPassed = checkPointSingle.orderIndex;
            checkPointSingle.completed = true;
            OnValidateCheckpoint(checkPointSingle);
        }

    }

    private void OnValidateCheckpoint(CheckPointSingle checkPoint)
    {
        if (checkPoint.isStartTrigger && !RaceManager.Instance.StartedTimer)
        {
            RaceManager.Instance.StartTimer();
        }

        if(checkPoint.sectorIndex > 0)
        {
            RaceManager.Instance.RegisterSectorTime(checkPoint.sectorIndex);
        }

        //Debug.Log(checkPoint.isTurnIndicator);

        if(checkPoint.isTurnIndicator)
        {
            PaceNoteHandler?.Invoke(checkPoint.turnDirection, checkPoint.turnSeverity);
            Debug.Log($"{checkPoint.turnDirection} {checkPoint.turnSeverity}");
        }
        
        if(checkPoint.isFinnishTrigger && RaceManager.Instance.StartedTimer)
        {
            RaceManager.Instance.StopTimer();
        }
        
    }


    public int GetCompletedCheckpoints()
    {
        int completedCheckpoints = 0;
        foreach(CheckPointSingle checkPoint in checkPointSingles)
        {
            if(!checkPoint.completed)
            {
                return completedCheckpoints;
            }
            completedCheckpoints++;
        }

        return completedCheckpoints;
    }

}
