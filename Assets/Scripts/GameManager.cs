using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Static instance of the Game Manager that 
    // can be accessed from anywhere
    public static GameManager instance = null;

    // Player Score
    public int score = 0;
    // High Score
    public int highScore = 0;
    // Level, starting in Level 1
    public int currentLevel = 1;
    // Highest level available in the game
    public int highestLevel = 2;

    // Called when the object is initialized
    void Awake()
    {
        // If it doesn't exist
        if (instance == null)
        {
            // Set instance to the current object
            instance = this;
        }
        // there can only be a single instance of the manager
        else if (instance != this)
        {
            // Destroy the current object, so there is only one manager
            Destroy(this.gameObject);
        }

        // Don't destroy this object when loading scenes
        DontDestroyOnLoad(this.gameObject);
    }

    //Restart game. Refresh previous score and send back to level 1
    public void Reset()
    {
        // Reset the score
        score = 0;

        // Set the current level to 1
        currentLevel = 1;

        // Load corresponding scene (level 1 or "splash screen" scene)
        SceneManager.LoadScene("Level" + currentLevel);
    }

    // Go to the next level
    public void IncreaseLevel()
    {
        if (currentLevel < highestLevel)
        {
            currentLevel++;
        }
        else
        {
            currentLevel = 1;
        }
        SceneManager.LoadScene("Level" + currentLevel);
    }

    // Increase Score
    public void IncreaseScore(int amount)
    {
        score += amount;

        if (score > highScore)
        {
            highScore = score;
        }
    } 
}
