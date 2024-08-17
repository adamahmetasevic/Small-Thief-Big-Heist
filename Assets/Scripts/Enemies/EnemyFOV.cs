using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius = 10f;       // How far the AI can see
    [Range(0, 360)]
    public float viewAngle = 90f;        // The angle of the AI's vision cone
    public LayerMask targetMask;         // The layers that the AI can see (e.g., player)
    public LayerMask obstacleMask;       // The layers that block the AI's vision (e.g., walls)

    private PlayerScript playerScript;   // Reference to the player's script
    private bool playerWasVisibleLastFrame = false; // Track visibility across frames 

    void Start()
    {
        // Assuming the player has the tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerScript = player.GetComponent<PlayerScript>();
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure the player object has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (playerScript != null)
        {
            FindVisibleTargets();

            // Set isHidden to true if the player was visible last frame but isn't anymore
            if (playerWasVisibleLastFrame && playerScript.isHidden)
            {
                Debug.Log("Player is no longer detected.");
            }

            playerWasVisibleLastFrame = !playerScript.isHidden;
        }
    }

    public void FindVisibleTargets()
{
    Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
    bool playerFound = false; // Flag to check if the player is found in this frame

    foreach (Collider target in targetsInViewRadius)
    {
        if (target.CompareTag("Player")) // Ensure the target is the player
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            // Draw a line representing the raycast
            Debug.DrawRay(transform.position, directionToTarget * distanceToTarget, Color.red);

            // Check if the angle to the target is within the AI's view angle
            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    // Check if the raycast does not hit the AI itself
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget, obstacleMask))
                    {
                        if (hit.collider.gameObject != gameObject) // Exclude self
                        {
                            // Player is detected, set isHidden to false
                            playerScript.isHidden = false;
                            playerFound = true; // Set the flag
                            Debug.Log("Player is detected by AI");
                        }
                    }
                }
            }
        }
    }

    // If the player was not found in this frame, set isHidden to true
    if (!playerFound && playerWasVisibleLastFrame)
    {
        playerScript.isHidden = true;
        Debug.Log("Player is no longer detected.");
    }
}


}
