using UnityEngine;
namespace BreathDetection
{
    [RequireComponent(typeof(LineRenderer), typeof(AudioSource))]
    public class AudioVisualizer : MonoBehaviour
    {
        [SerializeField] private bool isGetData = false;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private AudioSource _source = default;
        public float[] _data = new float[1024];
        const float pitchIncrementor = 24000f / 1024f;

        [SerializeField] Color color_threshHold = Color.yellow;
        [Range(0, 24000)]
        [SerializeField] float pitchThreshold = 1000;
        [SerializeField] float amp = 0.01f;

        private void OnEnable()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void OnDisable()
        {
            _lineRenderer = null;
            _source = null;
        }

        public void Update()
        {
            if (_lineRenderer == null) return;
            if (_source.clip == null) return;
            if (isGetData)
            {
                _source.GetOutputData(_data, 0);
            }
            else
            {
                _source.GetSpectrumData(_data, 0, FFTWindow.BlackmanHarris);
            }
            _lineRenderer.positionCount = _data.Length;

            var positions = new Vector3[_data.Length];
            const float xStretch = 8.0f;
            var yOffset = transform.position.y;
            for (var i = 0; i < _data.Length; i++)
            {
                positions[i] = new Vector3(
                    xStretch * (2.0f * i / (_lineRenderer.positionCount - 1.0f) - 1.0f),
                    _data[i] * 500.0f + yOffset,
                    0);
            }

            _lineRenderer.SetPositions(positions);

            //create the debug.drawline
            float index = pitchThreshold / pitchIncrementor;

            Debug.DrawLine(new Vector3(
                    xStretch * (2.0f * index / (_lineRenderer.positionCount - 1.0f) - 1.0f),
                    yOffset - 1f,
                    0),
                    new Vector3(
                    xStretch * (2.0f * index / (_lineRenderer.positionCount - 1.0f) - 1.0f),
                    yOffset + 1f,
                    0),
                    color_threshHold
                    );

            Debug.DrawLine(new Vector3(
                    xStretch * (2.0f * 0 / (_lineRenderer.positionCount - 1.0f) - 1.0f), 
                    amp * 500 + yOffset,
                    0
                ),
                new Vector3(
                    xStretch * 2.0f,
                    amp * 500 + yOffset,
                    0
                    ),
                color_threshHold
                );
        }
    }
}