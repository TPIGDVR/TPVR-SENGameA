using TMPro;
using UnityEngine;

namespace BreathDetection
{
    public class BreathingDetection : MonoBehaviour
    {
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


        [Header("safe file")]
        [SerializeField] BreathSafeFile safeFile;

        IBreathResult inhaleDetection;
        IBreathResult exhaleDetection;
        IBreathResult exhaleSpectrumDetection;

        bool _IsInhaling => inhaleDetection.Result();
        bool _IsExhaling => useVolPitchExhale ? exhaleDetection.Result() : exhaleSpectrumDetection.Result();

        [SerializeField] bool usePresetData;

        [Header("collection Data")]
        bool isTesting;
        [SerializeField] int amountToTest = 2;
        [SerializeField] float amountOfTimeToSample = 2f;
        int amountTested = 0;
        bool hasTestedInhale = false;

        float elapseTime = 0;

        ITestable<InhaleData> inhaleTester;
        ITestable<InhaleData> exhaleSpectrumTester;
        ITestable<ExhaleData> exhaleLoudnessTester;
        [Header("debugging")]
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] bool useVolPitchExhale;
        public BreathingOutPut breathingOutPut { get; private set; }

        private void Start()
        {
            //do the initializing here

            if (!usePresetData)
            {
                inhaleTester = new SpectrumMinMaxTester(micProvider, _inhaleDataTemplate);
                exhaleSpectrumTester = new SpectrumMinMaxTester(micProvider, _exhaleDataSpectrumTemplate);
                exhaleLoudnessTester = new ExhaleTester(_exhaleDataTemplate, micProvider);
                isTesting = true;
            }
            else
            {
                inhaleDetection = new InhalingDetector(micProvider, safeFile.inhaleCalculatedData);
                exhaleDetection = new ExhalingDetector(micProvider, safeFile.exhaleLoudnessData);
                exhaleSpectrumDetection = new InhalingDetector(micProvider, safeFile.exhaleCalculatedData);
                isTesting = false;
            }

            //Microphone.GetDeviceCaps(Microphone.devices[0],
            //    out int minFrequency,
            //    out int maxFrequency);
            //print($"{Microphone.devices[0]} min frequency {minFrequency} max frequency {maxFrequency}");
        }

        void CalculateData()
        {
            print($"Elapse time {elapseTime}. has finish inhale {hasTestedInhale}");
            if(elapseTime < amountOfTimeToSample)
            {
                if (!hasTestedInhale)
                {//run the inhale here
                    inhaleTester.Run();
                    text.text = "Please inhale";
                }
                else
                {//run the exhale here
                    exhaleLoudnessTester.Run();
                    exhaleSpectrumTester.Run();
                    text.text = "Please exhale";
                }
                elapseTime += Time.deltaTime;
            }

            else
            {//when the thing is done
                if (hasTestedInhale)
                {
                    amountTested++;

                    if(amountTested == amountToTest / 2)
                    {
                        inhaleTester = new SpectrumTester(micProvider, inhaleTester.Calculate());
                        exhaleSpectrumTester = new SpectrumTester(micProvider, exhaleSpectrumTester.Calculate());
                    }
                }

                if(amountTested < amountToTest)
                {
                    elapseTime = 0f;
                    hasTestedInhale = !hasTestedInhale;
                }
                else
                {
                    //has reach the requirement
                    FinishCalculation();
                    isTesting = false;
                    text.text = "Testing complete!";
                }
            }
            

        }
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
            if (isTesting)
            {
                CalculateData();
            }
            else
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
        public void ResetTesting()
        {
            isTesting = true;
            hasTestedInhale = false;
            elapseTime = 0;
            amountTested = 0;
            inhaleTester = new SpectrumMinMaxTester(micProvider, _inhaleDataTemplate);
            exhaleLoudnessTester = new ExhaleTester( _exhaleDataTemplate , micProvider);
            exhaleSpectrumTester = new SpectrumMinMaxTester(micProvider, _exhaleDataSpectrumTemplate);
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
}
