using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangePasswordUI : MonoBehaviour
{
    [SerializeField] TMP_InputField oldPasswordField;
    [SerializeField] TMP_InputField newPasswordField;
    [SerializeField] TMP_InputField confirmPasswordField;
    [SerializeField] Button changePasswordButton;
    [SerializeField] private Button goBackButton;

    void Start()
    {
        changePasswordButton.onClick.AddListener(OnChangePasswordPerformed);
        goBackButton.onClick.AddListener(OnGoBackPerformed);
    }


    public void OnGoBackPerformed()
    {
        GameManager.Instance.LoadScene("MainMenu");
    }

    async void OnChangePasswordPerformed()
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

        var result = await AuthController.Instance.ChangePassword(newPassword);

        if(result.Success)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Debug.LogError($"ERROR AL CAMBIAR LA CONTRASEÑA: {result.ErrorMessage}");
        }
    }


}
