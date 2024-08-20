using UnityEngine;
using UnityEngine.SceneManagement;

public class FailureManager : MonoBehaviour
{
    // Call this method to load the failure screen
    public void LoadFailureScreen()
    {
        SceneManager.LoadScene("Failure");
    }

    // Call this method to restart the game or go to the main menu
    public void RestartGame()
    {
        SceneManager.LoadScene("MainLevel"); // Replace "MainScene" with the name of your main level scene
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Replace "MainMenu" with the name of your main menu scene
    }
}
