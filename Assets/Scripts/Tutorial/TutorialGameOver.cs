using System.Collections;
using System.Collections.Generic;
using SoundRelated;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialGameOver : MonoBehaviour
{

    [SerializeField]
    Image progressUI;
    [SerializeField]
    float speedMultiplier;
    [SerializeField]
    Color lowColor, medColor, hiColor;

    public Transform deathPoint;
    float t, progress;
    bool scanning;
    bool scanCompleted;
    bool authenticate = false;
    [SerializeField]
    SoundManager audioPlayer;
    AudioSource globalAudioSource = null;

    

    #region legacy
    //private void Start()
    //{
    //    progressUI.fillAmount = 0f;
    //}
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        ScanStart();
    //    }

    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        ScanStop();
    //    }
    //}

    //private void Update()
    //{

    //    if (scanning && !scanCompleted)
    //    {
    //        progress += Time.deltaTime / 100 * speedMultiplier;

    //    }


    //    else if (!scanning && !scanCompleted)
    //    {
    //        //make the progress decay slightly slower than the gain speed
    //        progress -= Time.deltaTime / 200 * speedMultiplier;
    //    }


    //    progress = Mathf.Clamp01(progress); //make sure we maintain the 0-1 values 
    //    UpdateProgressUI();

    //    if (progress >= 1 && !scanCompleted)
    //    {
    //        scanCompleted = true;   
    //        audioPlayer.PlayAudioOneShot(SoundRelated.SFXClip.SCAN_SUCCESS, transform.position);
    //        EventSystem.player.TriggerEvent(PlayerEvents.RESTART);
    //    }
    //}

    //void UpdateProgressUI()
    //{
    //    progressUI.fillAmount = progress;

    //    //slowly changes color as it progresses
    //    if (progress < 0.5)
    //    {
    //        progressUI.color = Color.Lerp(lowColor, medColor, progress * 0.5f);
    //    }
    //    else
    //    {
    //        progressUI.color = Color.Lerp(medColor, hiColor, progress * 0.5f);
    //    }
    //}


    ////called by xr simple interactor
    //public void ScanStart()
    //{
    //    scanning = true;
    //    if (!scanCompleted)
    //        //audioSource.PlayOneShot(audioClips[0]);
    //        audioPlayer.PlayAudioOneShot(SoundRelated.SFXClip.KIOSK_AUTHETICATED, transform.position);
    //}

    ////called by xr simple interactor
    //public void ScanStop()
    //{
    //    scanning = false;

    //    if (!scanCompleted)
    //    {
    //        if (globalAudioSource)
    //        {
    //            audioPlayer.RetrieveAudioSource(globalAudioSource);
    //        }
    //        //StopSFX();
    //        //audioPlayer.StopPlayingContinuousAudio(SoundRelated.SFXClip.TEXT_TYPING);
    //    }
    //}
    #endregion
}
