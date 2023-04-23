using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{

    public static TutorialUI Instance { get; private set; }

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

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("There is more than one TutorialUI object active in the scene!");
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerReadyChanged;
    }

    private void GameManager_OnLocalPlayerReadyChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsLocalPlayerReady())
        {
            Hide();
        }
    }

    public void Player_OnKeyRebind(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        keyMoveUpText.text = Player.LocalInstance.GetBindingText(Player.Binding.MOVE_UP);
        keyMoveDownText.text = Player.LocalInstance.GetBindingText(Player.Binding.MOVE_DOWN);
        keyMoveRightText.text = Player.LocalInstance.GetBindingText(Player.Binding.MOVE_RIGHT);
        keyMoveLeftText.text = Player.LocalInstance.GetBindingText(Player.Binding.MOVE_LEFT);
        keyInteractText.text = Player.LocalInstance.GetBindingText(Player.Binding.INTERACT);
        keyAltInteractText.text = Player.LocalInstance.GetBindingText(Player.Binding.ALT_INTERACT);
        keyPauseText.text = Player.LocalInstance.GetBindingText(Player.Binding.PAUSE_GAME);
        keyInteractGamepadText.text = "X";
        keyAltInteractGamepadText.text = "RT";
        keyPauseGamepadText.text = ">";
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
