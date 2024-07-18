using System;
using System.Collections;
using UnityEngine;

namespace SoundRelated
{
    public enum SFXClip
    {
        //make sure to only add linearly and dont add anything in between to mess up the thing
        //dialog realted
        NEXT_LINE,
        START_DIALOG,
        END_DIALOG,
    }

    [Serializable]
    //this is the custom data structure to contain the music clip.
    public struct MusicClip
    {
        public SFXClip sfx;
        public AudioClip clip;
        [HideInInspector] public AudioSource source;
        [Range(0, 1)]
        public float volume;
        [Range(-3, 3)]
        public float pitch;
    }

    public enum AmbientClip
    {
        NONE,
        //put more here
    }

    [Serializable]
    public struct AmbientMusicClip
    {
        public AmbientClip ambientSFX;
        public AudioClip clip;
        [HideInInspector] public AudioSource source;
        [Range(0, 1)]
        public float volume;
        [Range(-3, 3)]
        public float pitch;
    }
}