using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelSelectorMenu : MonoBehaviour
{
    public GameObject levelButtonPrefab;
    public Transform contentParent;
    private GameObject lastItemSelected;

    void Start()
    {
        GenerateLevelButtons();
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

    void GenerateLevelButtons()
    {

        for(int i = 0; i < GameManager.Instance.availableStages.Count; i++)
        {
            StageData stageData = GameManager.Instance.availableStages[i];

            GameObject newButton = Instantiate(levelButtonPrefab, contentParent);
            LevelButtonUI buttonUI = newButton.GetComponent<LevelButtonUI>();
            buttonUI.Setup(stageData.Title, i);

            if (i == 0)
            {
                EventSystem.current.SetSelectedGameObject(buttonUI.gameObject);
                lastItemSelected = buttonUI.gameObject;
            }
        }

    }
}
