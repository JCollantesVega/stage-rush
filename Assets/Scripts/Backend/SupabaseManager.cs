using System;
using System.Threading.Tasks;
using Supabase.Gotrue;
using UnityEngine;
using Client = Supabase.Client;

public class SupabaseManager : MonoBehaviour
{
    public static SupabaseManager Instance {get; private set;}

    public const string SUPABASE_PUBLIC_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6IndpZXJsYnFycG14cmdybGFqdHFxIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjI4MTgyNzYsImV4cCI6MjA3ODM5NDI3Nn0.fYAFSSlSEGIOyBF31l8Tmi2ymKiFF7rDVbRDe_GilVg";
    public const string SUPABASE_URL = "https://wierlbqrpmxrgrlajtqq.supabase.co";

    private static Client _supabase;
    public Client Supabase
    {
        get 
        {
            return _supabase;
        }
    }
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

    private async void Start()
    {
        if(_supabase == null)
        {
            _supabase = new Supabase.Client(SUPABASE_URL, SUPABASE_PUBLIC_KEY);
            await _supabase.InitializeAsync();
        }

        await TryRestoreSession();
    }

    private async Task TryRestoreSession()
    {
        string refreshToken = PlayerPrefs.GetString("refresh_token", "");

        if(string.IsNullOrEmpty(refreshToken))
            return;

        try
        {
            var session = await _supabase.Auth.SignIn(Constants.SignInType.RefreshToken, refreshToken);

            if(session != null && session.User != null)
            {
                AuthController.Instance?.OnLoginSuccess?.Invoke();

                if (!string.IsNullOrEmpty(session.RefreshToken))
                {
                    PlayerPrefs.SetString("refresh_token", session.RefreshToken);
                    PlayerPrefs.Save();
                }

            }
            else
            {
                PlayerPrefs.DeleteKey("refresh_token");
            }
        }
        catch(Exception ex)
        {
            Debug.LogWarning($"Error restaurando sessión: {ex.Message}");
            PlayerPrefs.DeleteKey("refresh_token");
        }
    }


}
