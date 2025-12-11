using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Postgrest.Attributes;
using Postgrest.Models;

[Table("user_stats")]
public class Stats : BaseModel
{
    [PrimaryKey("user_uuid")]
    public string User_uuid{get; set;}

    [Column("distance_traveled")]
    public int DistanceTraveled{get; set;}

    [Column("total_attempts")]
    public int TotalAttempts{get; set;}

    [Column("most_played_stage_id")]
    public int? MostPlayedStage {get; set;}

    [Column("most_used_car_id")]
    public int? MostUsedCar{get; set;}

    [Column("stage_usage")]
    public Dictionary<int, int> StagePlayed{get; set;}

    [Column("car_usage")]
    public Dictionary<int, int> CarUsage{get; set;}

    
}

// public class Stats
// {
//     public string User_uuid{get; set;}
//     public int DistanceTraveled{get; set;}
//     public int TotalAttempts{get; set;}
//     public int? MostPlayedStage {get; set;}
//     public int? MostUsedCar{get; set;}
//     public Dictionary<int, int> StagePlayed{get; set;}
//     public Dictionary<int, int> CarUsage{get; set;}

//     public override string ToString()
//     {
//         return $"User UUID: {User_uuid}\n" +
//             $"Distance Traveled: {DistanceTraveled}\n" +
//             $"Total Attempts: {TotalAttempts.ToString() ?? "Not set"}\n" +
//             $"Most Played Stage: {MostPlayedStage.ToString() ?? "Not set"}\n" +
//             $"Most Used Car: {MostUsedCar}\n" +
//             $"Stage Usage: {(StagePlayed != null ? string.Join(", ", StagePlayed.Select(kv => $"{kv.Key}: {kv.Value}")) : "None")}\n" +
//             $"Car Usage: {(CarUsage != null ? string.Join(", ", CarUsage.Select(kv => $"{kv.Key}: {kv.Value}")) : "None")}";
//     }

//     public StatsRaw ToRaw()
//     {
//         return new StatsRaw
//         {
//             User_uuid = this.User_uuid,
//             DistanceTraveled = this.DistanceTraveled,
//             TotalAttempts = this.TotalAttempts,
//             MostPlayedStage = this.MostPlayedStage,
//             MostUsedCar = this.MostUsedCar,
//             StagePlayed = this.StagePlayed,
//             CarUsage = this.CarUsage
//         };
//     }

//     public static Stats MapFromRaw(StatsRaw r)
//     {
//         return new Stats
//         {
//             User_uuid = r.User_uuid,
//             DistanceTraveled = r.DistanceTraveled,
//             TotalAttempts = r.TotalAttempts,
//             MostPlayedStage = r.MostPlayedStage,
//             MostUsedCar = r.MostUsedCar,
//             StagePlayed = r.StagePlayed,
//             CarUsage = r.CarUsage
//         };
//     }

// }