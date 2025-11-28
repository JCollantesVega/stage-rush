using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI speedText;

    [SerializeField] private Image RPMFill, progressFill;

    private float currentRPM, maxRPM;

    private float currentProgress, maxProgress;
    
    public PaceNoteData paceNotes;
    [SerializeField]private GameObject notePrebaf;
    [SerializeField]private RectTransform uiParent;

    void Start()
    {
        maxRPM = (int)CarController.Instance.redLine;
        maxProgress = CheckPointList.Instance.checkPointSingles.Count();

        CheckPointList.Instance.PaceNoteHandler += OnPaceNoteShow;

    }

    void OnDestroy()
    {
        CheckPointList.Instance.PaceNoteHandler -= OnPaceNoteShow;
    }

    // Update is called once per frame
    void Update()
    {
        currentRPM = CarController.Instance.RPM;
        currentProgress = CheckPointList.Instance.GetCompletedCheckpoints();
        timeText.text = RaceManager.Instance.StageCurrentTime;
        speedText.text = ((int)(CarController.Instance.speed*3.6f)).ToString();

        RPMFill.fillAmount = Mathf.Clamp01(currentRPM / maxRPM);
        progressFill.fillAmount = Mathf.Clamp01(currentProgress / maxProgress);
    }

    void OnPaceNoteShow(Direction direction, int severity)
    {
        Sprite sprite = paceNotes.GetSprite(direction, severity);

        var note = Instantiate(notePrebaf, uiParent);
        var ui = note.GetComponent<PaceNoteUI>();

        ui.Setup(sprite, severity);
        ui.Play();

        
    }
}
