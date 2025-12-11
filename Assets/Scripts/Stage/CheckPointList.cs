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

    public int lastCheckpointPassed = -1;

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
        if (checkPointSingle.completed) return;

        if(checkPointSingle.orderIndex == lastCheckpointPassed + 1)
        {
            lastCheckpointPassed = checkPointSingle.orderIndex;
            checkPointSingle.completed = true;
            OnValidateCheckpoint(checkPointSingle, true);
        }
        else
        {
            int[] ommitedCheckPoints = GetOmittedCheckPoints(checkPointSingle.orderIndex);
            int totalPenalty = GetPenaltyTime(ommitedCheckPoints);
            Debug.Log($"Sanción total: {totalPenalty}");
            if(totalPenalty >= 8000)
            {
                MovePlayerToLastCheckpoint();
            }
            else
            {
                RaceManager.Instance.penalizedTime+=totalPenalty;
                ValidateOmittedCheckPoints(ommitedCheckPoints);
                lastCheckpointPassed = checkPointSingle.orderIndex;
                checkPointSingle.completed = true;
            }
        }

    }

    int[] GetOmittedCheckPoints(int currentIndex)
    {
        List<int> indexesList = new List<int>();
        for(int i = lastCheckpointPassed+1; i < currentIndex; i++)
        {
            Debug.Log($"Skipeado nº {checkPointSingles[i].orderIndex}");
            indexesList.Add(checkPointSingles[i].orderIndex);
        }
        return indexesList.ToArray();
    }

    void ValidateOmittedCheckPoints(int[] indexes)
    {
        for(int i = 0; i < indexes.Length; i++)
        {
            OnValidateCheckpoint(checkPointSingles[indexes[i]], false);
            checkPointSingles[indexes[i]].completed = true;
        }
    }

    int GetPenaltyTime(int[] omittedCheckPoints)
    {
        int totalPenalty = 0;
        for(int i = 0; i < omittedCheckPoints.Length; i++)
        {
            totalPenalty += checkPointSingles[omittedCheckPoints[i]].penaltyTime;
        }

        return totalPenalty;
    }

    void MovePlayerToLastCheckpoint()
    {
        Vector3 checkPointPosition = checkPointSingles[lastCheckpointPassed].transform.position;
        Quaternion checkPointRotation = checkPointSingles[lastCheckpointPassed].transform.rotation;
        
        RaceManager.Instance.RespawnCar(checkPointPosition, checkPointRotation);
    }

    private void OnValidateCheckpoint(CheckPointSingle checkPoint, bool showPaceNote)
    {
        if (checkPoint.isStartTrigger && !RaceManager.Instance.StartedTimer)
        {
            RaceManager.Instance.StartTimer();
        }

        if(checkPoint.sectorIndex > 0)
        {
            RaceManager.Instance.RegisterSectorTime(checkPoint.sectorIndex);
        }


        if(checkPoint.isTurnIndicator && showPaceNote)
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
