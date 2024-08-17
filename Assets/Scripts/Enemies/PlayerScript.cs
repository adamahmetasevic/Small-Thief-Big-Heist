using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public bool isHidden = true; // Start as hidden

    void Update()
    {
        if (isHidden)
        {
            // Logic for when the player is hidden
           // Debug.Log("Player is hidden");
        }
        else
        {
            // Logic for when the player is visible to the AI
           // Debug.Log("Player is detected!");
        }
    }
}
