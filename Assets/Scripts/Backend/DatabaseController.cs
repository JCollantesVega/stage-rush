using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class DatabaseController : MonoBehaviour
{
    public static DatabaseController Instance{get; private set;}
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async Task SaveTime(int sector1, int sector2, int sector3)
    {
        var currentUser = SupabaseManager.Instance.Supabase.Auth.CurrentSession?.User;
        // Clear the schema cache before operations
        

        if(currentUser == null)
        {
            Debug.LogError("No hay usuario logeado");
            return;
        }


        TimeRecord timeRecord = new TimeRecord
        {
            Sector_1 = sector1,
            Sector_2 = sector2,
            Sector_3 = sector3,
            Car_id = GameManager.Instance.selectedCar.Id,
            Stage_id = GameManager.Instance.selectedStage.Id,
            User_uuid = currentUser.Id.ToString()
        };

        TimeRecord currentPB = await GetPersonalBest(GameManager.Instance.selectedStage.Id);
        Postgrest.Responses.ModeledResponse<TimeRecord> response;

        Debug.Log(timeRecord.ToString());

        if(currentPB == null)
        {
            Debug.Log("No hay Personal best");
            response = await SupabaseManager.Instance.Supabase.From<TimeRecord>().Insert(timeRecord);
        }
        else if(currentPB.GetTotalTime() > timeRecord.GetTotalTime())
        {
            Debug.Log("Hay personal best");


            response = await SupabaseManager.Instance.Supabase
            .From<TimeRecord>()
            .Filter("id", Postgrest.Constants.Operator.Equals, currentPB.Id)
            .Set(x=>x.Sector_1, sector1)
            .Set(x => x.Sector_2, sector2)
            .Set(x => x.Sector_3, sector3)
            .Set(x => x.Car_id, GameManager.Instance.selectedCar.Id)
            .Update();

        }
        else
        {
            return;
        }


        if (response.Models == null)
        {
            Debug.LogError("Error al insertar el tiempo");
        }
        else
        {
            Debug.Log("Tiempo guardado");
        }
    }

    public async Task<TimeRecord> GetPersonalBest(int stageId)
    {
        var user = SupabaseManager.Instance.Supabase.Auth.CurrentSession?.User;
        if(user == null) return null;


        var response = await SupabaseManager.Instance.Supabase
            .From<TimeRecord>()
            .Select("*")
            .Filter("user_uuid", Postgrest.Constants.Operator.Equals, user.Id.ToString())
            .Filter("stage_id", Postgrest.Constants.Operator.Equals, stageId)
            .Order("total_time", Postgrest.Constants.Ordering.Ascending)
            .Limit(1)
            .Get();


        if(response.Models.Count == 0)
        {
            return null;
        }
    
        return response.Models[0];
    }

    public async Task<List<LeaderboardEntry>> GetLeaderboard(int stageId)
    {
        var result = await SupabaseManager.Instance.Supabase.Postgrest.Rpc<List<LeaderboardEntry>>("get_stage_leaderboard", new Dictionary<string, object>
        {
            {"stage_id_input", stageId}
        });

        if(result.Count() == 0)
            return null;

        return result;
    }
}
