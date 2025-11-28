using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterUI : MonoBehaviour
{
    [SerializeField] private Button registerButton, goBackButton;
    [SerializeField] private TMP_InputField emailField, passwordField, userNameField;
    public TextMeshProUGUI userNameAlreadyExists, mailAlreadyRegistered, registeredComplete;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        goBackButton.onClick.AddListener(OnGoBackPerformed);
        registerButton.onClick.AddListener(OnSignUpPerformed);

        userNameAlreadyExists.gameObject.SetActive(false);
        mailAlreadyRegistered.gameObject.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGoBackPerformed()
    {
        GameManager.Instance.LoadScene("Login");
    }

    public async void OnSignUpPerformed()
    {
        userNameAlreadyExists.gameObject.SetActive(false);
        mailAlreadyRegistered.gameObject.SetActive(false);

        var result = await AuthController.Instance.RegisterUser(emailField.text, passwordField.text, userNameField.text);

        switch(result)
        {
            case RegisterResult.Success:
                
                break;

            case RegisterResult.UserNameExists:
                userNameAlreadyExists.gameObject.SetActive(true);
                break;

            case RegisterResult.MailExists:
                mailAlreadyRegistered.gameObject.SetActive(true);
                break;

            case RegisterResult.UnknownError:
                
                break;
        }

    }
}
