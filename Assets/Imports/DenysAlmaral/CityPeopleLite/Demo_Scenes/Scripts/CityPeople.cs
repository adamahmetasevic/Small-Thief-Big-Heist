using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CityPeople
{
    public class CityPeople : MonoBehaviour
    {
        public Transform[] patrolPoints;
        public float moveSpeed = 3f;
        public float minWaitTime = 2f;
        public float maxWaitTime = 5f;

        private NavMeshAgent agent;
        private int currentPointIndex = 0;

        private AnimationClip[] myClips;
        private Animator animator;

        // Cache the Animator parameter ID for better performance
        private int speedParameterID;

        void Start()
        {
            animator = GetComponent<Animator>();
            speedParameterID = Animator.StringToHash("Speed"); // Cache the ID

            if (animator != null)
            {
                myClips = animator.runtimeAnimatorController.animationClips;
                PlayAnyClip();
            }

            agent = GetComponent<NavMeshAgent>();
            if (agent != null && patrolPoints.Length > 0)
            {
                agent.speed = moveSpeed;
                StartCoroutine(Patrol());
            }
            else
            {
                Debug.LogError("NavMeshAgent component missing or no patrol points set!");
            }
        }

        void Update()
        {
            // Update animator based on agent's velocity
            if (animator != null && agent != null)
            {
                animator.SetFloat(speedParameterID, agent.velocity.magnitude);
            }
        }

        void PlayAnyClip()
        {
            if (myClips.Length > 0)
            {
                var cl = myClips[Random.Range(0, myClips.Length)];
                animator.CrossFadeInFixedTime(cl.name, 1.0f, -1, Random.value * cl.length);
            }
        }

        IEnumerator Patrol()
        {
            while (true)
            {
                // Set destination for the NavMesh Agent
                agent.SetDestination(patrolPoints[currentPointIndex].position);

                // Wait until the agent reaches the destination or gets stuck
                while (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
                {
                    yield return null;
                }

                // Wait at the patrol point
                yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

                // Choose the next patrol point
                currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;

                // Play a random animation after reaching a point
                PlayAnyClip();
            }
        }
    }
}