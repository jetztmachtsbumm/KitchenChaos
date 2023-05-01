using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectUITemplate : MonoBehaviour
{

    [SerializeField] private int colorId;
    [SerializeField] private Image image;
    [SerializeField] private Transform selectedTransform;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.ChangePlayerColor(colorId);
        });
    }

    private void Start()
    {
        MultiplayerManager.Instance.OnPlayerDataListChanged += MultiplayerManager_OnPlayerDataListChanged;
        image.color = MultiplayerManager.Instance.GetPlayerColor(colorId);
        UpdateIsSelected();
    }
    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnPlayerDataListChanged -= MultiplayerManager_OnPlayerDataListChanged;
    }


    private void MultiplayerManager_OnPlayerDataListChanged(object sender, System.EventArgs e)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if(MultiplayerManager.Instance.GetPlayerData().colorId == colorId)
        {
            selectedTransform.gameObject.SetActive(true);
        }
        else
        {
            selectedTransform.gameObject.SetActive(false);
        }
    }

}
