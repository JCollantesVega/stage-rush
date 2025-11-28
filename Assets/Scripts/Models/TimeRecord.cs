using System.Text.Json.Serialization;
using Postgrest.Attributes;
using Postgrest.Models;

[Table("times")]
public class TimeRecord : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("sector_1")]
    public int Sector_1 { get; set; }

    [Column("sector_2")]
    public int Sector_2 { get; set; }

    [Column("sector_3")]
    public int Sector_3 { get; set; }

    [Column("car_id")]
    public int Car_id { get; set; }

    [Column("stage_id")]
    public int Stage_id { get; set; }

    [Column("user_uuid")]
    public string User_uuid { get; set; }


    public int GetTotalTime()
    {
        return Sector_1 + Sector_2 + Sector_3;
    }

    public override string ToString()
    {
        return $"id: {Id}, Car id: {Car_id}, stage id: {Stage_id}, total time: {GetTotalTime()}";
    }
}
