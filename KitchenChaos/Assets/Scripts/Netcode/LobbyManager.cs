using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{

    public static LobbyManager Instance { get; private set; }

    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler OnJoinFailed;
    public event EventHandler<List<Lobby>> OnLobbyListChanged;

    private Lobby joinedLobby;
    private float heartBeatTimer;
    private float lobbyListTimer;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("There is more than one LobbyManager object active in the scene!");
            Destroy(gameObject);
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
        InitializeUnityAuthentication();
    }

    private void Update()
    {
        LobbyHeartbeat();
        HandleLobbyList();
    }

    private void LobbyHeartbeat()
    {
        if (IsLobbyHost())
        {
            heartBeatTimer -= Time.deltaTime;
            if(heartBeatTimer <= 0f)
            {
                float heartBeatTimerMax = 15f;
                heartBeatTimer = heartBeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private void HandleLobbyList()
    {
        if (joinedLobby == null && AuthenticationService.Instance.IsSignedIn)
        {
            lobbyListTimer -= Time.deltaTime;
            if (lobbyListTimer <= 0)
            {
                float lobbyListTimerMax = 3f;
                lobbyListTimer = lobbyListTimerMax;

                ListLobbies();
            }
        }
    }

    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            OnLobbyListChanged?.Invoke(this, queryResponse.Results);
        } catch(LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    private async void InitializeUnityAuthentication()
    {
        if(UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

            await UnityServices.InitializeAsync();

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
        try 
        { 
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MultiplayerManager.MAX_PLAYERS, new CreateLobbyOptions
            {
                IsPrivate = isPrivate
            });

            MultiplayerManager.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        } catch (LobbyServiceException e)
        {
            Debug.LogException(e);
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void QuickJoin()
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            MultiplayerManager.Instance.StartClient();
        } catch (LobbyServiceException e)
        {
            Debug.LogException(e);
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void JoinWithCode(string lobbyCode)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            MultiplayerManager.Instance.StartClient();
        } catch(LobbyServiceException e)
        {
            Debug.LogException(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public async void JoinWithLobbyId(string lobbyId)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            MultiplayerManager.Instance.StartClient();
        } catch(LobbyServiceException e)
        {
            Debug.LogException(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void DeleteLobby()
    {
        if(joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
            } catch(LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }
    }

    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

            joinedLobby = null; 
        } catch(LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }
    
    public async void KickPlayer(string playerId)
    {
        if (IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }
    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }

}
