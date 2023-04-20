using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClipRefsSO audioClipRefs;

    private float volume = 1f;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("There's more than one SoundManager object active in the scene!");
            Destroy(gameObject);
        }
        Instance = this;

        volume = PlayerPrefs.GetFloat("SoundEffectsVolume", 1f);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        CuttingCounter.OnCut += CuttingCounter_OnCut;
        //Player.Instance.OnPickup += Player_OnPickup;
        BaseCounter.OnObjectDropped += BaseCounter_OnObjectDropped;
        TrashCounter.OnTrashed += TrashCounter_OnTrashed;
    }

    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e)
    {
        Vector3 position = (sender as MonoBehaviour).transform.position;
        PlaySound(audioClipRefs.deliveryFail, position);
    }

    private void DeliveryManager_OnRecipeCompleted(object sender, System.EventArgs e)
    {
        Vector3 position = (sender as MonoBehaviour).transform.position;
        PlaySound(audioClipRefs.deliverySuccess, position);
    }

    private void CuttingCounter_OnCut(object sender, System.EventArgs e)
    {
        Vector3 position = (sender as MonoBehaviour).transform.position;
        PlaySound(audioClipRefs.chop, position);
    }

    private void Player_OnPickup(object sender, System.EventArgs e)
    {
        //PlaySound(audioClipRefs.objectPickup, Player.Instance.transform.position);
    }

    private void BaseCounter_OnObjectDropped(object sender, System.EventArgs e)
    {
        Vector3 position = (sender as MonoBehaviour).transform.position;
        PlaySound(audioClipRefs.objectDrop, position);
    }

    private void TrashCounter_OnTrashed(object sender, System.EventArgs e)
    {
        Vector3 position = (sender as MonoBehaviour).transform.position;
        PlaySound(audioClipRefs.trash, position);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, this.volume * volume);
    }
    
    private void PlaySound(AudioClip[] audioClips, Vector3 position, float volume = 1f)
    {
        PlaySound(audioClips[Random.Range(0, audioClips.Length)], position, this.volume * volume);
    }

    public void PlayFootstepSound(Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipRefs.footsteps, position, this.volume * volume);
    }

    public void PlayCountdownSound()
    {
        PlaySound(audioClipRefs.warning[0], Camera.main.transform.position);
    }

    public void PlayWarningSound(Vector3 position)
    {
        PlaySound(audioClipRefs.warning[1], position);
    }

    public void ChangeVolume()
    {
        volume += 0.1f;
        if(volume > 1f)
        {
            volume = 0f;
        }

        PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return volume;
    }

}
