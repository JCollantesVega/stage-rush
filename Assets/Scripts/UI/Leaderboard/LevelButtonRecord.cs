using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonRecord : MonoBehaviour
{
    public int levelIndex {get; private set;}
    public TextMeshProUGUI levelNameText;
    public Button button;

    public void Setup(string levelTitle, int levelIndex)
    {
        this.levelIndex = levelIndex;
        levelNameText.text = levelTitle;
        button = GetComponent<Button>();
    }
}
