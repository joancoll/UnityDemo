using UnityEngine;
using UnityEngine.AI;

public class FollowSomethingController : MonoBehaviour
{
    // Start is called before the first frame update
    
    public CharacterController followTo;
    private Animator animator;


    private NavMeshAgent pathfinder;
    private Transform target;

    void Awake() 
    {
        // initially set reference variables
        animator = GetComponent<Animator>();
        pathfinder = GetComponent<NavMeshAgent>();
    }
    
    void Start()
    {   target = GameObject.Find(followTo.name).transform;
    }

    void Update()
    { // go for player
        pathfinder.SetDestination(target.position);
    }

}