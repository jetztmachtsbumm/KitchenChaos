using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        MultiplayerManager.Instance.OnJoinGameFailed += MultiplayerManager_OnJoinGameFailed;
        LobbyManager.Instance.OnCreateLobbyStarted += LobbyManager_OnCreateLobbyStarted;
        LobbyManager.Instance.OnCreateLobbyFailed += LobbyManager_OnCreateLobbyFailed;
        LobbyManager.Instance.OnJoinStarted += LobbyManager_OnJoinStarted;
        LobbyManager.Instance.OnQuickJoinFailed += LobbyManager_OnQuickJoinFailed;
        LobbyManager.Instance.OnJoinFailed += LobbyManager_OnJoinFailed;

        Hide();
    }

    private void LobbyManager_OnCreateLobbyStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Creating lobby...");
    }

    private void LobbyManager_OnCreateLobbyFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to create lobby!");
    }
    
    private void LobbyManager_OnJoinStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Joining lobby...");
    }
    
    private void LobbyManager_OnQuickJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("No lobby found!");
    }
    
    private void LobbyManager_OnJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to join lobby!");
    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnJoinGameFailed -= MultiplayerManager_OnJoinGameFailed;
    }

    private void MultiplayerManager_OnJoinGameFailed(object sender, System.EventArgs e)
    {
        if(NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("Connection timed out after 10000ms!\nAttempts: 10");
        }
        else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
