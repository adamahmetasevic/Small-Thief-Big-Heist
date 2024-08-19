using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    //what is shown to the player when hovering over an interactable object
    public string promtpMessage;

    //this will be called by the player
    public void BaseOpen()
    {
        OpenDoor();
    }

    protected virtual void OpenDoor()
    {
        //wont do anything because its inheritance 
        //maybe a little overkill for what we need to set it up like this but I like it this way
    }
}
