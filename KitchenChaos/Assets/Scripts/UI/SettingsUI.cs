using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{

    public static SettingsUI Instance { get; private set; }

    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button altInteractButton;
    [SerializeField] private Button pauseGameButton;
    [SerializeField] private TextMeshProUGUI soundEffectsText;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI altInteractText;
    [SerializeField] private TextMeshProUGUI pauseGameText;
    [SerializeField] private Transform pressToRebindKeyTransform;

    private Action onSettingsMenuClosed;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("There is more than one SettingsUI object active in the scene!");
            Destroy(Instance);
        }
        Instance = this;

        soundEffectsButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        backButton.onClick.AddListener(() =>
        {
            Hide();
            onSettingsMenuClosed();
        });

        moveUpButton.onClick.AddListener(() => RebindBinding(Player.Binding.MOVE_UP));
        moveDownButton.onClick.AddListener(() => RebindBinding(Player.Binding.MOVE_DOWN));
        moveRightButton.onClick.AddListener(() => RebindBinding(Player.Binding.MOVE_RIGHT));
        moveLeftButton.onClick.AddListener(() => RebindBinding(Player.Binding.MOVE_LEFT));
        interactButton.onClick.AddListener(() => RebindBinding(Player.Binding.INTERACT));
        altInteractButton.onClick.AddListener(() => RebindBinding(Player.Binding.ALT_INTERACT));
        pauseGameButton.onClick.AddListener(() => RebindBinding(Player.Binding.PAUSE_GAME));
    }

    private void Start()
    {
        GameManager.Instance.OnGamePauseToggled += GameManager_OnGamePauseToggled;
        UpdateVisual();
        Hide();
        HidePressToRebindKey();
    }

    private void GameManager_OnGamePauseToggled(object sender, bool e)
    {
        if (!e)
        {
            Hide();
        }
    }

    private void UpdateVisual()
    {
        soundEffectsText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f).ToString();
        musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f).ToString();

        moveUpText.text = Player.Instance.GetBindingText(Player.Binding.MOVE_UP);
        moveDownText.text = Player.Instance.GetBindingText(Player.Binding.MOVE_DOWN);
        moveRightText.text = Player.Instance.GetBindingText(Player.Binding.MOVE_RIGHT);
        moveLeftText.text = Player.Instance.GetBindingText(Player.Binding.MOVE_LEFT);
        interactText.text = Player.Instance.GetBindingText(Player.Binding.INTERACT);
        altInteractText.text = Player.Instance.GetBindingText(Player.Binding.ALT_INTERACT);
        pauseGameText.text = Player.Instance.GetBindingText(Player.Binding.PAUSE_GAME);
    }

    private void RebindBinding(Player.Binding binding)
    {
        ShowPressToRebindKey();
        Player.Instance.RebindBinding(binding, () => {
            HidePressToRebindKey();
            UpdateVisual();
        });
    }

    public void Show(Action onSettingsMenuClosed)
    {
        this.onSettingsMenuClosed = onSettingsMenuClosed;

        gameObject.SetActive(true);

        soundEffectsButton.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowPressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(true);
    }

    private void HidePressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }

}
