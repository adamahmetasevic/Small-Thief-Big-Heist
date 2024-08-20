using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 

public class GoalPoint : MonoBehaviour
{
    [SerializeField] private string victorySceneName; // Name of the victory scene
    [SerializeField] private float delay = 2f; // Delay in seconds before loading the victory scene
    [SerializeField] private float slowMotionTimeScale = 0.2f; // Time scale during slow motion
    [SerializeField] private AudioClip victorySound; // Sound effect to play

    private AudioSource audioSource; // Reference to the AudioSource component

    private void Awake()
    {
        // Get the AudioSource component (or add one if it doesn't exist)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
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

    private IEnumerator VictorySequence()
    {
        // Play the victory sound effect
        audioSource.PlayOneShot(victorySound);

        // Slow down time
        Time.timeScale = slowMotionTimeScale;

        // Wait for the specified delay
        yield return new WaitForSeconds(delay * slowMotionTimeScale); // Adjust wait time for slow motion

        // Reset time scale to normal
        Time.timeScale = 1f;

        // Load the victory scene
        SceneManager.LoadScene(victorySceneName);
    }
}