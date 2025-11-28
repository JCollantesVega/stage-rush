using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CarSelectorMenu : MonoBehaviour
{
    [SerializeField] private GameObject carButtonPrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private Button continueButton;
    [SerializeField] private Transform meshPosition;

    private GameObject lastItemSelected;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateCarButtons();
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
    void GenerateCarButtons()
    {
        int carCount = GameManager.Instance.availableCars.Count;
        for (int i = 0; i < carCount; i++)
        {
            string carName = GameManager.Instance.availableCars[i].Model;
            GameObject newButton = Instantiate(carButtonPrefab, contentParent);
            CarButtonUI buttonUI = newButton.GetComponent<CarButtonUI>();
            buttonUI.Setup(carName, i, meshPosition);

            if(i == 0)
            {
                EventSystem.current.SetSelectedGameObject(buttonUI.gameObject);
                lastItemSelected = buttonUI.gameObject;
            }
        }
    }
    // void checkCarSelected()
    // {
    //     if (GameManager.Instance.selectedCar != null)
    //     {
    //         continueButton.enabled = true;
    //     }
    //     else
    //     {
    //         continueButton.enabled = false;
    //     }
    // }
    void OnClickCarSelect()
    {
        GameManager.Instance.LoadScene("LevelSelector");
    }
}