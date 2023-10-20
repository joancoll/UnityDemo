using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerRB : MonoBehaviour
{
    public float playerSpeed = 5.0f;
    private Rigidbody rb;
    private Vector3 playerInput, movement;
    private float xMovement, zMovement;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveControl();
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

}
