using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOpen : MonoBehaviour
{
    private Camera cam;

    [SerializeField]
    private float distance = 5f; // how far the player can see infront of them. 
    [SerializeField]
     LayerMask door;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<FirstPersonController>().camera;
    }

    // Update is called once per frame
    void Update()
    {
        //creates a ray that shoots out of the center of the screen to detect for openable objects.
        Ray pRay = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(pRay.origin, pRay.direction * distance); //shows the ray in scene view
        RaycastHit hitInfo; // stores what is being hit by ray
        if (Physics.Raycast(pRay, out hitInfo, distance, door))
        {
            if (hitInfo.collider.GetComponent<DoorOpen>() != null)
            {
                DoorOpen doorOpen = hitInfo.collider.GetComponent<DoorOpen>();
                if(Input.GetKeyDown(KeyCode.E))
                {
                    doorOpen.BaseOpen();
                }
            }
        }
    }
}
