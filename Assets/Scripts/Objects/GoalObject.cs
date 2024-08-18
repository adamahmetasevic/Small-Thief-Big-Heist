using UnityEngine;

public class GoalObject : InteractableObject
{
    private bool isLarge = false;
    private Rigidbody rb;

    protected override void Start()
    {
        base.Start(); // Call the Start method of the parent class
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true; // Start immovable
    }

    public override void ChangeSize(float multiplier)
    {
        base.ChangeSize(multiplier);
        
        // Use the getter method to access originalSize
        if (transform.localScale.x >= GetOriginalSize().x * maxSizeMultiplier)
        {
            isLarge = true;
            rb.isKinematic = false; // Make it moveable
            AlertEnemies();
        }
        else
        {
            isLarge = false;
            rb.isKinematic = true; // Make it immovable
        }
    }

    private void AlertEnemies()
    {
        GameManager.instance.AlertAllEnemies();
    }
}