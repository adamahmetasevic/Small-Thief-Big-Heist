using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : DoorOpen
{
    [SerializeField] private GameObject door;
    [SerializeField] private float proximityRange = 2f; // Adjust the trigger distance
    [SerializeField] private float closeDelay = 2f;    // Delay before closing the door

    private bool doorOpen = false;
    private bool isNpcNear = false; // Flag to track if an NPC is near

    void Update()
    {
        if (!doorOpen)
        {
            CheckForNearbyNPCs();
        }
        else
        {
            // If the door is open, check if we need to close it
            if (!isNpcNear && doorOpen)
            {
                StartCoroutine(CloseDoorAfterDelay());
            }
        }
    }

    private void CheckForNearbyNPCs()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, proximityRange);

        isNpcNear = false; // Reset the flag before checking

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("NPC"))
            {
                isNpcNear = true; // Set the flag if an NPC is found
                OpenDoor();
                break;
            }
        }
    }

    protected override void OpenDoor()
    {
        doorOpen = true;
        door.GetComponent<Animator>().SetBool("IsOpen", doorOpen);
    }

    private IEnumerator CloseDoorAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(closeDelay);

        // Close the door
        CloseDoor();
    }

    public void CloseDoor()
    {
        doorOpen = false;
        door.GetComponent<Animator>().SetBool("IsOpen", doorOpen);
    }
}