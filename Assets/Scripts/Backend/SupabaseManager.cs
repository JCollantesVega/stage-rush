using System;
using Supabase;
using TMPro;
using UnityEngine;

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
            _supabase = new Client(SUPABASE_URL, SUPABASE_PUBLIC_KEY);
            await _supabase.InitializeAsync();
        }
    }


}
