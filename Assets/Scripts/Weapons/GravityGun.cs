using UnityEngine;

public class GravityGun : MonoBehaviour
{
    public float maxDistance = 10f;          // Maximum distance the gun can reach
    public float scrollSpeed = 1f;           // Speed of adjustment when scrolling
    public float hoverDistance = 2f;         // Default hover distance from the player
    public float repulsionForce = 5f;        // Force applied to repel the object
    public float crosshairMovementSpeed = 1f; // Speed of movement towards the crosshair direction
    public float releaseForceMultiplier = 2f; // Multiplier to apply force on release
    public Transform gunEnd;                 // End point of the gun, used for raycasting
    public RectTransform crosshair;          // Crosshair UI element
    private GameObject heldObject;
    private float currentDistance;           // Current distance of the object from the player
    private Vector3 lastVelocity;            // Last known velocity of the object when released

    void Start()
    {
        currentDistance = hoverDistance;     // Initialize current distance
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button to pick up or release
        {
            if (heldObject == null)
            {
                TryPickUpObject();
            }
            else
            {
                ReleaseObject();
            }
        }
        else if (Input.GetMouseButtonDown(1)) // Right mouse button to throw
        {
            RepelObject();
        }

        if (heldObject != null)
        {
            HoverObject();
            AdjustDistanceWithScrollWheel();
        }
    }

void TryPickUpObject()
{
    RaycastHit hit;
    if (Physics.Raycast(gunEnd.position, gunEnd.forward, out hit, maxDistance))
    {
        if (hit.rigidbody != null && hit.collider.CompareTag("Interactable"))
        {
            // Always pick up regular "Interactable" objects
            heldObject = hit.rigidbody.gameObject;
            Rigidbody rb = hit.rigidbody;
            rb.useGravity = false;
            rb.drag = 10;
            rb.angularDrag = 10;

            // Additionally, check for GoalObject for the IsLarge condition
            GoalObject goalObject = hit.collider.GetComponent<GoalObject>();
            if (goalObject != null && !goalObject.IsLarge())
            {
                // If it IS a GoalObject but NOT large, release it immediately
                ReleaseObject();
            }
        }
    }
}

    void ReleaseObject()
    {
        if (heldObject != null)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Calculate the last velocity before releasing
                Vector3 releaseVelocity = (heldObject.transform.position - rb.position) / Time.deltaTime;

                rb.useGravity = true;
                rb.drag = 0; // Reset drag
                rb.angularDrag = 0; // Reset angular drag

                // Apply additional force to simulate retained energy
                rb.AddForce(releaseVelocity * releaseForceMultiplier, ForceMode.Impulse);

                heldObject = null;
            }
        }
    }

    void RepelObject()
    {
        if (heldObject != null)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 repelDirection = (heldObject.transform.position - transform.position).normalized;
                rb.AddForce(repelDirection * repulsionForce, ForceMode.Impulse);
                ReleaseObject(); // Release the object after throwing
            }
        }
    }

    void HoverObject()
    {
        if (heldObject != null)
        {
            // Convert crosshair position to world space
            Vector3 crosshairScreenPosition = new Vector3(crosshair.position.x, crosshair.position.y, Camera.main.nearClipPlane + currentDistance);
            Vector3 crosshairWorldPosition = Camera.main.ScreenToWorldPoint(crosshairScreenPosition);

            // Smoothly move the object towards the crosshair world position
            heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, crosshairWorldPosition, Time.deltaTime * crosshairMovementSpeed);
        }
    }

    void AdjustDistanceWithScrollWheel()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            currentDistance += scrollInput * scrollSpeed;
            currentDistance = Mathf.Clamp(currentDistance, 0.5f, maxDistance); // Clamp the distance to avoid going too close or too far
        }
    }
}
