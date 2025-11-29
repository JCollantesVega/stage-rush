using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailField, passwordField;
    [SerializeField] private Button logInButton, signUpButton;
    [SerializeField] private TextMeshProUGUI invalidCredentialsWarning;
    [SerializeField] private Toggle keepSession;

    

    void Start()
    {
        logInButton.onClick.AddListener(OnLogInPerformed);
        signUpButton.onClick.AddListener(OnSignUpPerformed);
        invalidCredentialsWarning.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void OnLogInPerformed()
    {
        string email = emailField.text;
        bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

        if(!isEmail)
        {
            email = await AuthController.Instance.GetEmailByUserName(email);
        }
        
        bool result = await AuthController.Instance.LogInUser(email, passwordField.text, keepSession.isOn);
        
        if(!result)
        {
            invalidCredentialsWarning.gameObject.SetActive(true);
        }
    }

    public void OnSignUpPerformed()
    {
        GameManager.Instance.LoadScene("Register");
    }

}
