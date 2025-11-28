using TMPro;
using UnityEngine;

public class PlaygroundUI : MonoBehaviour
{
    public TextMeshProUGUI speedLabel;
    public TextMeshProUGUI rpmLabel;
    public TextMeshProUGUI gearLabel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        speedLabel.text = $"Speed: {CarController.Instance.speed*3.6:0.##} km/h ({CarController.Instance.speed:0.##} m/s)";
        rpmLabel.text = $"RPM: {CarController.Instance.RPM:#,000}";
        gearLabel.text = $"Gear: {(CarController.Instance.gearState == GearState.Neutral ? "N" : CarController.Instance.currentGear+1)}";
    }
}
