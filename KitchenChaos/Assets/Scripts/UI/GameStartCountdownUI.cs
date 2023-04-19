using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStartCountdownUI : MonoBehaviour
{

    private TextMeshProUGUI countdownText;
    private Animator animator;
    private int previousCountdownNumber;

    private void Awake()
    {
        countdownText = GetComponentInChildren<TextMeshProUGUI>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged; 
        countdownText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance.IsCountdownToStartActive())
        {
            int countdownNumber = Mathf.CeilToInt(GameManager.Instance.GetCountdownToStartTimer());
            countdownText.text = countdownNumber.ToString();

            if (previousCountdownNumber != countdownNumber)
            {
                previousCountdownNumber = countdownNumber;
                animator.SetTrigger("NumberPopup");
                SoundManager.Instance.PlayCountdownSound();
            }
        }
    }

    private void GameManager_OnGameStateChanged(object sender, GameManager.GameState e)
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
