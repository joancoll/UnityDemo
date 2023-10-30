using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerLife : MonoBehaviour
{
    const int DEFAULT_SCENE_INDEX = 0;
    private static GameController instance;
    private int playerLives = 3; // Afegim una variable per seguir les vides del jugador.

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChangeScene(DEFAULT_SCENE_INDEX);
        }
    }

    public void ChangeScene(int sceneBuildIndex)
    {
        SceneManager.LoadScene(sceneBuildIndex);
    }

    // Mètode per incrementar les vides del jugador
    public void IncreasePlayerLives(int amount)
    {
        playerLives += amount;
    }

    // Mètode per disminuir les vides del jugador
    public void DecreasePlayerLives(int amount)
    {
        playerLives -= amount;
        if (playerLives <= 0)
        {
            // Aquí pots gestionar la lògica quan el jugador queda sense vides.
        }
    }

    // Mètode per obtenir el valor actual de les vides del jugador
    public int GetPlayerLives()
    {
        return playerLives;
    }

    // Creem una propietat (opcional) per obtenir el valor de vides com a lectura
    public int PlayerLives
    {
        get { return playerLives; }
    }
}