using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas inGameUI;
    [SerializeField] private Canvas pauseUI;
    [SerializeField] private Canvas completedStageUI;

    private RaceManager raceManager;
    private PauseManager pauseManager;

    void Awake()
    {
        raceManager = FindFirstObjectByType<RaceManager>(); 
        pauseManager = FindFirstObjectByType<PauseManager>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (raceManager != null)
            raceManager.OnStageCompleted -= StageCompletedHandler;

        if (pauseManager != null)
            pauseManager.OnGamePaused -= GamePausedHandler;
    }


    void Start()
    {
        

        if (pauseUI.enabled)
        {
            pauseUI.enabled = false;
        }

        if (completedStageUI.enabled)
        {
            completedStageUI.enabled = false;
        }
        
        if(!inGameUI.enabled)
        {
            inGameUI.enabled = true;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (pauseManager != null)
        {
            pauseManager.OnGamePaused -= GamePausedHandler;
            pauseManager.OnGamePaused += GamePausedHandler;
        }

        if(raceManager != null)
        {
            RaceManager.Instance.OnStageCompleted -= StageCompletedHandler;
            RaceManager.Instance.OnStageCompleted += StageCompletedHandler;
        }
    }

    void GamePausedHandler(bool isPaused)
    {
        if (isPaused)
        {
            //inGameUI.enabled = false;
            pauseUI.enabled = true;
        }
        else
        {
            //inGameUI.enabled = true;
            pauseUI.enabled = false;
        }
    }

    void StageCompletedHandler(StageCompletedArgs args)
    {
        Time.timeScale = .2f;

        completedStageUI.enabled = true;
        CompletedStageUI ui = completedStageUI.GetComponent<CompletedStageUI>();

        ui.TimeLabel.text = $"Tiempo: {args.LapTime.FormatTime(args.LapTime.totalTime)}";
    }


}
