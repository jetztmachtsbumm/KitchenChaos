using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterFlashingBarUI : MonoBehaviour
{

    [SerializeField] private StoveCounter stoveCounter;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Flash", false);
    }

    private void Update()
    {
        animator.SetBool("Flash", stoveCounter.GetState() == StoveCounter.State.Fried && stoveCounter.GetBurningProgressNormalized() >= 0.5f);
    }

}
