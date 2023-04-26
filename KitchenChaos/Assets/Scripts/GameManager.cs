using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
 
    public static GameManager Instance { get; private set; }

    public event EventHandler<GameState> OnGameStateChanged;
    public event EventHandler<bool> OnLocalGamePaused;
    public event EventHandler<bool> OnMultiplayerGamePauseToggled;
    public event EventHandler OnLocalPlayerReadyChanged;

    public enum GameState
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    [SerializeField] private Transform playerPrefab; 

    private NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.WaitingToStart);
    private bool isLocalPlayerReady;
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);                                                             
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);               
    private float gamePlayingTimerMax = 90f;
    private bool isLocalgamePaused;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPausedDictionary;
    private bool autoTestGamePaused;
                                                                                                          
    private void Awake()                                                                                  
    {                      
        if(Instance != null)
        {
            Debug.LogWarning("There is more than one GameManager object active in the scene");
            Destroy(Instance);
        }
        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPausedDictionary = new Dictionary<ulong, bool>();
    }

    public override void OnNetworkSpawn()
    {
        gameState.OnValueChanged += GameState_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

        if (IsServer)
        {
            NetworkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach(ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void LateUpdate()
    {
        if (autoTestGamePaused)
        {
            autoTestGamePaused = false;
            TestGamePausedState();
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        autoTestGamePaused = true;
    }

    private void GameState_OnValueChanged(GameState previousValue, GameState newValue)
    {
        OnGameStateChanged?.Invoke(this, gameState.Value);
    }

    private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        if (isGamePaused.Value)
        {
            Time.timeScale = 0;
            OnMultiplayerGamePauseToggled?.Invoke(this, true);
        }
        else
        {
            Time.timeScale = 1f;
            OnMultiplayerGamePauseToggled?.Invoke(this, false);
        }
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
        isLocalgamePaused = !isLocalgamePaused;
        if (isLocalgamePaused)
        {
            PauseGameServerRpc();
        }
        else
        {
            UnPauseGameServerRpc();
        }
        OnLocalGamePaused?.Invoke(this, isLocalgamePaused);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;
        TestGamePausedState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnPauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;
        TestGamePausedState();
    }

    private void TestGamePausedState()
    {
        foreach(ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            if(playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId])
            {
                isGamePaused.Value = true;
                return;
            }
        }

        isGamePaused.Value = false;
    }

    public bool IsGamePlaying()
    {
        return gameState.Value == GameState.GamePlaying;
    }

    public bool IsWaitingToStart()
    {
        return gameState.Value == GameState.WaitingToStart;
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
