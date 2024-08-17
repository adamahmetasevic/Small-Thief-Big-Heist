using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public float maxSizeMultiplier = 3f;     // Maximum size multiplier
    public float minSizeMultiplier = 1f / 3f; // Minimum size multiplier
    private Vector3 originalSize;            // Original size of the object
    private enum SizeState { Original, Enlarged, Shrunk }
    private SizeState sizeState = SizeState.Original; // Current size state

    void Start()
    {
        // Initialize the original size of the object
        originalSize = transform.localScale;
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
}
