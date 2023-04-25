using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuMultiplayerUI : MonoBehaviour
{

    private void Start()
    {
        GameManager.Instance.OnMultiplayerGamePauseToggled += GameManager_OnMultiplayerGamePauseToggled;
        Hide();
    }

    private void GameManager_OnMultiplayerGamePauseToggled(object sender, bool e)
    {
        if (e)
        {
            Show();
        }
        else
        {
            Hide();
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
