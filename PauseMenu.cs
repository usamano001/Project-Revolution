using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenManager : MonoBehaviour
{
    public static bool isPaused = false; // Static variable to track pause state across scripts
    public GameObject pauseMenuUI; // Reference to the pause menu Canvas

    public AudioSource backgroundMusic; // Reference to the background music AudioSource

    void Start()
    {
        // Ensure the pause menu is hidden at the start
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        else
        {
            Debug.LogError("PauseMenuUI is not assigned in the Inspector.");
        }

        // Check for background music assignment
        if (backgroundMusic == null)
        {
            Debug.LogWarning("Background music AudioSource is not assigned in the Inspector.");
        }
    }

    void Update()
    {
        // Toggle pause menu when the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true); // Show the pause menu
        }

        Time.timeScale = 0f; // Pause the game
        isPaused = true; // Update the static pause state

        if (backgroundMusic != null)
        {
            backgroundMusic.Pause(); // Pause background music
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false); // Hide the pause menu
        }

        Time.timeScale = 1f; // Resume the game
        isPaused = false; // Update the static pause state

        if (backgroundMusic != null)
        {
            backgroundMusic.UnPause(); // Resume background music
        }
    }

    public void OnResumeButtonPressed()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            Debug.Log("Game is not paused; no need to resume.");
        }
    }

    public void OnRestartButtonPressed()
    {
        if (isPaused)
        {
            RestartGame();
        }
        else
        {
            Debug.Log("Game is not paused, but restarting anyway.");
            RestartGame();
        }
    }

    public void OnQuitButtonPressed()
    {
        if (isPaused)
        {
            QuitGame();
        }
        else
        {
            Debug.Log("Game is not paused, but quitting anyway.");
            QuitGame();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Ensure the game is running
        isPaused = false; // Reset the static pause state

        if (backgroundMusic != null)
        {
            backgroundMusic.Stop(); // Stop background music
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; // Ensure the game is running
        isPaused = false; // Reset the static pause state

        if (backgroundMusic != null)
        {
            backgroundMusic.Stop(); // Stop background music
        }

        // Load the Main Menu scene
        SceneManager.LoadScene("MainMenu"); // Replace "MainMenu" with the actual scene name
    }
}
