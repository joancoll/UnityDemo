using UnityEngine;


public class GunShooter : MonoBehaviour
{
    public GameObject bombPrefab;   // Prefab de la bomba (assigna-la des de l’Inspector)
    public Transform shootPoint;    // Punt d'origen del tret (pot ser davant del jugador)
    public int numberOfBombs = 5;   // Quantes bombes es dispararan
    public float spread = 10f;      // Obertura lateral del tret
    public float shootForce = 800f; // Força del tret


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ShootBombs();
        }
    }


    void ShootBombs()
    {
        for (int i = 0; i < numberOfBombs; i++)
        {
            // Crea una nova bomba
            GameObject newBomb = Instantiate(bombPrefab, shootPoint.position, shootPoint.rotation);
            // Afegeix una mica de variació en la direcció (dispersió)
            Vector3 randomDirection = shootPoint.forward +
                                      new Vector3(Random.Range(0, spread), Random.Range(0, spread), 0f) * 0.01f;
            // Normalitza la direcció
            randomDirection.Normalize();
            // Aplica una força a la bomba per disparar-la
            newBomb.GetComponent<Rigidbody>().AddForce(randomDirection * shootForce);
        }
    }
}

