using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class CompletedStageUI : MonoBehaviour
{
    public TextMeshProUGUI TimeLabel;

    public void onResetClickPerformed()
    {
        GameManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void onMainMenuClickPerformed()
    {
        GameManager.Instance.LoadScene("MainMenu");
    }
}
