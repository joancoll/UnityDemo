using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerFixedCamera : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;
    private PlayerInput playerInput;

    private bool isJumping = false;
    private bool isJumpAnimating = false;
    private float gravity;

    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private Vector3 currentRunMovement;
    private Vector3 appliedMovement;

    private bool isMovementPressed = false;
    private bool isRunPressed = false;
    private bool isJumpPressed = false;

    private float initialJumpVelocity;

    private enum AnimationState
    {
        Idle,
        Walk,
        Run,
        Jump,
        Dead
    }

    private AnimationState currentAnimationState = AnimationState.Idle;

    [Header("Paràmetres de moviment")]
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float runMultiplier = 4f;

    [Header("Paràmetres de salt")]
    [SerializeField] private float maxJumpHeight = 2f;
    [SerializeField] private float maxJumpTime = 0.75f;
    [SerializeField] private float groundedGravity = -0.05f;

    void Awake()
    {
        playerInput = new PlayerInput();

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        playerInput.CharacterControl.Move.started += OnMovementInput;
        playerInput.CharacterControl.Move.performed += OnMovementInput;
        playerInput.CharacterControl.Move.canceled += OnMovementInput;

        playerInput.CharacterControl.Run.started += OnRun;
        playerInput.CharacterControl.Run.canceled += OnRun;

        playerInput.CharacterControl.Jump.started += OnJump;
        playerInput.CharacterControl.Jump.canceled += OnJump;

        SetupJumpVariables();
    }

    void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2f;

        gravity = (-2f * maxJumpHeight) /
                  Mathf.Pow(timeToApex, 2f);

        initialJumpVelocity = (2f * maxJumpHeight) /
                              timeToApex;
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = Vector2.ClampMagnitude(
            context.ReadValue<Vector2>(), 1f);

        currentMovement.x = -currentMovementInput.x;
        currentMovement.z = -currentMovementInput.y;

        currentRunMovement.x = -currentMovementInput.x * runMultiplier;
        currentRunMovement.z = -currentMovementInput.y * runMultiplier;

        isMovementPressed = currentMovementInput != Vector2.zero;
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
        HandleGravity();
        HandleJump();
        HandleRotation();
        HandleMovement();
        HandleAnimation();
    }

    void HandleMovement()
    {
        appliedMovement.x = isRunPressed
            ? currentRunMovement.x
            : currentMovement.x;

        appliedMovement.z = isRunPressed
            ? currentRunMovement.z
            : currentMovement.z;

        characterController.Move(appliedMovement * Time.deltaTime);
    }

    void HandleRotation()
    {
        Vector3 movementDirection = new Vector3(
            -currentMovementInput.x,
            0f,
            -currentMovementInput.y
        );

        if (movementDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(movementDirection);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void HandleGravity()
    {
        bool isFalling = currentMovement.y <= 0f || !isJumpPressed;
        float fallMultiplier = 2f;

        if (characterController.isGrounded)
        {
            if (isJumpAnimating)
            {
                isJumpAnimating = false;
            }

            if (currentMovement.y < 0f)
            {
                currentMovement.y = groundedGravity;
            }

            appliedMovement.y = currentMovement.y;
        }
        else if (isFalling)
        {
            float previousYVelocity = currentMovement.y;

            currentMovement.y += gravity * fallMultiplier * Time.deltaTime;

            appliedMovement.y = Mathf.Max(
                (previousYVelocity + currentMovement.y) * 0.5f,
                -20f
            );
        }
        else
        {
            float previousYVelocity = currentMovement.y;

            currentMovement.y += gravity * Time.deltaTime;

            appliedMovement.y =
                (previousYVelocity + currentMovement.y) * 0.5f;
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

    void HandleAnimation()
    {
        AnimationState newAnimationState = AnimationState.Idle;

        if (isJumping)
        {
            newAnimationState = AnimationState.Jump;
        }
        else if (isMovementPressed)
        {
            newAnimationState = isRunPressed
                ? AnimationState.Run
                : AnimationState.Walk;
        }

        if (newAnimationState != currentAnimationState)
        {
            SetAnimationState(newAnimationState);
        }
    }

    void SetAnimationState(AnimationState newState)
    {
        animator.SetBool(currentAnimationState.ToString(), false);

        currentAnimationState = newState;

        animator.SetBool(currentAnimationState.ToString(), true);
    }
}
