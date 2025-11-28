using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuTextHighlighter : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI label;

    [Header("Colors")]
    public Color normalColor = new Color(156, 156, 156, 255);
    public Color selectedColor = new Color(1f, 1f, 1f, 1f);

    [Header("Size")]
    public int normalSize = 56;
    public int selectedSize = 60;

    void Reset()
    {
        label = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        label.color = selectedColor;
        label.fontSize = selectedSize;
    }

    public void OnDeselect(BaseEventData baseEventData)
    {
        label.color = normalColor;
        label.fontSize = normalSize;
    }
}
