using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Assets.Scripts.Audio_script
{
    public class Draw : MonoBehaviour
    {
        public int width = 500;
        public int height = 100;
        public Color waveformColor = Color.green;
        public Color bgColor = Color.green;
        public float sat = .5f;

        [SerializeField] Image img;
        [SerializeField] AudioClip clip;
        [SerializeField] Texture2D texture;
        [SerializeField] Sprite sprite;

        void Start()
        {
            //GetMicroPhone();
            texture = PaintWaveformSpectrum(clip, sat, width, height, waveformColor, bgColor);
            sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
            img.sprite = sprite;

            //.GetWaveFormFast(audioClip, 1, 0, audioClip.samples, r.width, r.height);
            //print("running");
            //texture = AudioUtility.GetWaveFormFast(clip, 1, 0, clip.samples,width,height);
            //sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height),
            //    new Vector2(0.5f, 0.5f));
            //img.sprite = sprite;
        }

        private void Update()
        {
            //texture = AssetPreview.GetAssetPreview(clip);
            //sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height),
            //    new Vector2(0.5f, 0.5f));
            //img.sprite = sprite;
        }

        public void GetMicroPhone()
        {
            string microPhoneName = Microphone.devices[0];
            clip = Microphone.Start(microPhoneName, true, width, AudioSettings.outputSampleRate);
        }


        public Texture2D PaintWaveformSpectrum(AudioClip audio, float saturation, int width, int height, Color col, Color bk)
        {
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float[] samples = new float[audio.samples];
            float[] waveform = new float[width];
            audio.GetData(samples, 0);
            int packSize = (audio.samples / width) + 1;
            int s = 0;
            for (int i = 0; i < audio.samples; i += packSize)
            {
                waveform[s] = Mathf.Abs(samples[i]);
                s++;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {//set everything to black.
                    tex.SetPixel(x, y, Color.black);
                }
            }

            for (int x = 0; x < waveform.Length; x++)
            {
                for (int y = 0; y <= waveform[x] * ((float)height * .75f); y++)
                {
                    tex.SetPixel(x, (height / 2) + y, col);
                    tex.SetPixel(x, (height / 2) - y, col);
                }
            }
            tex.Apply();

            return tex;
        }
    }

}