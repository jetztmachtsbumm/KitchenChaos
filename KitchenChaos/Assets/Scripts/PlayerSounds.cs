using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{

    private float footstepTimer;
    private float footstepTimerMax = 0.1f;

    private void Update()
    {
        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0)
        {
            footstepTimer = footstepTimerMax;

            if (Player.Instance.IsWalking())
            {
                SoundManager.Instance.PlayFootstepSound(Player.Instance.transform.position);
            }
        }
    }

}
