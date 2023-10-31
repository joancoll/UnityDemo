using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;
    private PlayerInput playerInput;

    private bool isJumping = false;
    private bool isJumpAnimating = false;
    private float gravity;

    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private Vector3 currentRunMovement, appliedMovement, positionToLookAt;
    private bool isMovementPressed = false;
    private bool isRunPressed = false;
    private bool isJumpPressed = false;
    private float initialJumpVelocity;

    private enum AnimationState
    { // Han de coincidir amb els noms dels booleans que controlen les animacions de l'animator
        Idle,
        Walk,
        Run,
        Jump,
        Dead
    }
    private AnimationState currentAnimationState = AnimationState.Idle;

    [Header("Paràmetres de Moviment")]
    [SerializeField] private float rotationFactorPerFrame = 15.0f; // Factor de rotació per fotograma
    [SerializeField] private float runMultiplier = 4.0f; // Multiplicador de velocitat de córrer

    [Header("Paràmetres de Salt")]
    [SerializeField] private float maxJumpHeight = 2.0f; // Altura màxima de salt
    [SerializeField] private float maxJumpTime = 0.75f; // Temps màxim de salt
    [SerializeField] private float groundedGravity = -0.05f; // Atracció negativa per simular petita gravetat quan està a terra

    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Configura els callbacks per als controls d'entrada del jugador
        playerInput.CharacterControl.Move.started += OnMovementInput;
        playerInput.CharacterControl.Move.canceled += OnMovementInput;
        playerInput.CharacterControl.Move.performed += OnMovementInput;
        playerInput.CharacterControl.Run.started += OnRun;
        playerInput.CharacterControl.Run.canceled += OnRun;
        playerInput.CharacterControl.Jump.started += OnJump;
        playerInput.CharacterControl.Jump.canceled += OnJump;

        SetupJumpVariables(); // Inicialitza les variables de salt
    }

    void SetupJumpVariables()
    {
        // Calcula les variables de salt en base als paràmetres definits
        // La força de llançament per poder fer el salt en el temps especificat
        // gravity indica la gravetat aplicada al personatge quan salta cap amunt
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    void OnRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    void OnEnable()
    {
        playerInput.CharacterControl.Enable();
    }

    void OnDisable()
    {
        playerInput.CharacterControl.Disable();
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleGravity();
        HandleJump();
        HandleAnimation();
    }

    void HandleMovement()
    {
        // Gestiona el moviment del personatge i aplica els canvis a la posició si camina o corre
        appliedMovement.x = isRunPressed ? currentRunMovement.x : currentMovement.x;
        appliedMovement.z = isRunPressed ? currentRunMovement.z : currentMovement.z;
        characterController.Move(appliedMovement * Time.deltaTime);
    }

    void HandleRotation()
    {
        // Gestiona la rotació del personatge en base a la direcció del moviment
        if (currentMovementInput != Vector2.zero)
        {
            positionToLookAt.Set(currentMovement.x, 0, currentMovement.z);
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void HandleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        float fallMultiplier = 2.0f;
        // fall Multiplier indica multiplicador gravetat en caiguda quan arriba al punt màxim del salt

        // Aplica la gravetat adequada si el jugador està a terra o no
        if (characterController.isGrounded)
        {
            if (isJumpAnimating)
            {
                isJumpAnimating = false;
            }
            currentMovement.y = groundedGravity;
            appliedMovement.y = groundedGravity;
        }
        else if (isFalling)
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            appliedMovement.y = Mathf.Max((previousYVelocity + currentMovement.y) * 0.5f, -20.0f);
        }
        else
        {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * Time.deltaTime);
            appliedMovement.y = (previousYVelocity + currentMovement.y) * 0.5f;
        }
    }

    void HandleJump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed)
        {
            isJumpAnimating = true;
            isJumping = true;
            currentMovement.y = initialJumpVelocity;
            appliedMovement.y = initialJumpVelocity;
        }
        else if (!isJumpPressed && isJumping && characterController.isGrounded)
        {
            isJumping = false;
        }
    }

    private void HandleAnimation()
    {
        // Gestiona l'animació del personatge en base al moviment
        var newAnimationState = AnimationState.Idle;

        if (isJumping)
        {
            newAnimationState = AnimationState.Jump;
        }
        else if (isMovementPressed)
        {
            newAnimationState = isRunPressed ? AnimationState.Run : AnimationState.Walk;
        }

        if (newAnimationState != currentAnimationState)
        {
            SetAnimationState(newAnimationState);
        }
    }

    private void SetAnimationState(AnimationState newState)
    {
        // Canvia l'estat de l'animació del personatge 
        animator.SetBool(currentAnimationState.ToString(), false);
        currentAnimationState = newState;
        animator.SetBool(currentAnimationState.ToString(), true);
    }
}