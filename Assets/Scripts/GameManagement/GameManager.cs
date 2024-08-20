using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
        public static float finalElapsedTime;

    public bool isHoldingGun = false;
    private bool playerDetected = false;
    private bool objectMadeBig = false;

    [SerializeField] private GameObject sizeGun;
    [SerializeField] private GameObject gravityGun;
    [SerializeField] private Transform gunParent;

    [SerializeField] private GameObject crosshair;
    [SerializeField] private string failureSceneName; // Name of the failure scene
    [SerializeField] private string victorySceneName; // Name of the victory scene

    private GameObject currentGun;
    private Dictionary<int, GameObject> inventory;

    private HideArms hideArmsScript;

    private float startTime;
    private bool timerRunning = false;

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
        if (crosshair != null)
        {
            crosshair.SetActive(false);
        }

        ResetTimer();
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
        if (timerRunning)
        {
            // Update the timer here
        }

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

    public void GoToFailureScene()
    {
        // Load the failure scene
        SceneManager.LoadScene(failureSceneName);
    } 

    public void ResetTimer()
    {
        startTime = Time.time;
        timerRunning = true;
    }

    public float GetElapsedTime()
    {
        if (timerRunning)
            return Time.time - startTime;
        else
            return 0f;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

