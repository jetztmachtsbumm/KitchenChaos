using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
 
    public static GameManager Instance { get; private set; }

    public event EventHandler OnGameStateChanged;

    private enum GameState
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    private GameState gameState;
    private float waitingToStartTimer = 1f;
    private float countdownToStartTimer = 3f;                                                             
    private float gamePlayingTimer;                                                                      
    private float gamePlayingTimerMax = 10f;                                                                      
                                                                                                          
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
                                                                                                           
    private void Update()                                                                                  
    {                                                                                                      
        switch (gameState)                                                                                 
        {                                                                                                  
            case GameState.WaitingToStart:                                                                 
                waitingToStartTimer -= Time.deltaTime;                                                     
                if(waitingToStartTimer <= 0)                                                               
                {                                                                                          
                    gameState = GameState.CountdownToStart;
                    OnGameStateChanged?.Invoke(this, EventArgs.Empty);
                }                                                                                          
                break;                                                                                     
            case GameState.CountdownToStart:                                                               
                countdownToStartTimer -= Time.deltaTime;                                                   
                if (countdownToStartTimer <= 0)                                                            
                {                                                                                          
                    gameState = GameState.GamePlaying;
                    gamePlayingTimer = gamePlayingTimerMax;
                    OnGameStateChanged?.Invoke(this, EventArgs.Empty);
                }                                                                                          
                break;                                                                                     
            case GameState.GamePlaying:                                                                    
                gamePlayingTimer -= Time.deltaTime;                                                             
                if (gamePlayingTimer <= 0)                                                                      
                {                                                                                          
                    gameState = GameState.GameOver;
                    OnGameStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case GameState.GameOver:
                break;
        }
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
