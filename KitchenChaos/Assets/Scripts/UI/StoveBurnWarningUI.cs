using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoveBurnWarningUI : MonoBehaviour
{

    [SerializeField] private StoveCounter stoveCounter;

    private void Update()
    {
        if(stoveCounter.GetState() == StoveCounter.State.Fried && stoveCounter.GetBurningProgressNormalized() >= 0.5f)
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
        transform.GetComponentInChildren<Image>().enabled = true;
    }

    private void Hide()
    {
        transform.GetComponentInChildren<Image>().enabled = false;
    }

}
