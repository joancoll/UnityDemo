using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemController : MonoBehaviour
{
    // ALERTA! Aquest script només funciona si heu instal·lat el paquet InputSystem
    // i l'heu definit per defecte a les preferències del projecte com entrada al Player
    // També cal un mapa d'entrades anomenat CharacterControl amb els següents controls:
    // Move (Vector2) i Jump (Button) i Run (Button)
    private PlayerInput playerInput;
    private Vector2 currentMovementInput;
    bool isJumpPressed, isRunPressed;

    private void Awake()
    {
        Debug.Log("Setting up Input System");
        playerInput = new PlayerInput();
        // Configura els callbacks per als controls d'entrada del jugador
        playerInput.CharacterControl.Move.started += OnMovementInput;
        playerInput.CharacterControl.Move.canceled += OnMovementInput;
        playerInput.CharacterControl.Move.performed += OnMovementInput;
        playerInput.CharacterControl.Run.started += OnRun;
        playerInput.CharacterControl.Run.canceled += OnRun;
        playerInput.CharacterControl.Jump.started += OnJump;
        playerInput.CharacterControl.Jump.canceled += OnJump;
    }
    void OnEnable()
    {
        playerInput.CharacterControl.Enable();
    }

    void OnDisable()
    {
        playerInput.CharacterControl.Disable();
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        Debug.Log("Movement Input: " + currentMovementInput);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
        Debug.Log("Jump Pressed: " + isJumpPressed);
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
        Debug.Log("Run Pressed: " + isRunPressed);
    }
}