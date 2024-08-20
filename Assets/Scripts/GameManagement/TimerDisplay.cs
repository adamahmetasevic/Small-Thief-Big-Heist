using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI regularTimerText;
    [SerializeField] private TextMeshProUGUI victoryTimerText;
    private GameManager gameManager;
    private bool victorySequenceStarted = false;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        // Ensure only the regular timer text is visible initially
        regularTimerText.gameObject.SetActive(true);
        victoryTimerText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameManager != null && !victorySequenceStarted)
        {
            float elapsedTime = gameManager.GetElapsedTime();
            UpdateTimerDisplay(elapsedTime);
        }
    }

    public void DisplayFinalTime(float finalTime)
    {
        int minutes = Mathf.FloorToInt(finalTime / 60f);
        int seconds = Mathf.FloorToInt(finalTime % 60f);
        string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);
        victoryTimerText.text = "Heist Successful! \n Final Time: " + timerString;
    }

    private void UpdateTimerDisplay(float elapsedTime)
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);
        regularTimerText.text = timerString;
    }

    public void VictorySequenceStarted()
    {
        victorySequenceStarted = true;

        // Hide the regular timer and show the victory timer
        regularTimerText.gameObject.SetActive(false);
        victoryTimerText.gameObject.SetActive(true);

        // Optionally, set the final time on the victory timer
        float finalTime = gameManager.GetElapsedTime();
        DisplayFinalTime(finalTime);
    }
}
