using System.Collections;
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
            pauseUI.gameObject.SetActive(false);
        }

        if (completedStageUI.enabled)
        {
            completedStageUI.gameObject.SetActive(false);
        }
        
        if(!inGameUI.enabled)
        {
            inGameUI.gameObject.SetActive(true);
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
            pauseUI.gameObject.SetActive(true);
        }
        else
        {
            pauseUI.gameObject.SetActive(false);
        }
    }

    void StageCompletedHandler(StageCompletedArgs args)
    {
        StartCoroutine(StageCompletedProcess(args));
    }

    private IEnumerator StageCompletedProcess(StageCompletedArgs args)
    {
        pauseManager.pauseAllowed = false;

        Time.timeScale = .2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        yield return new WaitForSecondsRealtime(2);

        Time.timeScale = 0;
        CarController.Instance.carFX.engineSound.Pause();
        completedStageUI.gameObject.SetActive(true);
        CompletedStageUI ui = completedStageUI.GetComponent<CompletedStageUI>();
        inGameUI.gameObject.SetActive(false);

        ui.TimeLabel.text = args.IsValidTime ? $"Tiempo: {args.LapTime.FormatTime(args.LapTime.totalTime)}" : $"Tiempo no válido: ({args.LapTime.totalTime})";
    }


}
