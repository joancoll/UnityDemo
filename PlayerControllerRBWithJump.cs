using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerRBWithJump : MonoBehaviour
{
    public float playerSpeed = 5.0f;
    public float jumpForce = 5.0f;
    private Rigidbody rb;
    private Vector3 playerInput, movement;
    private float xMovement, zMovement;
    private bool isGrounded = false; //for collision use
    private bool playJump = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveControl();
        JumpControl();
    }

    // FixedUpdate is called same FPS (useful to apply Rigidbody forces)
    void FixedUpdate()
    {
        if (playJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playJump = false;
        }
    }

    void MoveControl()
    {
        xMovement = Input.GetAxis("Horizontal");
        zMovement = Input.GetAxis("Vertical");
        playerInput.Set(xMovement, 0, zMovement);
        movement = playerSpeed * Time.fixedDeltaTime * playerInput;
        // Opció 1 - propietat position
        rb.MovePosition(transform.position + movement);
        // Opció 2 - mètode Translate()
        // transform.Translate(movement);
        // Opció 3 - mètode .AddForce()
        // rb.AddForce(movement, ForceMode.Impulse);
    }

    void JumpControl()
    {
        if (Input.GetButtonDown("Jump") && rb.velocity.y == 0)
        // Input.GetKeyDown(KeyCode.Space)
        // if (Input.GetButtonDown("Jump") && isGrounded) // collision use
        {
            playJump = true; //per executar el salt a FixedUpdate()
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("Ground enter");
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("Ground exit");
            isGrounded = false;
        }
    }
}
