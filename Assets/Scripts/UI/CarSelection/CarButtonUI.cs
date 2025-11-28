using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CarButtonUI : MonoBehaviour, ISelectHandler, IPointerClickHandler
{
    [SerializeField]private TextMeshProUGUI carText;

    private Button button;
    private int carIndex;

    [SerializeField] private Transform meshPosition;
    public static GameObject currentPreview;


    void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
                ConfirmCar();

            if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
                ConfirmCar();
        }
    }

    public void Setup(string carName, int carButtonIndex, Transform meshPosition)
    {
        carText.text = carName;
        carIndex = carButtonIndex;
        this.meshPosition = meshPosition;

        button = GetComponent<Button>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        SelectCar();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(GameManager.Instance.selectedCar== GameManager.Instance.availableCars[carIndex])
        {
            Debug.Log("Confirmar");
            ConfirmCar();
            return;
        }


        SelectCar();
    }


    private void SelectCar()
    {
        GameManager.Instance.SelectCar(carIndex);
        LoadCar();
    }

    private void ClearPreview()
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }
    }

    private void ConfirmCar()
    {
        GameManager.Instance.LoadScene("LevelSelector");
    }

    private void LoadCar()
    {
        ClearPreview();

        GameObject carSelected = GameManager.Instance.previewCars[carIndex];

        currentPreview = Instantiate(carSelected, meshPosition.position, meshPosition.rotation);

        currentPreview.SetActive(true);
    }

    
}
