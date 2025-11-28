using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    private PauseManager pauseManager;

    private GameObject lastItemSelected;

    void Start()
    {
        pauseManager = FindFirstObjectByType<PauseManager>();
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastItemSelected);
        }
        else
        {
            lastItemSelected = EventSystem.current.currentSelectedGameObject;
        }
    }

    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(FindFirstObjectByType<Button>().gameObject);
    }

    public void onContinueClickPerformed()
    {
        pauseManager.TogglePause();
    }

    public void onResetClickPerformed()
    {
        GameManager.Instance.LoadScene(GameManager.Instance.selectedStage.SceneName);
    }

    public void onMainMenuClickPerformed()
    {
        GameManager.Instance.LoadScene("MainMenu");
    }
}
