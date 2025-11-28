using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Postgrest.Attributes;
using Postgrest.Models;
using Supabase.Gotrue;
using UnityEngine;

public class AuthController : MonoBehaviour
{
    public static AuthController Instance;
    public Action OnLoginSuccess;
    public Action<string, string> OnRegisterSuccess;

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

    public async Task<RegisterResult> RegisterUser(string email, string password, string userName)
    {
        bool canRegisterUser = await CanRegisterUser(userName.ToLower());
        Debug.Log(canRegisterUser);

        if(!canRegisterUser)
            return RegisterResult.UserNameExists;

        if(await EmailExists(email))
            return RegisterResult.MailExists;
        
        
        try
        {
            var signUp = await SupabaseManager.Instance.Supabase.Auth.SignUp(email, password, new SignUpOptions
            {
                Data = new Dictionary<string, object>
                {
                    {"display_name", userName}
                }
            });

            
//            Debug.Log(signUp.User);
            return RegisterResult.Success;
            

            
            
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
            if (ex.Message.Contains("User already registered") || ex.Message.Contains("email"))
            return RegisterResult.MailExists;
        }

        return RegisterResult.Success;

    }

    public async Task<bool> LogInUser(string email, string password)
    {
        Task<Session> singIn = SupabaseManager.Instance.Supabase.Auth.SignIn(email,password);

        try
        {
            await singIn;
        }catch(Exception ex)
        {
            Debug.Log("Exception");
            Debug.Log($"{ex.Message}");
            return false;
        }

        Session session = singIn.Result;

        if(session == null)
        {
            return false;
        }
        else
        {
            Debug.Log($"Sign in success {session.User?.Id} Token: {session.AccessToken} HASTA AQUI {session.User?.Aud} {session.User?.Email} {session.RefreshToken}");
            OnLoginSuccess?.Invoke();
            return true;
        }
    }

    public async void LogOut()
    {
        try
        {
            await SupabaseManager.Instance.Supabase.Auth.SignOut();

        }
        catch(Exception ex)
        {
            Debug.LogError($"Error when logging out:{ex.Message}");
        }
    }

    public async Task<List<DisplayNameRecord>> GetDisplayNameRecordsAsync()
    {
        // Llamada a la RPC
        var displayNames = await SupabaseManager.Instance.Supabase
            .Postgrest
            .Rpc<List<DisplayNameRecord>>("get_all_display_names", null);

        // Ahora displayNames ya es List<DisplayNameRecord>
        return displayNames;
    }

    public async Task<bool> CanRegisterUser(string userName)
    {
        List<DisplayNameRecord> displayNameRecords = await GetDisplayNameRecordsAsync();

        foreach(DisplayNameRecord display in displayNameRecords)
        {
            if(display.display_name == userName) return false;
        }

        return true;
    }

    public async Task<bool> EmailExists(string email)
    {
        var result = await SupabaseManager.Instance.Supabase.Postgrest.Rpc<bool>("email_exists", new {target_email = email});

        return result;
    }

    public async Task<string> GetEmailByUserName(string userName)
    {
        string result = await SupabaseManager.Instance.Supabase.Postgrest.Rpc<string>("get_email_by_displayname", new{username = userName});

        return result;
    }

}


public class Users : BaseModel
{
    [PrimaryKey("id")]
    [Column("id")]
    public Guid Id{get; set;}

    [Column("username")]
    public string Username{get; set;} 
}

public class DisplayNameRecord
{
    public string user_id { get; set; }
    public string display_name { get; set; }
}

public enum RegisterResult
{
    Success,
    UserNameExists,
    MailExists,
    UnknownError
}