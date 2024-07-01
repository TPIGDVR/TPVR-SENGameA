
using UnityEngine;

public class AudioLoudnessDetection : MonoBehaviour
{
    public int sampleWindow = 64;
    public AudioClip microPhoneClip;

    private void Start()
    {
        GetMicroPhone();
    }

    public float GetLoudnessFromMicroPhone()
    {
        return GetLoudnessFromAudioClip(Microphone.GetPosition(Microphone.devices[0]), microPhoneClip);
    }

    public float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip)
    {
        int startPosition = clipPosition - sampleWindow;
        float[] waveData = new float[sampleWindow];
        clip.GetData(waveData, startPosition);

        float totalLoudness = 0f;
        for (int i = 0; i < sampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(waveData[i]);
        }

        //get the avergae of the loudness
        return totalLoudness / sampleWindow;
    }

    public void GetMicroPhone()
    {
        string microPhoneName = Microphone.devices[0];
        microPhoneClip = Microphone.Start(microPhoneName, true, 20, AudioSettings.outputSampleRate);
    }
}
