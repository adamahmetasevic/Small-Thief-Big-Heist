using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float speed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 10f;
    public float fieldOfViewAngle = 110f;
    public LayerMask obstacleMask;
    public float lostPlayerTime = 5f; // Time to wait before switching to patrol

    private int currentPatrolIndex;
    private Transform player;
    private bool isChasing = false;
    private float chaseTimer = 0f;
    private Animator animator;
    private Vector3 lastPosition;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        currentPatrolIndex = 0;
        lastPosition = transform.position;
        StartCoroutine(Patrol());
    }

    void Update()
    {
        // Calculate the speed of the AI
        float distanceMoved = Vector3.Distance(lastPosition, transform.position);
        float speed = distanceMoved / Time.deltaTime;
        lastPosition = transform.position;

        // Update the animator with the current speed
        animator.SetFloat("Speed", speed);

        if (isChasing)
        {
            ChasePlayer();
            CheckIfLostPlayer();
        }
        else
        {
            DetectPlayer();
        }
    }

    IEnumerator Patrol()
    {
        while (true)
        {
            if (!isChasing)
            {
                Transform targetPoint = patrolPoints[currentPatrolIndex];
                float distanceToTarget = Vector3.Distance(transform.position, targetPoint.position);
                
                if (distanceToTarget > 0.5f)
                {
                    // Move towards the patrol point
                    transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

                    // Rotate to face the patrol point but keep the AI upright
                    Vector3 directionToTarget = targetPoint.position - transform.position;
                    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                    targetRotation.x = 0; // Keep the AI upright
                    targetRotation.z = 0; // Keep the AI upright

                    // Smoothly rotate the AI towards the patrol point
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Adjust speed as needed
                }
                else
                {
                    // Update to the next patrol point
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                }
            }
            yield return null;
        }
    }

    void DetectPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angle = Vector3.Angle(directionToPlayer, transform.forward);

        // Calculate the height offset (e.g., half of the enemy's height)
        float heightOffset = 1f; // Adjust this value to match the desired height

        // Set the start point of the raycast at the height of the enemy
        Vector3 raycastStart = transform.position + Vector3.up * heightOffset;

        if (angle < fieldOfViewAngle * 0.5f && distanceToPlayer < detectionRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(raycastStart, directionToPlayer, out hit, detectionRange, ~obstacleMask))
            {
                Debug.DrawLine(raycastStart, hit.point, Color.red);
                if (hit.collider.CompareTag("Player"))
                {
                    PlayerDetected();
                }
            }
            else
            {
                Debug.DrawRay(raycastStart, directionToPlayer.normalized * detectionRange, Color.green);
            }
        }
    }

    void CheckIfLostPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) > detectionRange)
        {
            chaseTimer += Time.deltaTime;
            if (chaseTimer >= lostPlayerTime)
            {
                LostPlayer();
            }
        }
        else
        {
            chaseTimer = 0f; // Reset the timer if the player is still in range
        }
    }

    public void PlayerDetected()
    {
        isChasing = true;
        chaseTimer = 0f; // Reset the timer when the player is detected
        // Additional logic when the player is detected
    }

    public void LostPlayer()
    {
        isChasing = false;
        chaseTimer = 0f; // Reset the timer
        // Additional logic when the player is lost
        StartCoroutine(Patrol());
    }

    void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
        transform.LookAt(player); // Ensure the enemy is facing the player
        // Additional chasing logic can be added here
    }
}
