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

        IBreathResult inhaleDetection;
        IBreathResult exhaleDetection;

        bool _IsInhaling => inhaleDetection.Result();
        bool _IsExhaling => exhaleDetection.Result();

        [SerializeField] bool usePresetData;

        [Header("collection Data")]
        [SerializeField] bool isTesting = true;
        [SerializeField] int amountToTest = 2;
        [SerializeField] float amountOfTimeToSample = 2f;
        int amountTested = 0;
        bool hasTestedInhale = false;

        float elapseTime = 0;

        InhaleTester inhaleTester;
        ExhaleTester exhaleTester;
        InhaleTester exhaleSpectrumTester;

        [Header("debugging")]
        [SerializeField] TextMeshProUGUI text;
        private void Start()
        {
            //do the initializing here
            inhaleTester = new InhaleTester(micProvider, _inhaleDataTemplate);
            exhaleSpectrumTester = new InhaleTester(micProvider, _exhaleDataSpectrumTemplate);
            //exhaleTester = new ExhaleTester(_exhaleDataTemplate, micProvider);
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
                    exhaleSpectrumTester.Run();
                    text.text = "Please exhale";
                }
                elapseTime += Time.deltaTime;
            }
            else
            {
                if (hasTestedInhale) amountTested++;

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

            inhaleDetection = new InhalingDetector(micProvider, calculatedInhaleData);
            exhaleDetection = new InhalingDetector(micProvider, calculateExhaleSpectrumData);

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

                if(isInhaling && isExhaling) 
                {
                    text.text = "unknown";
                }
                else if (isInhaling)
                {
                    text.text = "inhaling";
                }
                else if(isExhaling)
                {
                    text.text = "exhaling";
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
            isTesting = false;
            hasTestedInhale = false;
            amountTested = 0;
            inhaleTester = new InhaleTester(micProvider, _inhaleDataTemplate);
            exhaleTester = new ExhaleTester( _exhaleDataTemplate , micProvider);
            exhaleSpectrumTester = new InhaleTester(micProvider, _exhaleDataSpectrumTemplate);
        }

        [ContextMenu("Update values")]
        public void UpdateValues()
        {
            //do changes this later since this is kinda effy way to do it.
            inhaleDetection = new InhalingDetector(micProvider , calculatedInhaleData);
            //exhaleDetection = new ExhalingDetector(micProvider , calculatedExhaleData);
            exhaleDetection = new InhalingDetector(micProvider, calculateExhaleSpectrumData);
        }
    }



}
