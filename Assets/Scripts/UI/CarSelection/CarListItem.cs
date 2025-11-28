using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    private int index;
    private CarSelectorMenu menu;
    private Button button;

    public void Setup(string name, int index, CarSelectorMenu menu)
    {
        this.index = index;
        this.menu = menu;
        label.text = name;

        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        // Para hover con ratón
        var trigger = gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        var entry = new UnityEngine.EventSystems.EventTrigger.Entry();
        entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        //entry.callback.AddListener(_ => menu.OnCarHovered(index));
        trigger.triggers.Add(entry);
    }

    private void OnClick()
    {
        //menu.OnCarClicked(index);
    }
}
