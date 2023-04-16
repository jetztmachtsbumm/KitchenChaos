using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() => GameManager.Instance.TogglePauseGame());
        mainMenuButton.onClick.AddListener(() => Loader.Load(Loader.Scene.MainMenu));
    }

    private void Start()
    {
        GameManager.Instance.OnGamePauseToggled += GameManager_OnGamePauseToggled;
        Hide();
    }

    private void GameManager_OnGamePauseToggled(object sender, bool e)
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

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

}
