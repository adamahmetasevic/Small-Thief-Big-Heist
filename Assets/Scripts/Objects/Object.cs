using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public float maxSizeMultiplier = 3f;     // Maximum size multiplier
    public float minSizeMultiplier = 1f / 3f; // Minimum size multiplier
    private Vector3 originalSize;            // Original size of the object
    private enum SizeState { Original, Enlarged, Shrunk }
    private SizeState sizeState = SizeState.Original; // Current size state

    public GameObject dustParticlePrefab;    // Particle effect prefab
    public AudioClip impactSound;             // Sound effect for impact
    public float impactSpeedThreshold = 5f;  // Speed threshold for impact
    private AudioSource audioSource;          // Audio source for playing sounds

    void Start()
{
    // Initialize the original size of the object
    originalSize = transform.localScale;
    audioSource = gameObject.AddComponent<AudioSource>();

    // Set up 3D sound settings
    audioSource.spatialBlend = 1.0f; // 1.0 makes it fully 3D
    audioSource.minDistance = 5.0f;  // Full volume within this distance
    audioSource.maxDistance = 100.0f; // No sound beyond this distance
}


    public void ChangeSize(float multiplier)
    {
        // Calculate the new size based on the multiplier
        Vector3 newSize = transform.localScale * multiplier;

        // Clamp the size within the allowed boundaries
        if (newSize.x > originalSize.x * maxSizeMultiplier)
        {
            newSize = originalSize * maxSizeMultiplier;
        }
        else if (newSize.x < originalSize.x * minSizeMultiplier)
        {
            newSize = originalSize * minSizeMultiplier;
        }

        // Apply the new size
        transform.localScale = newSize;

        // Update the size state
        UpdateSizeState();
    }

    private void UpdateSizeState()
    {
        // Update the size state based on the current size
        if (transform.localScale.x >= originalSize.x * maxSizeMultiplier)
        {
            sizeState = SizeState.Enlarged;
        }
        else if (transform.localScale.x <= originalSize.x * minSizeMultiplier)
        {
            sizeState = SizeState.Shrunk;
        }
        else
        {
            sizeState = SizeState.Original;
        }
    }

    public void ResizeBack()
    {
        // Resize based on the current state
        switch (sizeState)
        {
            case SizeState.Enlarged:
                ChangeSize(1 / maxSizeMultiplier); // Shrink back to original
                break;
            case SizeState.Shrunk:
                ChangeSize(maxSizeMultiplier); // Enlarge back to original
                break;
            default:
                break; // No change if in original state
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the object is enlarged and if it's hitting the ground
        if (sizeState == SizeState.Enlarged && collision.relativeVelocity.magnitude > impactSpeedThreshold && collision.collider.CompareTag("Ground"))
        {
            // Get the contact point of the collision
            ContactPoint contact = collision.contacts[0];
            Vector3 impactPoint = contact.point;

            // Play dust particle effect at the impact point
            if (dustParticlePrefab != null)
            {
                Instantiate(dustParticlePrefab, impactPoint, Quaternion.identity);
            }

            // Play impact sound
            if (impactSound != null)
            {
                audioSource.PlayOneShot(impactSound);
            }

            // Attract enemy AI (commented out for now)
            // AttractEnemyAI();
        }
    }

    // Uncomment this method if you want to use it later
    /*
    private void AttractEnemyAI()
    {
        // Find all enemy AI within the noise radius and notify them
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, noiseRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                // Notify or attract enemy AI
                EnemyAI enemyAI = hitCollider.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.OnAlert(transform.position);
                }
            }
        }
    }
    */
}
