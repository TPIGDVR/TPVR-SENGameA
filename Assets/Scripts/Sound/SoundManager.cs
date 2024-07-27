namespace SoundRelated
{
    using Assets.Scripts.pattern;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class SoundManager : SingletonDontDestroy<SoundManager>
    {
        [Header("Avaliable SFX")]
        [SerializeField] private List<MusicClip> musicClips;

        [Header("3D audio source")]
        [SerializeField] int initPoolSize3D;
        [SerializeField] GameObject prefabFor3DAudioSource;
        [SerializeField] Transform containerFor3DAudioSource;
        private PoolingPattern<AudioSource> pooled3DAudioSource;

        [Header("Global audio source")]
        [SerializeField] int initPoolSizeGlobal;
        [SerializeField] GameObject prefabForGlobalAudioSource;
        [SerializeField] Transform containerForGlobalAudioSource;
        private PoolingPattern<AudioSource> pooledGlobalAudioSource;
        //[SerializeField] private AmbientClip currentAmbientClip;
        //private AudioSource ambientAudioSource;

        private Dictionary<SFXClip, AudioSource> loopedAudioSources;


        private void Start()
        {
            //InitAmbientClip();
            //ambientAudioSource.loop = true;
            pooled3DAudioSource = CreatePoolObject(initPoolSize3D, prefabFor3DAudioSource, containerFor3DAudioSource);
            pooledGlobalAudioSource = CreatePoolObject(initPoolSizeGlobal, prefabForGlobalAudioSource, containerForGlobalAudioSource);
            loopedAudioSources = new Dictionary<SFXClip, AudioSource>();
        }

        private PoolingPattern<AudioSource> CreatePoolObject(int sizeNumber, 
            GameObject prefab, 
            Transform parent)
        {
            var poolObjects = new PoolingPattern<AudioSource>(prefab);
            poolObjects.InitWithParent(sizeNumber, parent, true);
            return poolObjects;
        }

        #region one shot
        public void PlayAudioOneShot(SFXClip clip)
        {
            foreach (var musicClip in musicClips)
            {
                //find the sfx clip that is related to the clip that is require to play
                if (musicClip.sfx == clip)
                {
                    AudioSource audioSource = pooledGlobalAudioSource.Get();
                    //set up all the audio source setting
                    audioSource.volume = musicClip.volume;
                    audioSource.clip = musicClip.clip;
                    audioSource.pitch = musicClip.pitch;
                    audioSource.loop = false;

                    audioSource.Play();
                    StartCoroutine(WaitAudioSourceToPlayFinish(audioSource, pooledGlobalAudioSource));
                    return;
                }
            }
            //show an error if there is no clip to play
            Debug.LogError("no clips");
        }

        public void PlayAudioOneShot(SFXClip clip, Vector3 globalPosition)
        {
            foreach (var musicClip in musicClips)
            {
                //find the sfx clip that is related to the clip that is require to play
                if (musicClip.sfx == clip)
                {
                    AudioSource audioSource = pooled3DAudioSource.Get();
                    //set up all the audio source setting
                    audioSource.volume = musicClip.volume;
                    audioSource.clip = musicClip.clip;
                    audioSource.pitch = musicClip.pitch;
                    audioSource.transform.position = globalPosition;
                    audioSource.loop = false;

                    audioSource.Play();
                    StartCoroutine(WaitAudioSourceToPlayFinish(audioSource, pooledGlobalAudioSource));
                    return;
                }
            }
            //show an error if there is no clip to play
            Debug.LogError("no clips");
        }

        private IEnumerator WaitAudioSourceToPlayFinish(
            AudioSource audioSource, 
            PoolingPattern<AudioSource> poolObjects)
        {
            while (audioSource.isPlaying)
            {
                yield return null;
            }
            poolObjects.Retrieve(audioSource);
        }
        #endregion

        //this will need to have another method to stop this
        public void PlayAudioContinuous(SFXClip clip)
        {
            foreach (var musicClip in musicClips)
            {
                //find the sfx clip that is related to the clip that is require to play
                if (musicClip.sfx == clip)
                {
                    AudioSource audioSource = pooledGlobalAudioSource.Get();
                    audioSource.volume = musicClip.volume;
                    audioSource.clip = musicClip.clip;
                    audioSource.pitch = musicClip.pitch;
                    audioSource.loop = true;

                    audioSource.Play();

                    loopedAudioSources.Add(clip, audioSource);

                    return;
                }
            }
            //show an error if there is no clip to play
            Debug.LogError("no clips");
        }
        public void PlayAudioContinuous(SFXClip clip, Vector3 globalPosition)
        {
            foreach (var musicClip in musicClips)
            {
                //find the sfx clip that is related to the clip that is require to play
                if (musicClip.sfx == clip)
                {
                    AudioSource audioSource = pooledGlobalAudioSource.Get();
                    audioSource.transform.position = globalPosition;
                    audioSource.volume = musicClip.volume;
                    audioSource.clip = musicClip.clip;
                    audioSource.pitch = musicClip.pitch;
                    audioSource.loop = true;

                    audioSource.Play();

                    loopedAudioSources.Add(clip, audioSource);

                    return;
                }
            }
            //show an error if there is no clip to play
            Debug.LogError("no clips");
        }

        public void StopPlayingContinuousAudio(SFXClip clip)
        {
            if(loopedAudioSources.TryGetValue(clip, out var audioSource))
            {
                audioSource.Stop();
                if(audioSource.transform.parent == containerForGlobalAudioSource)
                {
                    pooledGlobalAudioSource.Retrieve(audioSource);
                }
                else
                {
                    pooled3DAudioSource.Retrieve(audioSource);
                }

                loopedAudioSources.Remove(clip);
            }
            Debug.LogError("no clips to stop playing");

        }


        #region Ambient related code
        //private void InitAmbientClip()
        //{
        //    ambientAudioSource = gameObject.AddComponent<AudioSource>(); //create individual audio source for each SFX
        //}
        //public void PlayAmbientClip(AmbientClip clip)
        //{
        //    foreach (var ambientClip in ambientClips)
        //    {
        //        //find the sfx clip that is related to the clip that is require to play
        //        if (ambientClip.ambientSFX == clip)
        //        {
        //            currentAmbientClip = clip;
        //            if (ambientAudioSource.clip == null) //if there is nothing in the clip
        //            {
        //                StartCoroutine(WindUpMusic(ambientClip, 4f));
        //            }
        //            else
        //            {
        //                StartCoroutine(WindDownMusic(ambientClip, 2f));
        //            }
        //            return;
        //        }
        //    }
        //    Debug.LogError("no clips");
        //}

        //private IEnumerator WindUpMusic(AmbientMusicClip clip, float time)
        //{
        //    currentAmbientClip = clip.ambientSFX;
        //    ambientAudioSource.clip = clip.clip;
        //    ambientAudioSource.Play();
        //    ambientAudioSource.volume = 0;
        //    ambientAudioSource.pitch = clip.pitch;
        //    float elpseTime = 0;

        //    while (elpseTime < time)
        //    {
        //        float percentage = elpseTime / time;
        //        ambientAudioSource.volume = Mathf.Lerp(0, clip.volume, percentage);
        //        elpseTime += Time.deltaTime;
        //        yield return new WaitForEndOfFrame();
        //    }

        //    ambientAudioSource.volume = clip.volume;
        //}

        //private IEnumerator WindDownMusic(AmbientMusicClip Nexclip, float time)
        //{
        //    float maxVolumn = ambientAudioSource.volume;
        //    float elpseTime = 0;
        //    while (elpseTime < time)
        //    {
        //        float percentage = elpseTime / time;
        //        ambientAudioSource.volume = Mathf.Lerp(maxVolumn, 0, percentage);
        //        elpseTime += Time.deltaTime;
        //        yield return new WaitForEndOfFrame();
        //    }

        //    ambientAudioSource.volume = 0;
        //    StartCoroutine(WindUpMusic(Nexclip, time));
        //}


        //this is where the game can call to play the audio source
        #endregion

    }
}