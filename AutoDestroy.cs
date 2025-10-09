using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float lifetimeMs = 1000f;

    void Start()
    {
        Destroy(gameObject, lifetimeMs / 1000f);
    }
}

