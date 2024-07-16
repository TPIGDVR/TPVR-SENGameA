using TMPro;
using Unity.VisualScripting;
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

        IBreathResult inhaleDetection;
        IBreathResult exhaleDetection;
        IBreathResult exhaleSpectrumDetection;

        bool _IsInhaling => inhaleDetection.Result();
        bool _IsExhaling => useVolPitchExhale ? exhaleDetection.Result() : exhaleSpectrumDetection.Result();

        [SerializeField] bool usePresetData;

        [Header("collection Data")]
        [SerializeField] bool isTesting = true;
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
        private void Start()
        {
            //do the initializing here
            inhaleTester = new SpectrumMinMaxTester(micProvider, _inhaleDataTemplate);
            exhaleSpectrumTester = new SpectrumMinMaxTester(micProvider, _exhaleDataSpectrumTemplate);
            exhaleLoudnessTester = new ExhaleTester(_exhaleDataTemplate, micProvider);

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

                print($"Is Inhaling {isInhaling}. Is exhaling {isExhaling}");
                
                if(isExhaling)
                {
                    text.text = "exhaling";
                }
                else if (isInhaling)
                {
                    text.text = "inhaling";
                }
                else
                {
                    text.text = "nothing";
                }
            }
        }

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
    }
}
