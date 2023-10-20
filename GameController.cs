using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // El GameObject que conté aquest script (GameController) ha de ser marcat com a
    // "Don't Destroy On Load" perquè no es reiniciï quan canviïs d'escena.
    const int DEFAULT_SCENE_INDEX = 0;  // Defineix l'escena per defecte a la qual es canviarà quan es premi "ESC".
    private static GameController instance;

    private void Awake()
    {
        if (instance == null)
        {
            // Manté aquest GameObject quan es canvia d'escena.
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            // Ja existeix una instància, així que aquest GameObject no és necessari.
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
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
}
