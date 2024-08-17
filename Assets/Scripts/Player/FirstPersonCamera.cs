using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public float verticalClampAngle = 80f;
    public float maxLookAngle = 80f;    // Maximum up angle
    public float minLookAngle = -80f;
    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minLookAngle, maxLookAngle); 

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}