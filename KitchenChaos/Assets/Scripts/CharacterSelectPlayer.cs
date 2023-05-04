using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{

    [SerializeField] private int playerIndex;
    [SerializeField] private Transform readyTransform;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Button kickButton;
    [SerializeField] private TextMeshPro playerNameText;

    private void Awake()
    {
        kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            LobbyManager.Instance.KickPlayer(playerData.playerId.ToString());
            MultiplayerManager.Instance.KickPlayer(playerData.clientId);
        });
    }

    private void Start()
    {
        MultiplayerManager.Instance.OnPlayerDataListChanged += MultiplayerManager_OnPlayerDataListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        UpdatePlayer();
    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnPlayerDataListChanged -= MultiplayerManager_OnPlayerDataListChanged;
        CharacterSelectReady.Instance.OnReadyChanged -= CharacterSelectReady_OnReadyChanged;
    }

    private void MultiplayerManager_OnPlayerDataListChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    public void UpdatePlayer()
    {
        if (MultiplayerManager.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();

            PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);

            readyTransform.gameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));

            playerNameText.text = playerData.playerName.ToString();

            playerVisual.SetPlayerColor(MultiplayerManager.Instance.GetPlayerColor(playerData.colorId));
        }
        else
        {
            Hide();
        }
    }

    public void Show()
    {
        foreach(Transform childTransform in transform)
        {
            if(childTransform != transform)
            {
                childTransform.gameObject.SetActive(true);
            }
        }
    }

    public void Hide()
    {
        foreach (Transform childTransform in transform)
        {
            if (childTransform != transform)
            {
                childTransform.gameObject.SetActive(false);
            }
        }
    }

}
