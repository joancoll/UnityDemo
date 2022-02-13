using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // declare reference variables
    CharacterController characterController;
    Animator animator;
    PlayerInput playerInput; // NOTE: PlayerInput class must be generated from New Input System in Inspector 

    // variables to store optimized setter/getter parameter IDs
    int isWalkingHash;
    int isRunningHash;

    // variables to store player input values
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 appliedMovement;
    bool isMovementPressed;
    bool isRunPressed;

    // constants
    float rotationFactorPerFrame = 15.0f;
    float runMultiplier = 4.0f;
    int zero = 0;

    //gravity variables
    float gravity = -9.8f;
    float groundedGravity = -.05f;

    // Awake is called earlier than Start in Unity's event life cycle
    void Awake() 
    {
        // initially set reference variables
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // set the parameter hash references
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

        // set the player input callbacks
        playerInput.CharacterControl.Move.started += onMovementInput;
        playerInput.CharacterControl.Move.canceled += onMovementInput;
        playerInput.CharacterControl.Move.performed += onMovementInput;
        playerInput.CharacterControl.Run.started += onRun;
        playerInput.CharacterControl.Run.canceled += onRun;
    }


    void onRun (InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    void handleRotation()
    {
        Vector3 positionToLookAt;
        // the change in position our character should point to
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = zero;
        positionToLookAt.z = currentMovement.z;
        // the current rotation of our character
        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed) {
            // creates a new rotation based on where the player is currently pressing
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            // rotate the character to face the positionToLookAt            
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    // handler function to set the player input values
    void onMovementInput (InputAction.CallbackContext context)
    {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x;
            currentMovement.z = currentMovementInput.y;
            currentRunMovement.x = currentMovementInput.x * runMultiplier;
            currentRunMovement.z = currentMovementInput.y * runMultiplier;
            isMovementPressed = currentMovementInput.x != zero || currentMovementInput.y != zero;
    }
    
    void handleAnimation()
    {
        // get parameter values from animator
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        // start walking if movement pressed is true and not already walking
        if (isMovementPressed && !isWalking) {
            animator.SetBool(isWalkingHash, true);
        }
        // stop walking if isMovementPressed is false and not already walking
        else if (!isMovementPressed && isWalking) {
            animator.SetBool(isWalkingHash, false);
        }
        // run if movement and run pressed are true and not currently running
        if ((isMovementPressed && isRunPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }
        // stop running if movement or run pressed are false and currently running
        else if ((!isMovementPressed || !isRunPressed) && isRunning) {
            animator.SetBool(isRunningHash, false);
        }
    }

    void handleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f;
        float fallMultiplier = 4.0f;
        // apply proper gravity if the player is grounded or not
        if (characterController.isGrounded) {
            currentMovement.y = groundedGravity;
            appliedMovement.y = groundedGravity;

            // additional gravity applied after reaching apex of jump
        } else if (isFalling) {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            appliedMovement.y = Mathf.Max((previousYVelocity + currentMovement.y) * .5f, -20.0f);
            
            // applied when character is not grounded
        } else {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * Time.deltaTime);
            appliedMovement.y = (previousYVelocity + currentMovement.y) * .5f;
        }


    }

    // Update is called once per frame
    void Update()
    {
        handleRotation();
        handleAnimation();

        if (isRunPressed) {
            appliedMovement.x=currentRunMovement.x;
            appliedMovement.z=currentRunMovement.z;
        } else {
            appliedMovement.x=currentMovement.x;
            appliedMovement.z=currentMovement.z;
        }

        characterController.Move(appliedMovement * Time.deltaTime);

        handleGravity();
    }
    
    void OnEnable()
    {
        // enable the character controls action map
        playerInput.CharacterControl.Enable();
    }

    void OnDisable()
    {
        // disable the character controls action map
        playerInput.CharacterControl.Disable();
    }
}
