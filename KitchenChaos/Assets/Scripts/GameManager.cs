using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
 
    public static GameManager Instance { get; private set; }

    public event EventHandler<GameState> OnGameStateChanged;
    public event EventHandler<bool> OnGamePauseToggled;

    public enum GameState
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    private GameState gameState;
    private float countdownToStartTimer = 3f;                                                             
    private float gamePlayingTimer;                                                                      
    private float gamePlayingTimerMax = 2 * 60;
    private bool gamePaused;
                                                                                                          
    private void Awake()                                                                                  
    {                      
        if(Instance != null)
        {
            Debug.LogWarning("There is more than one GameManager object active in the scene");
            Destroy(Instance);
        }
        Instance = this;

        gameState = GameState.WaitingToStart;                                                              
    }

    private void Start()
    {
        Player.Instance.OnPauseAction += Player_OnPauseAction;
        Player.Instance.OnInteractAction += Player_OnInteractAction;
    }

    private void Update()                                                                                  
    {                                                                                                      
        switch (gameState)                                                                                 
        {                                                                                                  
            case GameState.WaitingToStart:                                                                                          
                break;                                                                                     
            case GameState.CountdownToStart:                                                               
                countdownToStartTimer -= Time.deltaTime;                                                   
                if (countdownToStartTimer <= 0)                                                            
                {                                                                                          
                    gameState = GameState.GamePlaying;
                    gamePlayingTimer = gamePlayingTimerMax;
                    OnGameStateChanged?.Invoke(this, gameState);
                }                                                                                          
                break;                                                                                     
            case GameState.GamePlaying:                                                                    
                gamePlayingTimer -= Time.deltaTime;                                                             
                if (gamePlayingTimer <= 0)                                                                      
                {                                                                                          
                    gameState = GameState.GameOver;
                    OnGameStateChanged?.Invoke(this, gameState);
                }
                break;
            case GameState.GameOver:
                break;
        }
    }

    private void Player_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    private void Player_OnInteractAction(object sender, EventArgs e)
    {
        if(gameState == GameState.WaitingToStart)
        {
            gameState = GameState.CountdownToStart;
            OnGameStateChanged?.Invoke(this, gameState);
        }
    }

    public void TogglePauseGame()
    {
        gamePaused = !gamePaused;
        if (gamePaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1f;
        }
        OnGamePauseToggled?.Invoke(this, gamePaused);
    }

    public bool IsGamePlaying()
    {
        return gameState == GameState.GamePlaying;
    }

    public bool IsCountdownToStartActive()
    {
        return gameState == GameState.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer;
    }

    public bool IsGameOver()
    {
        return gameState == GameState.GameOver;
    }

    public float GetGamePlayingTimerNormalized()
    {
        return gamePlayingTimer / gamePlayingTimerMax;
    }

}
