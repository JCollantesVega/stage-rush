using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangePasswordUI : MonoBehaviour
{
    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField oldPasswordField;
    [SerializeField] TMP_InputField newPasswordField;
    [SerializeField] TMP_InputField confirmPasswordField;
    [SerializeField] Button changePasswordButton;

    bool isUserLogged;

    void Start()
    {
        isUserLogged = SupabaseManager.Instance.Supabase.Auth.CurrentSession == null;

        if(isUserLogged)
        {
            emailField.gameObject.SetActive(false);
            oldPasswordField.gameObject.SetActive(true);
            newPasswordField.gameObject.SetActive(true);
            confirmPasswordField.gameObject.SetActive(true);
            changePasswordButton.onClick.AddListener(OnChangePasswordPerformedLoggedIn);
        }
        else
        {
            emailField.gameObject.SetActive(true);
            oldPasswordField.gameObject.SetActive(false);
            newPasswordField.gameObject.SetActive(false);
            confirmPasswordField.gameObject.SetActive(false);
        }

    }

    void OnChangePasswordPerformedLoggedIn()
    {
        string oldPassword = oldPasswordField.text;
        string newPassword = newPasswordField.text;
        string confirmPassword = confirmPasswordField.text;

        if (string.IsNullOrEmpty(oldPassword))
        {
            Debug.Log("Field cannot be empty");
            return;
        }

        if (string.IsNullOrEmpty(newPassword))
        {
            Debug.Log("Field cannot be empty");
            return;
        }

        if (string.IsNullOrEmpty(confirmPassword))
        {
            Debug.Log("Field cannot be empty");
            return;
        }

        if(oldPassword == newPassword)
        {
            Debug.Log("New password cannot be same as old password");
            return;
        }

        if(newPassword != confirmPassword)
        {
            Debug.Log("Passwords not matching");
            return;
        }

        AuthController.Instance.ChangePassword(newPassword);
    }

    void SendPasswordReset()
    {
        string email = emailField.text;

        if (string.IsNullOrEmpty(email))
        {
            Debug.Log("Field cannot be empty");
            return;
        }
    }

}
