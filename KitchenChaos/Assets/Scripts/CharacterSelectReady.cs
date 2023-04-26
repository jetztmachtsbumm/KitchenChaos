using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{

    public static CharacterSelectReady Instance { get; private set; }

    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("There is more than one CharacterSelectReady object active in the scene!");
            Destroy(gameObject);
        }
        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }

}