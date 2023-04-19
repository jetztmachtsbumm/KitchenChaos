using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI keyMoveUpText;
    [SerializeField] private TextMeshProUGUI keyMoveLeftText;
    [SerializeField] private TextMeshProUGUI keyMoveDownText;
    [SerializeField] private TextMeshProUGUI keyMoveRightText;
    [SerializeField] private TextMeshProUGUI keyMoveGamepadText;
    [SerializeField] private TextMeshProUGUI keyInteractText;
    [SerializeField] private TextMeshProUGUI keyAltInteractText;
    [SerializeField] private TextMeshProUGUI keyPauseText;
    [SerializeField] private TextMeshProUGUI keyInteractGamepadText;
    [SerializeField] private TextMeshProUGUI keyAltInteractGamepadText;
    [SerializeField] private TextMeshProUGUI keyPauseGamepadText;

    private void Start()
    {
        Player.Instance.OnKeyRebind += Player_OnKeyRebind;
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
        UpdateVisual();
        Show();
    }

    private void GameManager_OnGameStateChanged(object sender, GameManager.GameState e)
    {
        if (GameManager.Instance.IsCountdownToStartActive())
        {
            Hide();
        }
    }

    private void Player_OnKeyRebind(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        keyMoveUpText.text = Player.Instance.GetBindingText(Player.Binding.MOVE_UP);
        keyMoveDownText.text = Player.Instance.GetBindingText(Player.Binding.MOVE_DOWN);
        keyMoveRightText.text = Player.Instance.GetBindingText(Player.Binding.MOVE_RIGHT);
        keyMoveLeftText.text = Player.Instance.GetBindingText(Player.Binding.MOVE_LEFT);
        keyInteractText.text = Player.Instance.GetBindingText(Player.Binding.INTERACT);
        keyAltInteractText.text = Player.Instance.GetBindingText(Player.Binding.ALT_INTERACT);
        keyPauseText.text = Player.Instance.GetBindingText(Player.Binding.PAUSE_GAME);
        keyInteractGamepadText.text = "X";
        keyAltInteractGamepadText.text = "RT";
        keyPauseGamepadText.text = ">";
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
