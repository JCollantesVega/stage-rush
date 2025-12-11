using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    [Header("Time label")]
    public TextMeshProUGUI timeText;
    public Image penaltyContainer;
    public TextMeshProUGUI penaltyText;

    [Header("Speed label")]
    public TextMeshProUGUI speedText;

    [Header("Gearbox label")]
    public TextMeshProUGUI gearText;
    [SerializeField] private Image RPMFill;
    private float currentRPM, maxRPM;

    [Header("Stage Progress label")]
    [SerializeField] private Image progressFill;
    private float currentProgress, maxProgress;
    

    [Header("Pace notes label")]
    public PaceNoteData paceNotes;
    [SerializeField]private GameObject notePrebaf;
    [SerializeField]private RectTransform uiParent;

    [Header("Traffic light label")]
    [SerializeField] private Sprite redLight, greenLight;
    [SerializeField] private Image light_1, light_2, light_3;
    [SerializeField] private GameObject trafficLightParent;

    void Start()
    {
        maxRPM = (int)CarController.Instance.redLine;
        maxProgress = CheckPointList.Instance.checkPointSingles.Count();

        CheckPointList.Instance.PaceNoteHandler += OnPaceNoteShow;

        light_1.sprite = redLight;
        light_2.sprite = redLight;
        light_3.sprite = redLight;

        light_1.enabled = true;
        light_2.enabled = true;
        light_3.enabled = true;

    }

    void OnDestroy()
    {
        CheckPointList.Instance.PaceNoteHandler -= OnPaceNoteShow;
    }

    // Update is called once per frame
    void Update()
    {
        HandleTrafficLight();

        currentRPM = CarController.Instance.RPM;
        currentProgress = CheckPointList.Instance.GetCompletedCheckpoints();
        timeText.text = RaceManager.Instance.StageCurrentTime;

        penaltyContainer.enabled = RaceManager.Instance.penalizedTime > 0f;
        penaltyText.enabled = RaceManager.Instance.penalizedTime > 0f;

        penaltyText.text = $"+{RaceManager.Instance.FormatTime(RaceManager.Instance.penalizedTime)}";
        speedText.text = ((int)(CarController.Instance.speed*3.6f)).ToString();

        RPMFill.fillAmount = Mathf.Clamp01(currentRPM / maxRPM);
        progressFill.fillAmount = Mathf.Clamp01(currentProgress / maxProgress);
        gearText.text = $"{(CarController.Instance.gearState == GearState.Neutral ? "N" : CarController.Instance.currentGear+1)}";
    }

    void OnPaceNoteShow(Direction direction, int severity)
    {
        Sprite sprite = paceNotes.GetSprite(direction, severity);

        var note = Instantiate(notePrebaf, uiParent);
        var ui = note.GetComponent<PaceNoteUI>();

        ui.Setup(sprite, severity);
        ui.Play();

        
    }

    void HandleTrafficLight()
    {
        if(RaceManager.Instance.startCounter == 3)
        {
            light_3.enabled = false;
        }

        if(RaceManager.Instance.startCounter == 2)
        {
            light_2.enabled = false;
        }
        
        if(RaceManager.Instance.startCounter == 1)
        {
            light_1.enabled = false;
        }

        if(RaceManager.Instance.startCounter == 0)
        {
            light_1.sprite = greenLight;
            light_2.sprite = greenLight;
            light_3.sprite = greenLight;

            light_1.enabled = true;
            light_2.enabled = true;
            light_3.enabled = true;
        }

        if(RaceManager.Instance.startCounter == -1)
        {
            trafficLightParent.SetActive(false);
        }
    }
}
