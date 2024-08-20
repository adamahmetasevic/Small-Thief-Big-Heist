using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static float finalElapsedTime; 

    [SerializeField] private string failureSceneName; // Name of the failure scene
    [SerializeField] private string victorySceneName; // Name of the victory scene

    private bool playerDetected = false;
    private bool objectMadeBig = false;

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

        ResetTimer(); // Start the timer when the game starts
    }

    void Update()
    {
        if (timerRunning)
        {
            // Update the timer here (e.g., display it on the UI)
        }
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetTimer(); 
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
        finalElapsedTime = GetElapsedTime(); // Store the final time
    }

    public void LoadMainMenu()
    {
        // Ensure cursor is visible and unlocked before switching to the main menu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("MainMenu");
    }
}