using System.Collections;
using TMPro;
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
        ExhaleData _exhaleDataTemplate;

        [SerializeField]
        InhaleData _inhaleDataTemplate;

        [SerializeField]
        InhaleData _exhaleDataSpectrumTemplate;

        [Header("CalculatedValues")]
        [SerializeField]
        ExhaleData calculatedExhaleData;
        [SerializeField]
        InhaleData calculatedInhaleData;
        [SerializeField]
        InhaleData calculateExhaleSpectrumData;

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
        int amountTested = 0;

        float elapseTime = 0;

        ITestable<InhaleData> inhaleTester;
        ITestable<InhaleData> exhaleSpectrumTester;
        ITestable<ExhaleData> exhaleLoudnessTester;

        [Header("debugging")]
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] bool useVolPitchExhale;
        public BreathingOutPut breathingOutPut { get; private set; }

        //For breathing panel to display player
        public BreathingTestingState breathingTestingState { get; private set; }
        public float remainTimingForTesting { get
            {
                switch (breathingTestingState)
                {
                    case BreathingTestingState.PAUSE:
                        return amountOfTimeToPause - elapseTime;
                    case BreathingTestingState.INHALE:
                        return amountOfTimeToSample - elapseTime;
                    case BreathingTestingState.EXHALE:
                        return amountOfTimeToSample - elapseTime;
                        default : return 0;
                }
            }
        }

        #endregion 

        private void Start()
        {
            //do the initializing here

            if (usePresetData)
            {
                //inhaleTester = new SpectrumMinMaxTester(micProvider, _inhaleDataTemplate);
                //exhaleSpectrumTester = new SpectrumMinMaxTester(micProvider, _exhaleDataSpectrumTemplate);
                //exhaleLoudnessTester = new ExhaleTester(_exhaleDataTemplate, micProvider);
                //isTesting = true;
                inhaleDetection = new InhalingDetector(micProvider, safeFile.inhaleCalculatedData);
                exhaleDetection = new ExhalingDetector(micProvider, safeFile.exhaleLoudnessData);
                exhaleSpectrumDetection = new InhalingDetector(micProvider, safeFile.exhaleCalculatedData);
                isTesting = false;
                //StartCoroutine(RunBreathingTest());
            }
            

            //Microphone.GetDeviceCaps(Microphone.devices[0],
            //    out int minFrequency,
            //    out int maxFrequency);
            //print($"{Microphone.devices[0]} min frequency {minFrequency} max frequency {maxFrequency}");
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
                print($"complete test {numTested}");
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
            while(elapseTime < amountOfTimeToPause)
            {
                elapseTime += Time.deltaTime;
                yield return null;
            }
        }

        IEnumerator CalculatingInhale(int numberTested)
        {
            breathingTestingState = BreathingTestingState.INHALE;
            elapseTime = 0;
            if(numberTested == amountToTest / 2)
            {
                //if reach half for amount of tested
                inhaleTester = new SpectrumTester(micProvider, inhaleTester.Calculate());
            }
            text.text = "Please inhale";

            while(elapseTime < amountOfTimeToSample)
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

        /// <summary>
        /// Legacy dont use. used for collecting player breath.
        /// </summary>
        //void CalculateDataTester()
        //{
        //    print($"Elapse time {elapseTime}. has finish inhale {hasTestedInhale}");
        //    if(elapseTime < amountOfTimeToSample)
        //    {
        //        if (!hasTestedInhale)
        //        {//run the inhale here
        //            inhaleTester.Run();
        //            stateText.stateText = "Please inhale";
        //        }
        //        else
        //        {//run the exhale here
        //            exhaleLoudnessTester.Run();
        //            exhaleSpectrumTester.Run();
        //            stateText.stateText = "Please exhale";
        //        }
        //        elapseTime += Time.deltaTime;
        //    }
        //    else
        //    {//when the thing is done
        //        if (hasTestedInhale)
        //        {
        //            amountTested++;

        //            if(amountTested == amountToTest / 2)
        //            {
        //                //after finding the min and max points of the inhale and exhale, it would 
        //                //calculate the spectrum to find the common point between the two points.
        //                inhaleTester = new SpectrumTester(micProvider, inhaleTester.Calculate());
        //                exhaleSpectrumTester = new SpectrumTester(micProvider, exhaleSpectrumTester.Calculate());
        //            }
        //        }

        //        if(amountTested < amountToTest)
        //        {
        //            elapseTime = 0f;
        //            hasTestedInhale = !hasTestedInhale;
        //        }
        //        else
        //        {
        //            //has reach the requirement
        //            FinishCalculation();
        //            isTesting = false;
        //            stateText.stateText = "Testing complete!";
        //        }
        //    }
            

        //}
        void FinishCalculation()
        {
            calculatedInhaleData = inhaleTester.Calculate();
            calculateExhaleSpectrumData = exhaleSpectrumTester.Calculate();
            calculatedExhaleData = exhaleLoudnessTester.Calculate();

            inhaleDetection = new InhalingDetector(micProvider, calculatedInhaleData);
            exhaleSpectrumDetection = new InhalingDetector(micProvider, calculateExhaleSpectrumData);
            exhaleDetection = new ExhalingDetector(micProvider, calculatedExhaleData);
        }

        private void Update()
        {
            if(!isTesting && CanRun)
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
                print($"_ Is Inhaling {isInhaling}, Is Exhaling {isExhaling}");
            }
        }

        #region debugging
        [ContextMenu("testing")]
        public void StartTesting()
        {
            //isTesting = true;
            //hasTestedInhale = false;
            //elapseTime = 0;
            //amountTested = 0;
            //inhaleTester = new SpectrumMinMaxTester(micProvider, _inhaleDataTemplate);
            //exhaleLoudnessTester = new ExhaleTester( _exhaleDataTemplate , micProvider);
            //exhaleSpectrumTester = new SpectrumMinMaxTester(micProvider, _exhaleDataSpectrumTemplate);
            breathingTestingState = BreathingTestingState.PAUSE;
            elapseTime = 0;

            StopAllCoroutines();
            StartCoroutine(RunBreathingTest());
        }

        [ContextMenu("Update values")]
        public void UpdateValues()
        {
            //do changes this later since this is kinda effy way to do it.
            inhaleDetection = new InhalingDetector(micProvider , calculatedInhaleData);
            exhaleDetection = new ExhalingDetector(micProvider, calculatedExhaleData);
            exhaleSpectrumDetection = new InhalingDetector(micProvider, calculateExhaleSpectrumData);
        }

        [ContextMenu("safe Data")]
        public void SafeValue()
        {
            if(safeFile != null)
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
