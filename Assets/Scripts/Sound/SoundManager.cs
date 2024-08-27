namespace SoundRelated
{
    using Assets.Scripts.pattern;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class SoundManager : SingletonDontDestroy<SoundManager> , IScriptLoadQueuer
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

        protected override void Awake()
        {
            base.Awake();
            ScriptLoadSequencer.Enqueue(this, (int) LevelLoadSequence.SYSTEM);
        }

        //if the sound manager is placed at the starting menu that this will be called 
        private void Start()
        {
            TryInitalizePool();
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
                //find the sfx transcript that is related to the transcript that is require to play
                if (musicClip.sfx == clip)
                {
                    AudioSource audioSource = pooledGlobalAudioSource.Get();
                    //set up all the audio source setting
                    SetUpAudioSource(musicClip, audioSource);
                    audioSource.loop = false;

                    audioSource.Play();
                    StartCoroutine(WaitAudioSourceToPlayFinish(audioSource, pooledGlobalAudioSource));
                    return;
                }
            }
            //show an error if there is no transcript to play
            Debug.LogError("no clips");
        }


        public void PlayAudioOneShot(SFXClip clip, Vector3 globalPosition)
        {
            foreach (var musicClip in musicClips)
            {
                //find the sfx transcript that is related to the transcript that is require to play
                if (musicClip.sfx == clip)
                {
                    AudioSource audioSource = pooled3DAudioSource.Get();
                    //set up all the audio source setting
                    SetUpAudioSource(musicClip, audioSource);
                    audioSource.transform.position = globalPosition;
                    audioSource.loop = false;

                    audioSource.Play();
                    StartCoroutine(WaitAudioSourceToPlayFinish(audioSource, pooled3DAudioSource));
                    return;
                }
            }
            //show an error if there is no transcript to play
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

        #region playing continuous music
        //this will need to have another method to stop this
        public AudioSource PlayAudioContinuous(SFXClip clip)
        {
            foreach (var musicClip in musicClips)
            {
                //find the sfx transcript that is related to the transcript that is require to play
                if (musicClip.sfx == clip)
                {
                    AudioSource audioSource = pooledGlobalAudioSource.Get();
                    SetUpAudioSource(musicClip, audioSource);
                    audioSource.loop = true;

                    audioSource.Play();
                    return audioSource;
                }
            }
            //show an error if there is no transcript to play
            Debug.LogError("no clips");
            return null;
        }
        public AudioSource PlayAudioContinuous(SFXClip clip, Vector3 globalPosition)
        {
            foreach (var musicClip in musicClips)
            {
                //find the sfx transcript that is related to the transcript that is require to play
                if (musicClip.sfx == clip)
                {
                    AudioSource audioSource = pooled3DAudioSource.Get();
                    SetUpAudioSource(musicClip,audioSource);
                    audioSource.transform.position = globalPosition;
                    audioSource.loop = true;

                    audioSource.Play();
                    return audioSource;
                }
            }
            //show an error if there is no transcript to play
            Debug.LogError("no clips");
            return null;
        }
        #endregion
        
        public AudioSource PlayMusicClip(MusicClip clip)
        {
            var source = pooledGlobalAudioSource.Get();
            SetUpAudioSource(clip, source);
            source.loop = false;

            source.Play();
            return source;  
        }

        public AudioSource PlayMusicClip(MusicClip clip, Vector3 globalPosition)
        {
            var source = pooled3DAudioSource.Get();
            SetUpAudioSource(clip, source);
            source.loop = false;
            source.transform.position = globalPosition;
            source.Play();
            return source;
        } 

        private void SetUpAudioSource(MusicClip musicClip, AudioSource audioSource)
        {
            audioSource.volume = musicClip.volume;
            audioSource.clip = musicClip.clip;
            audioSource.pitch = musicClip.pitch;
        }

        public void RetrieveAudioSource(AudioSource audioSource)
        {
            if (audioSource == null)
            {
                Debug.LogError("no audio source to retrieve from!");
                return;
            }
            audioSource.Stop();
            if (audioSource.transform.parent == containerForGlobalAudioSource)
            {
                pooledGlobalAudioSource.Retrieve(audioSource);
            }
            else if(audioSource.transform.parent == containerFor3DAudioSource)
            {
                pooled3DAudioSource.Retrieve(audioSource);
            }
        }

        public void Initialize()
        {
            TryInitalizePool();
        }

        void TryInitalizePool()
        {
            if(pooledGlobalAudioSource == null)
            {
                pooledGlobalAudioSource = CreatePoolObject(initPoolSizeGlobal, prefabForGlobalAudioSource, containerForGlobalAudioSource);
            }
            if(pooled3DAudioSource == null)
            {
                pooled3DAudioSource = CreatePoolObject(initPoolSize3D, prefabFor3DAudioSource, containerFor3DAudioSource);
            }
        }

        #region Ambient related code
        //private void InitAmbientClip()
        //{
        //    ambientAudioSource = gameObject.AddComponent<AudioSource>(); //create individual audio source for each SFX
        //}
        //public void PlayAmbientClip(AmbientClip transcript)
        //{
        //    foreach (var ambientClip in ambientClips)
        //    {
        //        //find the sfx transcript that is related to the transcript that is require to play
        //        if (ambientClip.ambientSFX == transcript)
        //        {
        //            currentAmbientClip = transcript;
        //            if (ambientAudioSource.transcript == null) //if there is nothing in the transcript
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

        //private IEnumerator WindUpMusic(AmbientMusicClip transcript, float time)
        //{
        //    currentAmbientClip = transcript.ambientSFX;
        //    ambientAudioSource.transcript = transcript.transcript;
        //    ambientAudioSource.Play();
        //    ambientAudioSource.volume = 0;
        //    ambientAudioSource.pitch = transcript.pitch;
        //    float elpseTime = 0;

        //    while (elpseTime < time)
        //    {
        //        float percentage = elpseTime / time;
        //        ambientAudioSource.volume = Mathf.Lerp(0, transcript.volume, percentage);
        //        elpseTime += Time.deltaTime;
        //        yield return new WaitForEndOfFrame();
        //    }

        //    ambientAudioSource.volume = transcript.volume;
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