using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{

    [SerializeField] private StoveCounter stoveCounter;

    private AudioSource audioSource;
    private float warningSoundTimer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void Update()
    {
        if(stoveCounter.GetState() == StoveCounter.State.Fried && stoveCounter.GetBurningProgressNormalized() >= 0.5f)
        {
            warningSoundTimer -= Time.deltaTime;
            if(warningSoundTimer <= 0)
            {
                float warningSoundTimerMax = 0.2f;
                warningSoundTimer = warningSoundTimerMax;
                SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);
            }
        }
    }

    private void StoveCounter_OnStateChanged(object sender, System.EventArgs e)
    {
        bool playSound = stoveCounter.GetState() == StoveCounter.State.Frying || stoveCounter.GetState() == StoveCounter.State.Fried;
        if (playSound)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Pause();
        }
    }
}
