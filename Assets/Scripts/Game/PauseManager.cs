using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public bool isPaused{get; private set;}
    public bool pauseAllowed;

    public event Action<bool> OnGamePaused;

    private PlayerInput playerInput;
    private InputAction pauseAction;
    private InputAction cancelAction;

    void Start()
    {
        playerInput = InputManager.Instance.playerInput;

        pauseAction = playerInput.actions["Pause"];
        pauseAction.performed += HandlePause;

        cancelAction = playerInput.actions["Cancel"];
        cancelAction.performed += HandlePause;

        isPaused = false;

        Time.timeScale = 1;
    }

    void OnDestroy()
    {
        pauseAction.performed -= HandlePause;
        cancelAction.performed -= HandlePause;
    }
    private void HandlePause(InputAction.CallbackContext ctx)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if(!pauseAllowed)
            return;
        
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;

        if(isPaused)
        {
            InputManager.Instance.EnableUI();
            CarController.Instance.carFX.engineSound.Pause();
        }
        else
        {
            InputManager.Instance.EnableGameplay();
            CarController.Instance.carFX.engineSound.Play();
        }

        OnGamePaused?.Invoke(isPaused);

    }
}
