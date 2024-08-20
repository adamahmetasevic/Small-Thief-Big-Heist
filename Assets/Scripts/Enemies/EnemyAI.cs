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
    public LayerMask restrictedAreaMask;
    public float lostPlayerTime = 5f; 
[SerializeField] private float slowMotionTimeScale = 0.2f; // Time scale during slow motion
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
        float distanceMoved = Vector3.Distance(lastPosition, transform.position);
        float currentSpeed = distanceMoved / Time.deltaTime;
        lastPosition = transform.position;

        animator.SetFloat("Speed", currentSpeed);

        if (isChasing)
        {
            ChasePlayer();
            if (!GameManager.instance.IsObjectMadeBig())
            {
                CheckIfLostPlayer();
            }
        }
        else
        {
            if (IsPlayerOnRestrictedArea())
            {
                DetectPlayer();
            }
        }
    }

    bool IsPlayerOnRestrictedArea()
    {
        return false; //Physics.CheckSphere(player.position, 0.2f, restrictedAreaMask);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isChasing)
        {
            TriggerFailureScene();
        }
    }

   IEnumerator Patrol()
{
    while (true)
    {
        if (!isChasing)
        {
            // Check if the patrolPoints array has elements
            if (patrolPoints.Length > 0) 
            {
                Transform targetPoint = patrolPoints[currentPatrolIndex];
                float distanceToTarget = Vector3.Distance(transform.position, targetPoint.position);

                if (distanceToTarget > 0.5f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

                    Vector3 directionToTarget = targetPoint.position - transform.position;
                    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                    targetRotation.x = 0; // Keep the enemy upright
                    targetRotation.z = 0; 

                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                }
                else
                {
                    // Move to the next patrol point
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length; 
                }
            }
            else
            {
                // Handle the case where there are no patrol points
                Debug.LogWarning("No patrol points assigned to " + gameObject.name);
                yield break; // Stop the coroutine 
            }
        }
        yield return null; 
    }
}

    private IEnumerator SlowMotionAndFailureSequence()
{
    // Play the victory sound effect (if you have one)
    // audioSource.PlayOneShot(failureSound);

    // Slow down time
    Time.timeScale = slowMotionTimeScale;

    // Wait for a specified delay (adjust as needed)
    yield return new WaitForSeconds(2f * slowMotionTimeScale); // Adjust wait time for slow motion

    // Reset time scale to normal
    Time.timeScale = 1f;

    // Load the failure scene
    GameManager.instance.GoToFailureScene();
}

private void TriggerFailureScene()
{
    StartCoroutine(SlowMotionAndFailureSequence());
}

    void DetectPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angle = Vector3.Angle(directionToPlayer, transform.forward);

        float heightOffset = 1f;
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
            if (chaseTimer >= lostPlayerTime && !GameManager.instance.IsObjectMadeBig())
            {
                LostPlayer();
            }
        }
        else
        {
            chaseTimer = 0f;
        }
    }

    public void PlayerDetected()
    {
        isChasing = true;
        chaseTimer = 0f;
        StopCoroutine(Patrol());
    }

    public void LostPlayer()
    {
        if (!GameManager.instance.IsObjectMadeBig())
        {
            isChasing = false;
            chaseTimer = 0f;
            StartCoroutine(Patrol());
        }
    }

    void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
        transform.LookAt(player);
    }
}
