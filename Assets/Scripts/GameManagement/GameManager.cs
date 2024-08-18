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

    public void EquipGun(int gunIndex)
    {
        if (inventory.TryGetValue(gunIndex, out GameObject gun))
        {
            UnequipGun(); // Unequip any currently held gun

            currentGun = gun;
            currentGun.SetActive(true);

            if (crosshair != null)
            {
                crosshair.SetActive(true); // Show crosshair when holding a gun
            }

            if (hideArmsScript != null)
            {
                hideArmsScript.ApplyHideMaterial(); // Hide arms when holding a gun
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

    public bool IsObjectMadeBig()
    {
        return objectMadeBig;
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

        // Ensure gunParent is hidden
        if (gunParent != null)
        {
            gunParent.gameObject.SetActive(false);
        }

        // Reset the current gun reference
        currentGun = null;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
