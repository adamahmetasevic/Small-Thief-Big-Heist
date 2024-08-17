using UnityEngine;

public class AIDetection : MonoBehaviour
{
    private FieldOfView fov;
    private AIStateManager stateManager;

    void Start()
    {
        fov = GetComponent<FieldOfView>();
        stateManager = GetComponent<AIStateManager>();
    }

    void Update()
    {
        // Only call FindVisibleTargets if the AI is in a detection state
        if (stateManager.CurrentState == AIState.Chasing || // Add other relevant states
            stateManager.CurrentState == AIState.Searching) 
        {
            fov.FindVisibleTargets(); 
        }
    }
}