using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance { get; private set; }

    public string StageCurrentTime { get { return FormatCurrentTime(); } }
    private int timeInMs;
    private bool startedTimer = false;
    public int[] sectorTimes = new int[3];
    private float lastSectorTime;
    public bool StartedTimer { get { return startedTimer; } }

    public event Action<StageCompletedArgs> OnStageCompleted;

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

    }

    void Update()
    {
        if (startedTimer)
        {
            NewTimeStamp();
        }
    }
    
    public void StartTimer()
    {
        if (!startedTimer)
        {
            startedTimer = true;
            timeInMs = 0;
            lastSectorTime = 0;

            for(int i = 0; i< sectorTimes.Length; i++)
            {
                sectorTimes[i] = 0;
            }
        }
    }

    private void NewTimeStamp()
    {
        if (startedTimer)
        {
            timeInMs += (int)(Time.deltaTime * 1000);
        }
    }

    public void StopTimer()
    {
        if (startedTimer)
        {
            startedTimer = false;
            LapTime lapTime = new LapTime(sectorTimes);

            var StageCompletedArgs = new StageCompletedArgs(ValidateCheckPoints(), lapTime, GameManager.Instance.selectedCar.name, SceneManager.GetActiveScene().name);

            OnStageCompleted?.Invoke(StageCompletedArgs);
        }
    }

    private bool ValidateCheckPoints()
    {
        bool validTime = true;

        List<bool> times = new List<bool>();

        for (int i = 0; i < CheckPointList.Instance.checkPointSingles.Count() && validTime; i++)
        {

            times.Add(CheckPointList.Instance.checkPointSingles[i].completed);

            if (!CheckPointList.Instance.checkPointSingles[i].completed)
            {
                validTime = false;
            }
        }
        return validTime;
    }
    
    public void RegisterSectorTime(int sectorIndex)
    {
        if (!startedTimer) return;
        if (sectorIndex < 1 || sectorIndex > sectorTimes.Length) return;

        int currentSectorTime = (int)(timeInMs - lastSectorTime);
        sectorTimes[sectorIndex - 1] = currentSectorTime;

        lastSectorTime = timeInMs;

    }

    public string FormatTime(int msTime)
    {
        int min = msTime / 60000;
        int sec = msTime % 60000 / 1000;
        int ms = msTime % 1000;
        return $"{min:D2}:{sec:D2}.{ms:D3}";
    }

    private string FormatCurrentTime()
    {
        int min = timeInMs / 60000;
        int sec = timeInMs % 60000 / 1000;
        int ms = timeInMs % 1000;

        return $"{min:D2}:{sec:D2}.{ms:D3}";
    }
}


public class StageCompletedArgs
{
    public bool IsValidTime;
    public LapTime LapTime;
    public string SelectedCar;
    public string SelectedStage;

    public StageCompletedArgs(bool isValidTime, LapTime lapTime, string selectedCar, string selectedStage)
    {
        IsValidTime = isValidTime;
        LapTime = lapTime;
        SelectedCar = selectedCar;
        SelectedStage = selectedStage;
    }
}


public class LapTime
{
    public int[] sectorTimes;
    public int totalTime;

    public LapTime(int[] sectorTimes)
    {
        this.sectorTimes = new int[3] { sectorTimes[0], sectorTimes[1], sectorTimes[2] };

        foreach (int sector in sectorTimes)
        {
            totalTime += sector;
        }
    }

    public string FormatTime(int msTime)
    {
        int min = msTime / 60000;
        int sec = msTime % 60000 / 1000;
        int ms = msTime % 1000;
        return $"{min:D2}:{sec:D2}.{ms:D3}";
    }

    public override string ToString()
    {
        int min = totalTime / 60000;
        int sec = totalTime % 60000 / 1000;
        int ms = totalTime % 1000;

        return $"{min:D2}:{sec:D2}.{ms:D3}. Sector 1: {FormatTime(sectorTimes[0])}.Sector 2: {FormatTime(sectorTimes[1])}.Sector 3: {FormatTime(sectorTimes[2])}";
    }
}