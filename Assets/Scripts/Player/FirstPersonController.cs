using UnityEngine;
using System.Collections.Generic;
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

    public Camera camera; // needed so the ray can be drawn from the camera
    private Animator animator; // Reference to the Animator component


    [Header("Weapon Settings")]
    [SerializeField] private GameObject sizeGun;
    [SerializeField] private GameObject gravityGun;
    [SerializeField] private Transform gunParent;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private SkinnedMeshRenderer[] handMeshes; 
    private HideArms hideArmsScript;
    private bool isHoldingGun = false; // Declare the variable here

    private GameObject currentGun;
    private Dictionary<int, GameObject> inventory;


     void Awake()
     {
        InitializeInventory();
    hideArmsScript = FindObjectOfType<HideArms>();
     }
    void Start()
    {
        originalScale = transform.localScale;
        originalControllerCenter = controller.center;
        originalControllerHeight = controller.height;

        animator = GetComponent<Animator>(); 

                if (crosshair != null) 
        {
            crosshair.SetActive(false);
        }
    }

void InitializeInventory()
    {
        inventory = new Dictionary<int, GameObject>
        {
            { 1, sizeGun },
            { 2, gravityGun }
        };

        foreach (var gun in inventory.Values)
        {
            if (gun != null)
            {
                gun.transform.SetParent(gunParent, false);
                gun.SetActive(false);
            }
        }
    }

    void Update()
    {
        // Check if grounded
        isGrounded = Physics.CheckSphere(transform.position, groundDistance, groundMask) ||
                     Physics.CheckSphere(transform.position, groundDistance, 1 << LayerMask.NameToLayer("restrictedarea"));
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

        HandleWeaponInput(); 

    }
void HandleWeaponInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipGun(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipGun(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UnequipGun();
        }
    }
public void EquipGun(int gunIndex)
    {
        if (inventory.TryGetValue(gunIndex, out GameObject gun))
        {
            UnequipGun(); 

            currentGun = gun;
            currentGun.SetActive(true);

            if (crosshair != null)
            {
                crosshair.SetActive(true); 
            }
            if (hideArmsScript != null)
            {
                hideArmsScript.ApplyHideMaterial(); 
                isHoldingGun = true;
            }
        }
    }
    public void UnequipGun()
    {
        if (currentGun != null)
        {
            currentGun.SetActive(false);
            currentGun = null;

            if (crosshair != null)
            {
                crosshair.SetActive(false);
            }

            if (hideArmsScript != null)
            {
                hideArmsScript.RestoreOriginalMaterials();
                isHoldingGun = false;
            }
        }
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