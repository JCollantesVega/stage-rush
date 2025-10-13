using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public string StageCurrentTime { get { return formatCurrentTime(); } }
    private int timeInMs;
    private bool startedTimer = false;
    public bool StartedTimer {get { return startedTimer; }}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        newTimeStamp();
    }

    private void newTimeStamp()
    {
        if (startedTimer)
        {
            timeInMs += (int)(Time.deltaTime*1000);
        }
    }

    public void startTimer()
    {
        if (!startedTimer)
        {
            startedTimer = true;
            timeInMs = 0;
        }

    }

    public void stopTimer()
    {
        if(startedTimer)
        {
            startedTimer = false;
        }
    }

    private string formatCurrentTime()
    {
        int min = (int)(timeInMs / 60000);
        int sec = (int)((timeInMs % 60000) / 1000);
        int ms = (int)(timeInMs % 1000);

        return $"{min:D2}:{sec:D2}.{ms:D3}";
    }
}
