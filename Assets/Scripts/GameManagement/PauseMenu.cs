using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameManager gameManager;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Pause game time
        isPaused = true;

        // Show cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

     public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameManager.ResumeTimer(); // Resume the timer
        isPaused = false;

        // Hide cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Reset time scale before loading the main menu
        SceneManager.LoadScene("MainMenu");
    }
}
