using UnityEngine;

public class SizeChangerGun : MonoBehaviour
{
    public float enlargeMultiplier = 3f;       // Multiplier for enlarging the object
    public float shrinkMultiplier = 1f / 3f;   // Multiplier for shrinking the object
    public float maxDistance = 10f;            // Maximum distance to interact with objects
    public RectTransform crosshair;            // Crosshair UI element
    private InteractableObject interactedObjectScript;
    private bool hasBeenEnlarged = false;      // Track if the object has been enlarged
    private bool hasBeenShrunk = false;        // Track if the object has been shrunk

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // M1 for enlarging
        {
            if (interactedObjectScript != null)
            {
                if (!hasBeenEnlarged)
                {
                    interactedObjectScript.ChangeSize(enlargeMultiplier);
                    hasBeenEnlarged = true;
                    hasBeenShrunk = false;
                }
            }
        }
        else if (Input.GetMouseButtonDown(1)) // M2 for shrinking
        {
            if (interactedObjectScript != null)
            {
                if (!hasBeenShrunk)
                {
                    interactedObjectScript.ChangeSize(shrinkMultiplier);
                    hasBeenShrunk = true;
                    hasBeenEnlarged = false;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.R)) // Press 'R' to reset interaction
        {
            ResetInteraction();
        }
        else if (Input.GetKeyDown(KeyCode.B)) // Press 'B' to resize back
        {
            if (interactedObjectScript != null)
            {
                interactedObjectScript.ResizeBack();
                hasBeenEnlarged = false;
                hasBeenShrunk = false;
            }
        }

        SelectObjectWithCrosshair();
    }

    void SelectObjectWithCrosshair()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(crosshair.position);
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                interactedObjectScript = hit.collider.GetComponent<InteractableObject>();

                // Reset size states when a new object is interacted with
                hasBeenEnlarged = false;
                hasBeenShrunk = false;
            }
        }
        else
        {
            interactedObjectScript = null; // Clear interaction if nothing is hit
        }
    }

    void ResetInteraction()
    {
        interactedObjectScript = null;
        hasBeenEnlarged = false;
        hasBeenShrunk = false;
    }
}
