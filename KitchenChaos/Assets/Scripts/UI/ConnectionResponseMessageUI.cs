using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseMessageUI : MonoBehaviour
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
        Hide();
    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnJoinGameFailed -= MultiplayerManager_OnJoinGameFailed;
    }

    private void MultiplayerManager_OnJoinGameFailed(object sender, System.EventArgs e)
    {
        Show();
        messageText.text = NetworkManager.Singleton.DisconnectReason;

        if(messageText.text == "")
        {
            messageText.text = "Connection timed out after 10000ms!\nAttempts: 10";
        }
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
