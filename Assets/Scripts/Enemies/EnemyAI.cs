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
        return Physics.CheckSphere(player.position, 0.5f, restrictedAreaMask);
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
                Transform targetPoint = patrolPoints[currentPatrolIndex];
                float distanceToTarget = Vector3.Distance(transform.position, targetPoint.position);
                
                if (distanceToTarget > 0.5f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

                    Vector3 directionToTarget = targetPoint.position - transform.position;
                    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                    targetRotation.x = 0;
                    targetRotation.z = 0;

                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                }
                else
                {
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                }
            }
            yield return null;
        }
    }

    private void TriggerFailureScene()
    {
        GameManager.instance.GoToFailureScene();
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
