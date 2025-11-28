using TMPro;
using UnityEngine;
using Postgrest.Attributes;

public class LeaderboardRegistry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI positionLabel, usernameLabel, timeLabel, carModelLabel;


    public void Setup(int position, string userName, int time, string carModel, bool isUser)
    {
        positionLabel.text = position.ToString();
        usernameLabel.text = userName;
        timeLabel.text = FormatTime(time);
        carModelLabel.text = carModel;
    }


    public string FormatTime(int msTime)
    {
        int min = msTime / 60000;
        int sec = msTime % 60000 / 1000;
        int ms = msTime % 1000;
        return $"{min:D2}:{sec:D2}.{ms:D3}";
    }
}


[Table("get_stage_leaderboard")]
public class LeaderboardEntry : Postgrest.Models.BaseModel
{
    public string userName { get; set; }
    public int stage_time { get; set; }
    public string car_model { get; set; }

    public string FormatTime(int msTime)
    {
        int min = msTime / 60000;
        int sec = msTime % 60000 / 1000;
        int ms = msTime % 1000;
        return $"{min:D2}:{sec:D2}.{ms:D3}";
    }

    public override string ToString()
    {
        return $"Username: {userName}, time: {FormatTime(stage_time)}, car: {car_model}";
    }
}

