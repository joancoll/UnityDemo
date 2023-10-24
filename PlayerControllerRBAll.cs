using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControllerRBAll : MonoBehaviour
{
    public float playerSpeed = 5.0f;
    public float jumpForce = 5.0f;
    public float enemyBounceForceBack = 5.0f;
    public float enemyBounceForceUp = 5.0f;
    public float bounceForceUp = 10.0f;
    public float bounceForceForward = 10.0f;
    public AudioClip audioJump;
    public AudioClip audioBounce;
    public AudioClip audioHit;
    public AudioClip audioTransport;
    public AudioClip audioLost;
    private const float DEATHPOS = -2f; //posició de mort
    private Rigidbody rb;
    private Vector3 playerInput, movement;
    private float xMovement, zMovement;
    private bool gameFinished = false; // per acabar el joc
    private bool stopMovement = false; // per parar el moviment
    private bool isGrounded = false; //for collision use
    private bool playJump = false;
    private Vector3 collisionDirection;
    private AudioSource audioSource;
    private ParticleSystem particles;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        particles = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopMovement)
        {
            MoveControl();
            JumpControl();
            LifeControl();
        }
    }

    void FixedUpdate()
    {
        if (playJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            particles.Play();
            audioSource.clip = audioJump;
            audioSource.Play();
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

    void LifeControl()
    {
        if (transform.position.y < DEATHPOS)
        {
            Debug.Log("Game Over");
            stopMovement = true;
            gameFinished = true;
            // Canvi d'escena
            StartCoroutine(SceneLoader(audioLost, SceneManager.GetActiveScene().buildIndex));
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("Ground enter");
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Injured!");
            audioSource.clip = audioHit;
            audioSource.Play();
            // Calcula la direcció contrària de la col·lisió
            // Obté la direcció del xoc
            collisionDirection = (transform.position - collision.transform.position).normalized;
            // Aplica una força contrària del sentit del Player 
            rb.AddForce(collisionDirection * enemyBounceForceBack + Vector3.up * enemyBounceForceUp, ForceMode.Impulse);
        }
        if (collision.gameObject.CompareTag("Bounce"))
        {
            Debug.Log("Bouncing");
            audioSource.clip = audioBounce;
            audioSource.Play();
            // Obté la direcció del xoc en la col·lisió
            collisionDirection = (collision.transform.position - transform.position).normalized;
            // Aplica una força de salt en el sentit del Player 
            rb.AddForce(collisionDirection * bounceForceForward + Vector3.up * bounceForceUp, ForceMode.Impulse);
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

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("NextLevel"))
        {
            Debug.Log("Next Level");
            stopMovement = true;
            // Canvi d'escena
            StartCoroutine(SceneLoader(audioTransport, SceneManager.GetActiveScene().buildIndex + 1));
        }
    }

    IEnumerator SceneLoader(AudioClip clip, int builtIndex)
    {
        audioSource.clip = clip;
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        gameFinished = false;
        SceneManager.LoadScene(builtIndex);
    }
}
