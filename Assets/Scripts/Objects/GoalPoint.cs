using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GoalPoint : MonoBehaviour
{
    [SerializeField] private string victorySceneName; // Name of the victory scene
    [SerializeField] private float delay = 2f; // Delay in seconds before loading the victory scene
    [SerializeField] private float slowMotionTimeScale = 0.2f; // Time scale during slow motion
    [SerializeField] private AudioClip victorySound; // Sound effect to play

    [SerializeField] private GameManager gameManager; // Reference to the GameManager instance

    private AudioSource audioSource; // Reference to the AudioSource component

    private void Awake()
    {
        // Get the AudioSource component (or add one if it doesn't exist)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Find the GameManager instance
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the GoalObject
        GoalObject goalObject = other.GetComponent<GoalObject>();
        if (goalObject != null && goalObject.IsLarge())
        {
            // Start the victory sequence coroutine
            StartCoroutine(VictorySequence());
        }
    }
public void NotifyTimerDisplayOfVictorySequence()
{
    TimerDisplay timerDisplay = FindObjectOfType<TimerDisplay>();
    if (timerDisplay != null)
    {
        timerDisplay.VictorySequenceStarted();
    }
    else
    {
        Debug.LogWarning("TimerDisplay script not found in the scene.");
    }
}
    

private IEnumerator VictorySequence()
{
    // Notify the TimerDisplay script that the victory sequence has started
    NotifyTimerDisplayOfVictorySequence();

    // Stop the timer
    gameManager.StopTimer();

        // Play the victory sound effect
        audioSource.PlayOneShot(victorySound);

        // Slow down time
        Time.timeScale = slowMotionTimeScale;

        // Wait for the specified delay
        yield return new WaitForSeconds(delay * slowMotionTimeScale); // Adjust wait time for slow motion

        // Reset time scale to normal
        Time.timeScale = 1f;

        // Load the victory scene and pass the elapsed time
        gameManager.LoadMainMenu();
    }
}