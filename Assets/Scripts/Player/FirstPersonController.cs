using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    private Vector3 targetScale;
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public CharacterController controller; // Assign in Inspector
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float sprintSpeed = 10f;
    private Vector3 velocity;
    private bool isGrounded;

    public float shrinkFactor = 10f; 
    private Vector3 originalScale;
    private Vector3 originalControllerCenter;
    private float originalControllerHeight;
    private bool isShrinking = false;

    private Animator animator; // Reference to the Animator component

    void Start()
    {
        originalScale = transform.localScale;
        originalControllerCenter = controller.center;
        originalControllerHeight = controller.height;

        animator = GetComponent<Animator>(); // Get the Animator component
    }

    void Update()
    {
        // Check if grounded
        isGrounded = Physics.CheckSphere(transform.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small negative value to prevent falling through
        }

        // Handle shrinking
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ToggleShrink();
        }

        // Get movement input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement speed
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        Vector3 moveDirection = transform.right * horizontalInput + transform.forward * verticalInput;
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        float movementSpeed = moveDirection.magnitude;
        animator.SetFloat("Speed", movementSpeed);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Notify GameManager if player is hiding
        CheckIfHiding();
    }

    void ToggleShrink()
    {
        if (transform.localScale == originalScale)
        {
            transform.localScale = originalScale / shrinkFactor;
            controller.center = originalControllerCenter / shrinkFactor;
            controller.height = originalControllerHeight / shrinkFactor;
        }
        else
        {
            transform.localScale = originalScale;
            controller.center = originalControllerCenter;
            controller.height = originalControllerHeight;
        }
    }

    void CheckIfHiding()
{
    RaycastHit hit;
    Vector3 rayOrigin = transform.position + Vector3.up * controller.height / 2;
    if (Physics.Raycast(rayOrigin, transform.forward, out hit, 2f))
    {
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            // Notify GameManager that player is hiding
            GameManager.instance.PlayerLost();
        }
        else
        {
            // Notify GameManager that player is detected
            GameManager.instance.AlertAllEnemies();
        }
    }
}
}
