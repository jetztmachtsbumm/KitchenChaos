using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyBrowserTemplate : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyPlayerCountText;

    private Lobby lobby;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => LobbyManager.Instance.JoinWithLobbyId(lobby.Id));
    }

    public void SetLobby(Lobby lobby)
    {
        this.lobby = lobby;
        lobbyNameText.text = lobby.Name;
        lobbyPlayerCountText.text = "Players: " + lobby.Players.Count + "/" + lobby.MaxPlayers;
    }

}
