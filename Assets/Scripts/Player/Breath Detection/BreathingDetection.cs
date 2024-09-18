using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace BreathDetection
{
    public class BreathingDetection : MonoBehaviour
    {
        //if the breath detection can even run.
        [SerializeField] bool canRun = true;
        public bool CanRun { get => canRun; set => canRun = value; }

        #region properties
        [SerializeField] MicProvider micProvider;

        [Header("Templates")]
        [SerializeField]
        LoudnessData _exhaleDataTemplate;

        [SerializeField]
        SpectrumData _inhaleDataTemplate;

        [SerializeField]
        SpectrumData _exhaleDataSpectrumTemplate;

        [Header("CalculatedValues")]
        [SerializeField]
        LoudnessData calculatedExhaleData;
        [SerializeField]
        SpectrumData calculatedInhaleData;
        [SerializeField]
        SpectrumData calculateExhaleSpectrumData;

        /// <summary>
        /// For saving file for inhaling and exhaling
        /// </summary>
        [Header("safe file")]
        [SerializeField] BreathSafeFile safeFile;

        IBreathResult inhaleDetection;
        IBreathResult exhaleDetection;
        IBreathResult exhaleSpectrumDetection;

        bool _IsInhaling => inhaleDetection.Result();
        bool _IsExhaling => useVolPitchExhale ? exhaleDetection.Result() : exhaleSpectrumDetection.Result();

        [SerializeField] bool usePresetData;

        [Header("collection Data")]
        bool isTesting = false;
        [SerializeField] int amountToTest = 2;
        [SerializeField] float amountOfTimeToSample = 2f;
        [SerializeField] float amountOfTimeToPause = 0.5f;

        float elapseTime = 0;

        ITestable<SpectrumData> inhaleTester;
        ITestable<SpectrumData> exhaleSpectrumTester;
        ITestable<LoudnessData> exhaleLoudnessTester;

        [Header("debugging")]
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] bool useVolPitchExhale;
        public BreathingOutPut breathingOutPut { get; private set; }

        //For breathing panel to display player
        public BreathingTestingState breathingTestingState { get; private set; }
        public float remainTimingForTesting
        {
            get
            {
                switch (breathingTestingState)
                {
                    case BreathingTestingState.PAUSE:
                        return amountOfTimeToPause - elapseTime;
                    case BreathingTestingState.INHALE:
                        return amountOfTimeToSample - elapseTime;
                    case BreathingTestingState.EXHALE:
                        return amountOfTimeToSample - elapseTime;
                    default: return 0;
                }
            }
        }

        public float NormaliseVolumeForUI
        {
            get
            {
                //20
                float clampValue = Mathf.Clamp(micProvider.volume, -60, 20);
                clampValue -= 20;
                return 1 - math.abs(clampValue / -80);
            }
        }

        #endregion 

        private void Start()
        {
            //do the initializing here

            if (usePresetData)
            {
                inhaleDetection = new SpectrumDetector(micProvider, safeFile.inhaleCalculatedData);
                exhaleDetection = new LoudnessDetector(micProvider, safeFile.exhaleLoudnessData);
                exhaleSpectrumDetection = new SpectrumDetector(micProvider, safeFile.exhaleCalculatedData);
                isTesting = false;
            }
        }

        #region testing
        IEnumerator RunBreathingTest()
        {
            isTesting = true;
            int numTested = 0;

            inhaleTester = new SpectrumMinMaxTester(micProvider, _inhaleDataTemplate);
            exhaleSpectrumTester = new SpectrumMinMaxTester(micProvider, _exhaleDataSpectrumTemplate);
            exhaleLoudnessTester = new ExhaleTester(_exhaleDataTemplate, micProvider);

            while (numTested < amountToTest)
            {
                yield return PauseForBreathing();
                yield return CalculatingInhale(numTested);
                yield return PauseForBreathing();
                yield return CalculatingExhale(numTested);
                numTested++;
                //print($"complete test {numTested}");
            }
            //indicate that the breathing is done.
            breathingTestingState = BreathingTestingState.NONE;

            FinishCalculation();
            isTesting = false;
            text.text = "finish testing";
        }

        IEnumerator PauseForBreathing()
        {
            breathingTestingState = BreathingTestingState.PAUSE;
            elapseTime = 0;
            text.text = "Pause";
            while (elapseTime < amountOfTimeToPause)
            {
                elapseTime += Time.deltaTime;
                yield return null;
            }
        }

        IEnumerator CalculatingInhale(int numberTested)
        {
            breathingTestingState = BreathingTestingState.INHALE;
            elapseTime = 0;
            if (numberTested == amountToTest / 2)
            {
                //if reach half for amount of tested
                inhaleTester = new SpectrumTester(micProvider, inhaleTester.Calculate());
            }
            text.text = "Please inhale";

            while (elapseTime < amountOfTimeToSample)
            {
                inhaleTester.Run();
                elapseTime += Time.deltaTime;
                yield return null;
            }
        }

        IEnumerator CalculatingExhale(int numberTested)
        {
            breathingTestingState = BreathingTestingState.EXHALE;
            elapseTime = 0;
            if (numberTested == amountToTest / 2)
            {
                //if reach half for amount of tested
                exhaleSpectrumTester = new SpectrumTester(micProvider, exhaleSpectrumTester.Calculate());
            }
            text.text = "Please exhale";

            while (elapseTime < amountOfTimeToSample)
            {
                exhaleSpectrumTester.Run();
                exhaleLoudnessTester.Run();
                elapseTime += Time.deltaTime;
                yield return null;
            }
        }
        #endregion

        void FinishCalculation()
        {
            calculatedInhaleData = inhaleTester.Calculate();
            calculateExhaleSpectrumData = exhaleSpectrumTester.Calculate();
            calculatedExhaleData = exhaleLoudnessTester.Calculate();

            inhaleDetection = new SpectrumDetector(micProvider, calculatedInhaleData);
            exhaleSpectrumDetection = new SpectrumDetector(micProvider, calculateExhaleSpectrumData);
            exhaleDetection = new LoudnessDetector(micProvider, calculatedExhaleData);
        }

        private void Update()
        {
            if (!isTesting && CanRun)
            {
                bool isInhaling = this._IsInhaling;
                bool isExhaling = this._IsExhaling;
                if (isExhaling)
                {
                    breathingOutPut = BreathingOutPut.EXHALE;
                }
                else if (isInhaling)
                {
                    breathingOutPut = BreathingOutPut.INHALE;
                }
                else
                {
                    breathingOutPut = BreathingOutPut.SILENCE;
                }
                //print($"_ Is Inhaling {isInhaling}, Is Exhaling {isExhaling}");
            }
        }

        #region debugging
        [ContextMenu("testing")]
        public void StartTesting()
        {
            breathingTestingState = BreathingTestingState.PAUSE;
            elapseTime = 0;

            StopAllCoroutines();
            StartCoroutine(RunBreathingTest());
        }

        [ContextMenu("Update values")]
        public void UpdateValues()
        {
            //do changes this later since this is kinda effy way to do it.
            inhaleDetection = new SpectrumDetector(micProvider, calculatedInhaleData);
            exhaleDetection = new LoudnessDetector(micProvider, calculatedExhaleData);
            exhaleSpectrumDetection = new SpectrumDetector(micProvider, calculateExhaleSpectrumData);
        }

        [ContextMenu("safe Data")]
        public void SafeValue()
        {
            if (safeFile != null)
            {
                safeFile.inhaleCalculatedData = calculatedInhaleData;
                safeFile.exhaleLoudnessData = calculatedExhaleData;
                safeFile.exhaleCalculatedData = calculateExhaleSpectrumData;
            }
        }
        #endregion
    }

    public enum BreathingOutPut
    {
        SILENCE,
        INHALE,
        EXHALE,
    }

    public enum BreathingTestingState
    {
        PAUSE,
        INHALE,
        EXHALE,
        NONE
    }
}
