using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaceNoteUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image curveImage;
    public TextMeshProUGUI severity;

    [Header("Animation Settings")]
    public float enterDuration = 0.35f;
    public float holdDuration = 1f;
    public float exitDuration = 0.35f;

    // These will be computed automatically
    private Vector2 startPosition;
    private Vector2 stayPosition;
    private Vector2 exitPosition;

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();

        // --- ANCLAR SIEMPRE AL BORDE INFERIOR IZQUIERDO ---
        rect.anchorMin = new Vector2(.5f, .5f);
        rect.anchorMax = new Vector2(.5f, .5f);
        rect.pivot = new Vector2(.5f, .5f);

        ResetPositions();
    }

    void ResetPositions()
    {
        startPosition = new Vector2(300, 250); // Entra desde abajo
        stayPosition = new Vector2(0,250);    // Posición visible
        exitPosition = new Vector2(-300, 250);  // Sale por la izquierda
    }


    public void Setup(Sprite sprite, int severity)
    {
        curveImage.sprite = sprite;
        this.severity.text = severity.ToString();

        // Colocar inicialmente fuera de pantalla
        rect.anchoredPosition = startPosition;
    }

    public void Play()
    {
        StopAllCoroutines();
        StartCoroutine(AnimationRoutine());
    }

    IEnumerator AnimationRoutine()
    {
        // Entrada desde abajo
        yield return Move(rect, startPosition, stayPosition, enterDuration);

        // Mantener
        yield return new WaitForSeconds(holdDuration);

        // Salida por la izquierda
        yield return Move(rect, stayPosition, exitPosition, exitDuration);

        Destroy(gameObject);
    }

    IEnumerator Move(RectTransform obj, Vector2 from, Vector2 to, float time)
    {
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / time);
            obj.anchoredPosition = Vector2.Lerp(from, to, p);
            yield return null;
        }

        obj.anchoredPosition = to;
    }
}
