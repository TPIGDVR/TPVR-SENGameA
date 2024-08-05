using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Jobs;

namespace Assets.Scripts.Player.Anxiety_Scripts
{
    public class PlayerAnxietyHandler : MonoBehaviour
    {

        public float _anxietyLevel = 0;

        [Header("Anxiety Related")]
        [SerializeField]
        float _maxAnxietyLevel;
        [SerializeField]
        float _anxietyIncreaseSpeed;
        [SerializeField]
        float _minAnxietyIncreaseScale = 0;
        [SerializeField]
        float _maxAnxietyIncreaseScale = 3;
        [SerializeField]
        float _anxietyIncreaseScale = 0;

        //Death timer related
        [SerializeField]
        float _maxDeathTime;
        float _currDeathTimer = 0;
        public bool isDead;

        //noise related

        NoiseSource[] _noiseSources;

        //CalculateGlare related
        [Header("Glare related")]
        [SerializeField]
        [Tooltip("Any glare below this threshold will be ignored")]
        [Range(0, 1)]
        float glareThrehold = 0.1f;
        float _maxGlareValue = 1;
        float glareValue = 0;
        Texture2D lumTex2D;
        [SerializeField, Range(0, 1)] float lT = 0.2f;


        EventManager<PlayerEvents> em_p = EventSystem.player;
        EventManager<GameEvents> em_g = EventSystem.game;
        EventManager<TutorialEvents> em_t = EventSystem.tutorial;

        [Header("reduction anxiety")]
        [SerializeField] float maxTimeReduction;
        [SerializeField] float anxietyReduction;
        float reduceElapseTime = 0f;

        public bool CanRun;
        float curAnxiety => _anxietyLevel / _maxAnxietyLevel;

        #region properties
        public float AnxietyIncreaseSpeed { get => _anxietyIncreaseSpeed; set => _anxietyIncreaseSpeed = value; }
        public float MinAnxietyIncreaseScale { get => _minAnxietyIncreaseScale; set => _minAnxietyIncreaseScale = value; }
        public float MaxAnxietyIncreaseScale { get => _maxAnxietyIncreaseScale; set => _maxAnxietyIncreaseScale = value; }
        public float AnxietyIncreaseScale { get => _anxietyIncreaseScale; set => _anxietyIncreaseScale = value; }
        #endregion

        public void InitializePlayerAnxiety()
        {
            em_p.AddListener<float>(PlayerEvents.ANXIETY_BREATHE, Breath);
            em_p.AddListener<float>(PlayerEvents.HEART_BEAT, () => curAnxiety);
            _noiseSources = FindObjectsOfType<NoiseSource>();
        }

        public void CalculateAnxiety()
        {
            //Debug.Log("calculating anxiety...");

            DetermineAnxietyScale();
            if (!CanRun || isDead)
            {
                _anxietyIncreaseScale = 0f;
                //em_t.TriggerEvent(TutorialEvents.TUTORIAL_DEATH);
            }

            DetermineAnxietyLevel();
            DetermineDeathTimer();
        }

        #region ANXIETY CALC
        float CalculateAnxietyScaleBasedOffNoiseLevel()
        {
            float totalNoiseLevel = 0;
            Transform camTrans = Camera.main.transform;
            foreach (var source in _noiseSources)
            {
                float dist = Vector3.Distance(camTrans.position, source.transform.position);

                if (source.CheckIfBlockedOrOutOfRange())
                    continue;

                float noiseLevel = Mathf.Lerp(source.NoiseValue, 0, dist / source.NoiseRangeScaled);
                totalNoiseLevel += noiseLevel;
            }



            return Mathf.Lerp(_minAnxietyIncreaseScale
                , _maxAnxietyIncreaseScale
                , totalNoiseLevel);
        }

        float CalculateAnxietyScaleBasedOffGlareLevel()
        {
            CalculateGlare();
            if (glareValue <= glareThrehold)
            {
                //do change this if u adding the anxiety for noise;
                return 0f;
            }
            else
            {
                return Mathf.Lerp(_minAnxietyIncreaseScale
                    , _maxAnxietyIncreaseScale
                    , glareValue / _maxGlareValue);
            }
        }

        void IncrementAnxietyLevel()
        {
            _anxietyLevel += (Time.deltaTime * _anxietyIncreaseSpeed) * _anxietyIncreaseScale;
        }

        void ReduceAnxietyLevel()
        {
            _anxietyLevel -= Time.deltaTime * anxietyReduction;
        }

        private void DetermineAnxietyLevel()
        {
            if (_anxietyIncreaseScale < 0.01f)
            {
                reduceElapseTime += Time.deltaTime;
                if (reduceElapseTime > maxTimeReduction)
                {
                    ReduceAnxietyLevel();
                }
            }
            else
            {
                reduceElapseTime = 0;
                IncrementAnxietyLevel();
            }
            _anxietyLevel = Mathf.Clamp(_anxietyLevel, 0, _maxAnxietyLevel);
        }

        private void DetermineAnxietyScale()
        {
            _anxietyIncreaseScale = 0f;
            _anxietyIncreaseScale += CalculateAnxietyScaleBasedOffGlareLevel();
            _anxietyIncreaseScale += CalculateAnxietyScaleBasedOffNoiseLevel();
        }

        private void DetermineDeathTimer()
        {
            if (_anxietyLevel >= _maxAnxietyLevel)
            {
                _currDeathTimer += Time.deltaTime;
            }
            else
            {
                _currDeathTimer -= Time.deltaTime;
            }
            _currDeathTimer = Mathf.Clamp(_currDeathTimer, 0, _maxDeathTime);



            if (_currDeathTimer >= _maxDeathTime && !isDead)
            {
                isDead = true;
                if (GameData.IsInTutorial)
                {
                    em_t.TriggerEvent(TutorialEvents.TUTORIAL_DEATH);
                }
                else
                {
                    em_g.TriggerEvent(GameEvents.LOSE);
                }
            }
        }
        #endregion

        void Breath(float decrease)
        {
            _anxietyLevel *= decrease;
        }

        void CalculateGlare()
        {
            try
            {
                RenderTexture rt = em_p.TriggerEvent<RTHandle>(PlayerEvents.REQUEST_LUMTEXTURE).rt;
                if (lumTex2D == null || lumTex2D.width != rt.width || lumTex2D.height != rt.height)
                    lumTex2D = new(rt.width, rt.height);

                RenderTexture.active = rt;
                lumTex2D.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                lumTex2D.Apply();
                RenderTexture.active = null;

                Color[] lumArray = lumTex2D.GetPixels();
                float totalBrightness = 0;
                for (int i = 0; i < lumArray.Length; i++)
                {
                    float brightness = lumArray[i].grayscale;
                    totalBrightness += brightness;
                }
                totalBrightness /= lumArray.Length;
                if (totalBrightness < lT)
                    totalBrightness = 0;

                rt.Release();
                glareValue = totalBrightness;

                //Debug.Log(totalBrightness);
                //em.TriggerEvent<float>(PlayerEvents.GLARE_UPDATE, totalBrightness);
            }
            catch
            {
                Debug.LogWarning("Using defualt value");
                glareValue = 0;
            }

            //glareValue = gv;
        }
    }
}

