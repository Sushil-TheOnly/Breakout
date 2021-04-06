﻿using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Assets.Data.Models;
using System;

public class GameManager : MonoBehaviour
{
    public bool gameOver;
    public static bool loadFromSavedGame;

    [SerializeField] private TextMeshProUGUI scoreDisplay;
    [SerializeField] private TextMeshProUGUI livesDisplay;
    [SerializeField] private TextMeshProUGUI highScoreDisplay;
    [SerializeField] private TMP_InputField labelInputField;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private int totalLives;

    private int numberOfBricks; // number of bricks left in scene
    private int currentScore;
    private int currentLives;
    private int currentHighScore;

    private SavedGameManager savedGameManager;

    private void Awake()
    {
        ResetTimeToNormal();
        InitalizeGame();
        currentHighScore = PlayerPrefs.GetInt("HIGH_SCORE");
        savedGameManager = SavedGameManager.GetInstance();
    }

    private void OnEnable()
    {
        BallBehaviour.OnBallHittingFloor += ProcessOnBallHittingFloor;
        BallBehaviour.OnBallHittingBrick += ProcessOnBallHittingBrick;
    }

    private void OnDisable()
    {
        BallBehaviour.OnBallHittingFloor -= ProcessOnBallHittingFloor;
        BallBehaviour.OnBallHittingBrick -= ProcessOnBallHittingBrick;
    }

    // Start is called before the first frame update
    private void Start()
    {
        scoreDisplay.text = $"Score: {currentScore}";
        livesDisplay.text = $"Lives: {currentLives}";
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    private void InitalizeGame()
    {
        if (loadFromSavedGame)
        {
            currentLives = SavedGameManager.SavedGame.livesRemaining;
            currentScore = SavedGameManager.SavedGame.score;
            // TODO re construct position of brick of each color
        } 
        else
        {
            currentLives = totalLives;
            currentScore = 0;
        }
        numberOfBricks = ComputeNumberOfBricks();
    }

    private int ComputeNumberOfBricks()
    {
        return GameObject.FindGameObjectsWithTag("RedBrick").Length +
               GameObject.FindGameObjectsWithTag("OrangeBrick").Length +
               GameObject.FindGameObjectsWithTag("GreenBrick").Length +
               GameObject.FindGameObjectsWithTag("YellowBrick").Length;
    }

    public void UpdateLives(int delta)
    {
        currentLives += delta;

        if (currentLives <= 0)
        {
            currentLives = 0;
            EndGame();
        }

        livesDisplay.text = $"Lives: {currentLives}";
    }

    public void UpdateScore(int delta)
    {
        currentScore += delta;
        scoreDisplay.text = $"Score: {currentScore}";
    }

    public void PlayAgain()
    {
        ResetTimeToNormal();
        SceneManager.LoadScene(Scenes.GameScene);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(Scenes.MainMenu);
    }

    private void EndGame()
    {
        gameOver = true;
        gameOverPanel.SetActive(true);
        FreezeTime();

        if (currentScore > currentHighScore)
        {
            PlayerPrefs.SetInt("HIGH_SCORE", currentScore);
            highScoreDisplay.text = $"New High Score Achieved!\n {currentScore}";
        }
        else
        {
            highScoreDisplay.text = $"High Score: {currentHighScore}\nYour score: {currentScore}";
        }
    }

    private void PauseGame()
    {
        optionsPanel.SetActive(true);
        FreezeTime();
    }

    public void ResumeGame()
    {
        optionsPanel.SetActive(false);
        ResetTimeToNormal();
    }

    private void ProcessOnBallHittingFloor()
    {
        UpdateLives(-1);
    }

    private void ProcessOnBallHittingBrick(BrickBehaviour brick)
    {
        UpdateScore(brick.points);
        UpdateNumberOfBricks();
    }

    private void UpdateNumberOfBricks()
    {
        numberOfBricks--;

        if (numberOfBricks <= 0)
        {
            EndGame();
        }
    } 

    public void SaveGame()
    {
        SavedGame savedGame = new SavedGame();
        savedGame.label = labelInputField.text;
        savedGame.score = currentScore;
        savedGame.hasHighScore = currentScore >= currentHighScore;
        savedGame.livesRemaining = currentLives;
        savedGame.bricksRemaining = 2;
        savedGame.saveTime = DateTime.Now;
        SavedGameManager.SavedGame = savedGame;
        savedGameManager.Save();

        string currentSaveFiles = PlayerPrefs.GetString("SAVE_FILES", string.Empty);
        string updatedSaveFiles = string.IsNullOrEmpty(currentSaveFiles)
                                  ? savedGame.label
                                  : currentSaveFiles + "," + savedGame.label;
        PlayerPrefs.SetString("SAVE_FILES", updatedSaveFiles);
        PlayerPrefs.Save();
        Debug.Log("Game saved");
    }

    public static void FreezeTime()
    {
        Time.timeScale = 0.0f;
    }

    public static void ResetTimeToNormal()
    {
        if (Time.timeScale != 1.0f)
        {
            Time.timeScale = 1.0f;
        }
    }
}
