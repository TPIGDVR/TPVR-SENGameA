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

        //kiosk related
        KIOSK_AUTHETICATED,
        SCAN_SUCCESS,
        TEXT_TYPING,
        IMAGE_KIOSK_OPEN,
        HOLOGRAM_CLOSE,
        DATA_DOWNLOADING,

        //heart beat
        HEART_BEAT,

        //OBJECTIVE PANEL
        FUTURISTIC_PANEL_OPEN,
    }

    [Serializable]
    //this is the custom data structure to contain the music clip.
    public struct MusicClip
    {
        public SFXClip sfx;
        public AudioClip clip;
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


}