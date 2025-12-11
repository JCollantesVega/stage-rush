using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CompletedStageUI : MonoBehaviour
{
    public TextMeshProUGUI TimeLabel;
    public LeaderBoardTable leaderBoardContainer;
    private GameObject lastItemSelected;

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

    public void onResetClickPerformed()
    {
        GameManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void onMainMenuClickPerformed()
    {
        GameManager.Instance.LoadScene("MainMenu");
    }

    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(FindFirstObjectByType<Button>().gameObject);
        leaderBoardContainer.ClearLeaderboard();
        leaderBoardContainer.GenerateLeaderboard(GameManager.Instance.selectedStage.Id);
    }
}
