using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : DoorOpen
{
    [SerializeField] private GameObject door;
    [SerializeField] private float proximityRange = 2f; // Adjust the trigger distance

    private bool doorOpen = false;

    void Update()
    {
        // Check for nearby NPCs only if the door is currently closed
        if (!doorOpen)
        {
            CheckForNearbyNPCs();
        }
    }

    private void CheckForNearbyNPCs()
    {
        // Use OverlapSphere to find all colliders within the proximity range
        Collider[] colliders = Physics.OverlapSphere(transform.position, proximityRange);

        // Check if any of the colliders belong to an NPC
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("NPC")) // Assuming your NPCs have the tag "NPC"
            {
                OpenDoor();
                break; // Open only once even if multiple NPCs are nearby
            }
        }
    }

    protected override void OpenDoor()
    {
        doorOpen = true;
        door.GetComponent<Animator>().SetBool("IsOpen", doorOpen);
    }

    // You might also want a CloseDoor() method to handle closing:
    public void CloseDoor()
    {
        doorOpen = false;
        door.GetComponent<Animator>().SetBool("IsOpen", doorOpen);
    }
}