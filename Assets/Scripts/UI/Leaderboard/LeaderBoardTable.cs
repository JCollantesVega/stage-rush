using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LeaderBoardTable : MonoBehaviour
{

    public GameObject leaderBoardRegistryPrefab;
    public Transform contentElement;

    public async void GenerateLeaderboard(int trackId)
    {
        var currentUser = SupabaseManager.Instance.Supabase.Auth.CurrentSession?.User;


        List<LeaderboardEntry> entries = await DatabaseController.Instance.GetLeaderboard(trackId);
        
        int positionIndex = 1;

        for(int i = 0; i < entries.Count; i++)
        {
            bool isUser = false;
            if(currentUser != null)
            {
                isUser = entries[i].userName == currentUser.UserMetadata["display_name"].ToString();
            }

            GameObject newRegistry = Instantiate(leaderBoardRegistryPrefab, contentElement);
            LeaderboardRegistry registry = newRegistry.GetComponent<LeaderboardRegistry>();
            registry.Setup(positionIndex, entries[i].userName, entries[i].stage_time, entries[i].car_model, isUser);
            positionIndex++;
        }
    }

    public void ClearLeaderboard()
    {
        LeaderboardRegistry[] entries = contentElement.GetComponentsInChildren<LeaderboardRegistry>();

        foreach(LeaderboardRegistry registry in entries)
        {
            Debug.Log($"Borrar registro {registry}");
            Destroy(registry.gameObject);
        }
    }
}
