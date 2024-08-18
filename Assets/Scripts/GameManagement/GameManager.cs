using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isHoldingGun = false;
    private bool playerDetected = false;
    private bool objectMadeBig = false;

    [SerializeField] private GameObject sizeGun;
    [SerializeField] private GameObject gravityGun;
    [SerializeField] private Transform gunParent; // Assign this to your camera or hand transform in the inspector

    [SerializeField] private GameObject crosshair;

    private GameObject currentGun;
    private Dictionary<int, GameObject> inventory;

    private HideArms hideArmsScript;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeInventory();
        hideArmsScript = FindObjectOfType<HideArms>();
    }
void Start()
{
    // Hide crosshair at the start
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

        // Set the initial parent and position/rotation for each gun
        foreach (var gun in inventory.Values)
        {
            if (gun != null)
            {
                gun.transform.SetParent(gunParent, false); // Set parent without changing local transform
                gun.SetActive(false);
            }
        }
    }

    void Update()
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
     public void AlertAllEnemies()
    {
        objectMadeBig = true;
        if (!playerDetected)
        {
            EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
            foreach (EnemyAI enemy in enemies)
            {
                enemy.PlayerDetected();
            }
            playerDetected = true;
        }
    }

    public void PlayerLost()
    {
        if (playerDetected && !objectMadeBig)
        {
            EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
            foreach (EnemyAI enemy in enemies)
            {
                enemy.LostPlayer();
            }
            playerDetected = false;
        }
    }
public void ResetLevel()
{
    // Reset any necessary game state
    isHoldingGun = false;
    playerDetected = false;
    objectMadeBig = false;

    // Unequip the current gun
    UnequipGun();

    // Deactivate existing guns instead of destroying them
    if (inventory != null)
    {
        foreach (var gun in inventory.Values)
        {
            if (gun != null)
            {
                gun.SetActive(false);
            }
        }
    }

    // Reset the inventory (Reactivate guns)
    InitializeInventory();

    // Ensure arms are visible
    if (hideArmsScript != null)
    {
        hideArmsScript.RestoreOriginalMaterials();
    }

    // Hide crosshair
    if (crosshair != null)
    {
        crosshair.SetActive(false);
    }

    // Reset the current gun reference
    currentGun = null;

    // Reload the current scene
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}

void InitializeInventory()
{
    if (inventory == null)
    {
        inventory = new Dictionary<int, GameObject>
        {
            { 1, sizeGun },
            { 2, gravityGun }
        };
    }

    // Set the initial parent and position/rotation for each gun
    foreach (var gun in inventory.Values)
    {
        if (gun != null)
        {
            gun.transform.SetParent(gunParent, false); // Set parent without changing local transform
            gun.SetActive(false);
        }
    }
}



    public bool IsObjectMadeBig()
    {
        return objectMadeBig;
    }

void EquipGun(int gunIndex)
{
    if (inventory.TryGetValue(gunIndex, out GameObject gun))
    {
        UnequipGun(); // Unequip current gun if any

        // Set the gun as a child of the gunParent and enable it
        currentGun = gun;
        currentGun.SetActive(true);
        currentGun.transform.localPosition = Vector3.zero; // Reset local position
        currentGun.transform.localRotation = Quaternion.identity; // Reset local rotation

        // Show crosshair and hide arms when holding a gun
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

void UnequipGun()
{
    if (currentGun != null)
    {
        // Disable the current gun instead of destroying it
        currentGun.SetActive(false);
        currentGun = null;

        // Hide crosshair and show arms when no gun is held
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

}
