using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    Regex regex = new Regex("[0-9]+_[A-Za-z0-9]+", RegexOptions.IgnoreCase);

    public static GameManager Instance{ get; private set; }

    public List<CarData> availableCars;
    public List<StageData> availableStages;
    public List<GameObject> previewCars;

    public CarData selectedCar;
    public StageData selectedStage;

    [HideInInspector] public Transform spawnPoint;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            AuthController.Instance.OnLoginSuccess += OnLoginSuccess;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        AuthController.Instance.OnLoginSuccess -= OnLoginSuccess;

        RaceManager raceManager = FindFirstObjectByType<RaceManager>();
        if(raceManager != null)
        {
            RaceManager.Instance.OnStageCompleted -= HandleStageCompleted;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject carPrefab;
        PauseManager pauseManager = FindFirstObjectByType<PauseManager>();


        if(regex.IsMatch(scene.name))
        {
            InputManager.Instance.EnableGameplay();

            if(pauseManager != null) pauseManager.pauseAllowed = true;
        }
        else
        {
            InputManager.Instance.EnableUI();

            if(pauseManager != null) pauseManager.pauseAllowed = false;
        }

        if (selectedCar != null && spawnPoint == null)
        {
            GameObject foundSpawn = GameObject.FindWithTag("PlayerSpawn");
            if (foundSpawn != null)
            {
                spawnPoint = foundSpawn.transform;
            }
        }

        if (selectedCar != null && spawnPoint != null)
        {
            carPrefab = selectedCar.Prefab;
            Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        RaceManager raceManager = FindFirstObjectByType<RaceManager>();
        if(raceManager != null)
        {
            RaceManager.Instance.OnStageCompleted += HandleStageCompleted;
        }
    }

    public void SelectCar(int index)
    {
        if (index >= 0 && index < availableCars.Count)
        {
            selectedCar = availableCars[index];
        }
    }

    public void SelectScene(int index)
    {
        if (index >= 0 && index < availableStages.Count)
        {
            selectedStage = availableStages[index];
        }

        LoadScene(selectedStage.SceneName);
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }
    
    private async void HandleStageCompleted(StageCompletedArgs args)
    {
        if (args.IsValidTime)
        {
            await DatabaseController.Instance.SaveTime(args.LapTime.sectorTimes[0], args.LapTime.sectorTimes[1], args.LapTime.sectorTimes[2]);

            await DatabaseController.Instance.GetLeaderboard(selectedStage.Id);
        }
        else
        {
            Debug.Log("Tiempo no válido");
        }
    }


    private void OnLoginSuccess()
    {
        LoadScene("MainMenu");
    }
}
