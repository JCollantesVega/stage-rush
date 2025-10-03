using UnityEngine;
using UnityEngine.InputSystem;

class CarInputHandler : MonoBehaviour
{
    CarController carController;

    private PlayerInput playerInput;
    private InputAction accelerateAction;
    private InputAction steerAction;

    void Awake()
    {
        carController = GetComponent<CarController>();

        playerInput = GetComponentInChildren<PlayerInput>();
        accelerateAction = playerInput.actions["Accelerate"];
        steerAction = playerInput.actions["Steer"];
    }

    void Update()
    {
        Vector2 inputVector = Vector2.zero;

        inputVector.y = accelerateAction.ReadValue<float>();
        inputVector.x = steerAction.ReadValue<float>();

        carController.setInputVector(inputVector);
    }
}