using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{

    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinWithCodeButton;
    [SerializeField] private TMP_InputField lobbyCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private Transform lobbyBrowser;
    [SerializeField] private Transform lobbyBrowserTemplate;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() => {
            LobbyManager.Instance.LeaveLobby();
            Loader.Load(Loader.Scene.MainMenu);
        });

        createLobbyButton.onClick.AddListener(() => lobbyCreateUI.Show());

        quickJoinButton.onClick.AddListener(() => LobbyManager.Instance.QuickJoin());

        joinWithCodeButton.onClick.AddListener(() => LobbyManager.Instance.JoinWithCode(lobbyCodeInputField.text));

        lobbyBrowserTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        playerNameInputField.text = MultiplayerManager.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((string newText) => MultiplayerManager.Instance.SetPlayerName(newText));

        LobbyManager.Instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }

    private void LobbyManager_OnLobbyListChanged(object sender, List<Lobby> availableLobbies)
    {
        UpdateLobbyList(availableLobbies);        
    }

    private void UpdateLobbyList(List<Lobby> availableLobbies)
    {
        foreach(Transform child in lobbyBrowser)
        {
            if (child == lobbyBrowserTemplate) continue;

            Destroy(child.gameObject);
        }

        foreach(Lobby lobby in availableLobbies)
        {
            Transform lobbyTransform = Instantiate(lobbyBrowserTemplate, lobbyBrowser);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyBrowserTemplate>().SetLobby(lobby);
        }
    }

}
