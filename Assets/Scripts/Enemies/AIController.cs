using UnityEngine;

public class AIController : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float speed = 3.0f;
    public float chaseSpeed = 5.0f;
    public float searchDuration = 5.0f; 
    public float searchRadius = 10.0f; 

    private int currentPatrolIndex;
    private AIStateManager stateManager;
    private Transform playerTransform;
    private float searchTimer = 0f;

    void Start()
    {
        currentPatrolIndex = 0;
        stateManager = GetComponent<AIStateManager>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; 
    }

    void Update()
    {
        switch (stateManager.CurrentState)
        {
            case AIState.Patrolling:
                Patrol();
                break;
            case AIState.Chasing:
                Chase();
                break;
            case AIState.Searching:
                Search();
                break;
        }
    }

    void Patrol()
    {
        Transform targetPoint = patrolPoints[currentPatrolIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    void Chase()
    {
        if (playerTransform != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, chaseSpeed * Time.deltaTime);
        }
        else
        {
            // Player not found, transition to searching
            stateManager.ChangeState(AIState.Searching); 
            searchTimer = searchDuration; // Reset search timer
        }
    }

    void Search()
    {
        searchTimer -= Time.deltaTime;

        // Simple search: Rotate the AI while searching
        transform.Rotate(Vector3.up * 180f * Time.deltaTime);

        if (searchTimer <= 0)
        {
            // Search time over, return to patrolling
            stateManager.ChangeState(AIState.Patrolling);
        }
    }
}
