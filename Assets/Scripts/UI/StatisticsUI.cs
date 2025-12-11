using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsUI : MonoBehaviour
{
    private Stats stats;
    public TextMeshProUGUI distanceTraveled, totalAttempts, mostPlayedStage, mostUsedCar;
    [SerializeField] private Button goBackButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        await HandleSceneLoaded();

        goBackButton.onClick.AddListener(OnGoBackPerformed);
        distanceTraveled.text = stats.DistanceTraveled >= 1000 ? (stats.DistanceTraveled / 1000).ToString("0.#") + " km" : stats.DistanceTraveled.ToString() + " m";
        totalAttempts.text = stats.TotalAttempts.ToString();
        mostPlayedStage.text = stats.MostPlayedStage == null ? "Not played" : GameManager.Instance.availableStages[(int)stats.MostPlayedStage].Title;
        mostUsedCar.text = stats.MostUsedCar == null ? "Not set" : GameManager.Instance.availableCars[(int)stats.MostUsedCar].Model;
    }

    public void OnGoBackPerformed()
    {
        GameManager.Instance.LoadScene("MainMenu");
    }
    private async Task HandleSceneLoaded()
    {
        stats = await DatabaseController.Instance.GetStats();
    }
}
