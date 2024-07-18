namespace SoundRelated
{
    using Assets.Scripts.pattern;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class SoundManager : SingletonDontDestroy<SoundManager>
    {
        [SerializeField] private List<MusicClip> musicClips;
        [SerializeField] private List<AmbientMusicClip> ambientClips;

        [SerializeField] private AmbientClip currentAmbientClip;
        private AudioSource ambientAudioSource;

        private void Start()
        {
            InitMusicClip();
            InitAmbientClip();
            ambientAudioSource.loop = true;
        }

        private void InitMusicClip()
        {
            //init
            for (int i = 0; i < musicClips.Count; i++)
            {
                // set up the music clip for each music clip registered in editor
                var musicClip = musicClips[i];

                var component = gameObject.AddComponent<AudioSource>(); //create individual audio source for each SFX

                component.clip = musicClip.clip;
                component.volume = musicClip.volume;
                component.pitch = musicClip.pitch;
                //set up the audio source vol, pitch and clip so that it is ready to play it.

                musicClip.source = component; //put it back into the music clip so that it can be called
                musicClips[i] = musicClip;
            }
        }

        private void InitAmbientClip()
        {
            ambientAudioSource = gameObject.AddComponent<AudioSource>(); //create individual audio source for each SFX
        }

        public void PlayAmbientClip(AmbientClip clip)
        {
            foreach (var ambientClip in ambientClips)
            {
                //find the sfx clip that is related to the clip that is require to play
                if (ambientClip.ambientSFX == clip)
                {
                    currentAmbientClip = clip;
                    if (ambientAudioSource.clip == null) //if there is nothing in the clip
                    {
                        StartCoroutine(WindUpMusic(ambientClip, 4f));
                    }
                    else
                    {
                        StartCoroutine(WindDownMusic(ambientClip, 2f));
                    }
                    return;
                }
            }
            Debug.LogError("no clips");
        }

        private IEnumerator WindUpMusic(AmbientMusicClip clip, float time)
        {
            currentAmbientClip = clip.ambientSFX;
            ambientAudioSource.clip = clip.clip;
            ambientAudioSource.Play();
            ambientAudioSource.volume = 0;
            ambientAudioSource.pitch = clip.pitch;
            float elpseTime = 0;

            while (elpseTime < time)
            {
                float percentage = elpseTime / time;
                ambientAudioSource.volume = Mathf.Lerp(0, clip.volume, percentage);
                elpseTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            ambientAudioSource.volume = clip.volume;
        }

        private IEnumerator WindDownMusic(AmbientMusicClip Nexclip, float time)
        {
            float maxVolumn = ambientAudioSource.volume;
            float elpseTime = 0;
            while (elpseTime < time)
            {
                float percentage = elpseTime / time;
                ambientAudioSource.volume = Mathf.Lerp(maxVolumn, 0, percentage);
                elpseTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            ambientAudioSource.volume = 0;
            StartCoroutine(WindUpMusic(Nexclip, time));
        }


        //this is where the game can call to play the audio source
        public void PlayAudio(SFXClip clip)
        {
            foreach (var musicClip in musicClips)
            {
                //find the sfx clip that is related to the clip that is require to play
                if (musicClip.sfx == clip)
                {
                    //and play the clip
                    musicClip.source.Play();
                    return;
                }
            }
            //show an error if there is no clip to play
            Debug.LogError("no clips");
        }


    }
}