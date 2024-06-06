
using UnityEngine;

public class ScaleFromAudioClip : MonoBehaviour
{
    public AudioLoudnessDetection detector;
    public AudioSource source;
    public Vector3 minScale;
    public Vector3 maxScale;

    private void Update()
    {
        float loudness = detector.GetLoudnessFromMicroPhone();
        transform.localScale = Vector3.Lerp(minScale, maxScale, loudness);
    }
}
