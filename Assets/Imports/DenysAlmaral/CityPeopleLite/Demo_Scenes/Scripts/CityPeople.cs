using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityPeople
{
    public class CityPeople : MonoBehaviour
    {
        public Transform[] patrolPoints;
        public float moveSpeed = 3f;
        public float minWaitTime = 2f;
        public float maxWaitTime = 5f;
        public float reachDistance = 0.5f; 
        public float rotationSpeed = 5f; // Add a rotation speed variable

        private int currentPointIndex = 0;
        private bool isMoving = true; 

        private AnimationClip[] myClips;
        private Animator animator;

        void Start()
        {
            animator = GetComponent<Animator>();

            if (animator != null)
            {
                myClips = animator.runtimeAnimatorController.animationClips;
                PlayAnyClip();
            }

            if (patrolPoints.Length == 0)
            {
                enabled = false; 
            }
            else
            {
                StartCoroutine(FollowPatrolPoints());
            }
        }

        void Update()
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", isMoving ? moveSpeed : 0f); 
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

        IEnumerator FollowPatrolPoints()
        {
            while (true)
            {
                if (isMoving)
                {
                    Vector3 targetPosition = patrolPoints[currentPointIndex].position;

                    // Calculate direction and rotate smoothly (using Vector3.up)
                    Vector3 direction = (targetPosition - transform.position).normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up); 
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);


                    if (Vector3.Distance(transform.position, targetPosition) < reachDistance)
                    {
                        isMoving = false;
                        StartCoroutine(WaitAtPoint());
                    }
                }

                yield return null; 
            }
        }

        IEnumerator WaitAtPoint()
        {
            PlayAnyClip(); 

            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length; 
            isMoving = true;
        }
    }
}