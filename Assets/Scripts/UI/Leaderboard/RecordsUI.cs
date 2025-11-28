using UnityEngine;
using UnityEngine.UI;


public class RecordsUI : MonoBehaviour
{
    public LeaderBoardTable leaderBoard;
    public Button[] levelButtons;

    public GameObject levelButtonPrefab;
    public Transform buttonParentLocation;

    void Start()
    {

        foreach(StageData stage in GameManager.Instance.availableStages)
        {
            GameObject newButton = Instantiate(levelButtonPrefab, buttonParentLocation);
            LevelButtonRecord buttonUI = newButton.GetComponent<LevelButtonRecord>();
            buttonUI.Setup(stage.Title, stage.Id);
            buttonUI.button.onClick.AddListener(() => OnLevelSelected(stage.Id));
        }
    }

    void OnLevelSelected(int levelId)
    {
        leaderBoard.ClearLeaderboard();
        leaderBoard.GenerateLeaderboard(levelId);
    }

}