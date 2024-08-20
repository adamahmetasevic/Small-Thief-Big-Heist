using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static float finalElapsedTime;

    [SerializeField] private string failureSceneName;
    [SerializeField] private string victorySceneName;

    // Music
    public AudioSource musicSource;
    public List<AudioClip> gameMusic = new List<AudioClip>();
    public AudioClip failureMusic;
    public AudioClip victoryMusic;

    private int currentTrackIndex = 0;

    private bool playerDetected = false;
    private bool objectMadeBig = false;

    private float startTime;
    private bool timerRunning = false;

    private float pausedTimeOffset = 0f;

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

        // Don't destroy the music source when loading new scenes
        DontDestroyOnLoad(musicSource.gameObject);

        ResetTimer();
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

        if (scene.name == failureSceneName)
        {
            PlayMusic(failureMusic);
        }
        
        else
        {
            PlayMusic(gameMusic);
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(failureSceneName);
    }

    public void ResetTimer()
    {
        startTime = Time.time;
        pausedTimeOffset = 0f;
        timerRunning = true;
    }

    public float GetElapsedTime()
    {
        if (timerRunning)
            return Time.time - startTime - pausedTimeOffset;
        else
            return 0f;
    }

    public void StopTimer()
    {
        if (timerRunning)
        {
            pausedTimeOffset += Time.time - startTime;
            timerRunning = false;
        }
    }

    public void ResumeTimer()
    {
        if (!timerRunning)
        {
            startTime = Time.time - pausedTimeOffset;
            timerRunning = true;
        }
    }

    public void LoadMainMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("MainMenu");
    }

    // Function to play music from a list, looping through them
    public void PlayMusic(List<AudioClip> playlist)
    {
        if (playlist == null || playlist.Count == 0) return;

        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        currentTrackIndex = 0;
        musicSource.clip = playlist[currentTrackIndex];
        musicSource.Play();

        StartCoroutine(PlayNextTrack(playlist));
    }

    // Function to play a single AudioClip (not looping)
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        musicSource.clip = clip;
        musicSource.Play();
    }

    // Coroutine to play the next track in the playlist
    private System.Collections.IEnumerator PlayNextTrack(List<AudioClip> playlist)
    {
        while (true)
        {
            yield return new WaitForSeconds(musicSource.clip.length);

            currentTrackIndex = (currentTrackIndex + 1) % playlist.Count;

            musicSource.clip = playlist[currentTrackIndex];
            musicSource.Play();
        }
    }
}
