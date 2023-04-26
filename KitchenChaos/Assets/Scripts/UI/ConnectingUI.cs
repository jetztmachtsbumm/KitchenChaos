using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{

    private void Start()
    {
        MultiplayerManager.Instance.OnTryJoinGame += MultiplayerManager_OnTryJoinGame;
        MultiplayerManager.Instance.OnJoinGameFailed += MultiplayerManager_OnJoinGameFailed;
        Hide();
    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnTryJoinGame -= MultiplayerManager_OnTryJoinGame;
        MultiplayerManager.Instance.OnJoinGameFailed -= MultiplayerManager_OnJoinGameFailed;
    }

    private void MultiplayerManager_OnJoinGameFailed(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void MultiplayerManager_OnTryJoinGame(object sender, System.EventArgs e)
    {
        Show();
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
