using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public static InputManager Instance{get; private set;}

    public PlayerInput playerInput{get; private set;}

    public float Gas { get; private set; }
    public float Brake { get; private set; }
    public float Steer { get; private set; }
    public float Clutch { get; private set; }

    private InputActionMap globalMap;

    float gasSmooth, brakeSmooth, steerSmooth;

    float smoothSpeed;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            playerInput = GetComponent<PlayerInput>();

            globalMap = playerInput.actions.FindActionMap("Global");
            globalMap.Enable();
            
            EnableUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }



    // Update is called once per frame
    void Update()
    {
        if(playerInput.currentActionMap.name != "Gameplay")
            return;

        var gasAction = playerInput.actions["Gas"];
        var brakeAction = playerInput.actions["Brake"];
        var steerAction = playerInput.actions["Steer"];
        var clutchAction = playerInput.actions["Clutch"];

        float targetGas = gasAction.ReadValue<float>();
        float targetBrake = brakeAction.ReadValue<float>();
        float targetSteer = steerAction.ReadValue<float>();
        float targetClutch = clutchAction.ReadValue<float>();

        // gasSmooth = Mathf.Lerp(gasSmooth, targetGas, smoothSpeed * Time.deltaTime * 5f);
        // brakeSmooth = Mathf.Lerp(brakeSmooth, targetBrake, smoothSpeed * Time.deltaTime * 5f);
        // steerSmooth = Mathf.Lerp(steerSmooth, targetSteer, smoothSpeed * Time.deltaTime * 5f);

        Gas = targetGas;
        Brake = targetBrake;
        Steer = targetSteer;
        Clutch = targetClutch;

    }

    public void EnableGameplay()
    {
        playerInput.SwitchCurrentActionMap("Gameplay");
    }

    public void EnableUI()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }
}
