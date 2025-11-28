using UnityEngine;
using UnityEngine.InputSystem;

public class GoBackHandler : MonoBehaviour
{
    [SerializeField] private string targetScene;

    private InputAction goBackAction;

    void Start()
    {
        goBackAction = InputManager.Instance.playerInput.actions["Back"];
        goBackAction.performed += HandleBack;
    }

    void OnDestroy()
    {
        goBackAction.performed -= HandleBack;
    }


    private void HandleBack(InputAction.CallbackContext context)
    {
        if(targetScene == "exit")
        {
            Application.Quit();
        }
        else
        {
            GameManager.Instance.LoadScene(targetScene);
        }
    }
}
