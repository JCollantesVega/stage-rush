using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton, accountButton, logOutButton, logInButton, recordsButton;
    [SerializeField] private GameObject logInBtnGameObject, logOutBtnGameObject;
    [SerializeField] private TextMeshProUGUI currentUser;

    private GameObject lastItemSelected;

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastItemSelected);
        }
        else
        {
            lastItemSelected = EventSystem.current.currentSelectedGameObject;
        }
    }

    IEnumerator Start()
    {
        //espera un frame antes de iniciar
        //sin esta línea hay NullReference en línea 19
        yield return null;

        playButton.onClick.AddListener(onPlayButtonClickPerformed);
        logOutButton.onClick.AddListener(OnLogOutClickPerformed);
        logInButton.onClick.AddListener(OnLoginClickPerformed);
        recordsButton.onClick.AddListener(OnRecordsClickPerformed);

        if(SupabaseManager.Instance.Supabase.Auth.CurrentUser == null)
        {
            logOutBtnGameObject.SetActive(false);
            logInBtnGameObject.SetActive(true);
            accountButton.interactable = false;
            currentUser.text = $"User: Guest";
        }
        else
        {
            logOutBtnGameObject.SetActive(true);
            logInBtnGameObject.SetActive(false);
            accountButton.interactable = true;
            currentUser.text = $"User: {SupabaseManager.Instance.Supabase.Auth.CurrentUser.UserMetadata["display_name"]}";
        }

    } 

    void OnLogOutClickPerformed()
    {
        AuthController.Instance.LogOut();

        logOutBtnGameObject.SetActive(false);
        logInBtnGameObject.SetActive(true);
        accountButton.interactable = false;
        currentUser.text = $"User: Guest";

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(logInBtnGameObject);
    }


    void onPlayButtonClickPerformed()
    {
        GameManager.Instance.LoadScene("CarLoader");
    }

    void OnLoginClickPerformed()
    {
        GameManager.Instance.LoadScene("LogIn");
    }

    void OnRecordsClickPerformed()
    {
        GameManager.Instance.LoadScene("RecordsMenu");
    }
}
