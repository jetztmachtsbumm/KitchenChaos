using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
 
    public static GameManager Instance { get; private set; }

    public event EventHandler<GameState> OnGameStateChanged;
    public event EventHandler<bool> OnGamePauseToggled;
    public event EventHandler OnLocalPlayerReadyChanged;

    public enum GameState
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    private NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.WaitingToStart);
    private bool isLocalPlayerReady;
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);                                                             
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);               
    private float gamePlayingTimerMax = 90f;
    private bool gamePaused;
    private Dictionary<ulong, bool> playerReadyDictionary;
                                                                                                          
    private void Awake()                                                                                  
    {                      
        if(Instance != null)
        {
            Debug.LogWarning("There is more than one GameManager object active in the scene");
            Destroy(Instance);
        }
        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public override void OnNetworkSpawn()
    {
        gameState.OnValueChanged += GameState_OnValueChanged;
    }

    private void GameState_OnValueChanged(GameState previousValue, GameState newValue)
    {
        OnGameStateChanged?.Invoke(this, gameState.Value);
    }

    private void Update()                                                                                  
    {
        if (!IsServer)
        {
            return;
        }

        switch (gameState.Value)                                                                                 
        {                                                                                                  
            case GameState.WaitingToStart:                                                                                          
                break;                                                                                     
            case GameState.CountdownToStart:                                                               
                countdownToStartTimer.Value -= Time.deltaTime;                                                   
                if (countdownToStartTimer.Value <= 0)                                                            
                {                                                                                          
                    gameState.Value = GameState.GamePlaying;
                    gamePlayingTimer.Value = gamePlayingTimerMax;
                }                                                                                          
                break;                                                                                     
            case GameState.GamePlaying:                                                                    
                gamePlayingTimer.Value -= Time.deltaTime;                                                             
                if (gamePlayingTimer.Value <= 0)                                                                      
                {                                                                                          
                    gameState.Value = GameState.GameOver;
                }
                break;
            case GameState.GameOver:
                break;
        }
    }

    public void Player_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    public void Player_OnInteractAction(object sender, EventArgs e)
    {
        if(gameState.Value == GameState.WaitingToStart)
        {
            isLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            gameState.Value = GameState.CountdownToStart;
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
        return gameState.Value == GameState.GamePlaying;
    }

    public bool IsCountdownToStartActive()
    {
        return gameState.Value == GameState.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }

    public bool IsGameOver()
    {
        return gameState.Value == GameState.GameOver;
    }

    public float GetGamePlayingTimerNormalized()
    {
        return gamePlayingTimer.Value / gamePlayingTimerMax;
    }

    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }

}
