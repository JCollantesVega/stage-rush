using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonUI : MonoBehaviour
{
    public TextMeshProUGUI levelNameText;
    private int sceneIndex;

    private Button button;

    public void Setup(string sceneTitle, int sceneIndex)
    {
        this.sceneIndex = sceneIndex;
        levelNameText.text = sceneTitle;

        button = GetComponent<Button>();

        button.onClick.AddListener(OnClick);

    }
    

    public void OnClick()
    {
        GameManager.Instance.SelectScene(sceneIndex);
    }
}
