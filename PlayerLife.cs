using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] AudioSource deathSound;
    Animator animator;

    private enum AnimationState
    { // Han de coincidir amb els noms dels booleans que controlen les animacions de l'animator
        Idle,
        Walk,
        Run,
        Jump,
        Dead
    }


    bool dead = false;

    private void Update()
    {
        if (transform.position.y < -1f && !dead)
        {
            Die();
        }
    }

    private void OnTriggerEnter(Collider collision)
    { 
        if (!dead){
            Debug.Log("Player collision trig");
            Debug.Log(collision.gameObject.tag);
            if (collision.gameObject.CompareTag("Enemy"))
            {  
                Debug.Log("Enemy trig");
                Die();
            }
            else if (collision.gameObject.CompareTag("Launcher"))
            {  
                Debug.Log("Launcher trig");
                SceneManager.LoadScene("Scene2");
            }
        }
    }

    void Die()
    {   animator = GetComponent<Animator>();
        SetAnimationStateToDeath();
        Invoke(nameof(ReloadLevel), 2.0f);
        dead = true;
        deathSound.Play();
    }

    private void SetAnimationStateToDeath()
    {
        // Canvia l'estat de l'animació a mort del personatge
        animator.SetBool(AnimationState.Jump.ToString(), false);
        animator.SetBool(AnimationState.Walk.ToString(), false);
        animator.SetBool(AnimationState.Run.ToString(), false);
        animator.SetBool(AnimationState.Idle.ToString(), false);
        animator.SetBool(AnimationState.Dead.ToString(), true); 
    }

    void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
