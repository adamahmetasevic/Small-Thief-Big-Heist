using UnityEngine;

public enum AIState { Patrolling, Chasing, Searching }

public class AIStateManager : MonoBehaviour
{
    public AIState CurrentState { get; private set; } = AIState.Patrolling; // Start patrolling

    public void ChangeState(AIState newState)
    {
        CurrentState = newState;
    }
}