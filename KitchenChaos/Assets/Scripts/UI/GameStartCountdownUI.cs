using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStartCountdownUI : MonoBehaviour
{

    private TextMeshProUGUI countdownText;

    private void Awake()
    {
        countdownText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged; 
        countdownText.gameObject.SetActive(false);
    }

    private void Update()
    {
        countdownText.text = Mathf.Ceil(GameManager.Instance.GetCountdownToStartTimer()).ToString();
    }

    private void GameManager_OnGameStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsCountdownToStartActive())
        {
            countdownText.gameObject.SetActive(true);
        }
        else
        {
            countdownText.gameObject.SetActive(false);
        }
    }
}
