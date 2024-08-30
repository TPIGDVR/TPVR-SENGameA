using PGGE.Patterns;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace BreathDetection
{
    public class BreathingAnxietyReduction : MonoBehaviour, IScriptLoadQueuer
    {
        [SerializeField] BreathingDetection detector;
        [Range(0.1f, 1)]
        [SerializeField] float minAnxietyReduction = 0.2f;
        [SerializeField] float maxAnxietyReduction = 0.8f;
        [SerializeField] float maximumInhaleTimer = 3f;
        [SerializeField] float maximumExhaleTimer = 3f;
        [SerializeField] float coolDownPeriod = 4f;
        [SerializeField] float speedToReset = 3f;
        [SerializeField] TextMeshProUGUI displayText;

        public float inhaleElapseTime;
        public float exhaleElapseTime;
        EventManager<PlayerEvents> em = EventSystem.player;

        [Header("Breathing determiner")]
        [SerializeField] int numberOfSample = 100;
        Queue<BreathingOutPut> sample;
        public BreathingOutPut currentOutput { get; private set; }

        public float MinAnxietyReduction { get => minAnxietyReduction; set => minAnxietyReduction = value; }
        public float MaxAnxietyReduction { get => maxAnxietyReduction; set => maxAnxietyReduction = value; }
        public float MaximumInhaleTimer { get => maximumInhaleTimer; set => maximumInhaleTimer = value; }
        public float MaximumExhaleTimer { get => maximumExhaleTimer; set => maximumExhaleTimer = value; }
        public float CoolDownPeriod { get => coolDownPeriod; set => coolDownPeriod = value; }

        FSM _mfsm;

        [Header("UI")]
        [SerializeField] GameObject breathingPanel;
        [SerializeField] Image breathingImage;
        [SerializeField] TextMeshProUGUI stateText;

        [SerializeField] Gradient breathingColorScheme;
        [Header("debugging")]
        public string stateName;

        private void Awake()
        {
            ScriptLoadSequencer.Enqueue(this, (int)LevelLoadSequence.SYSTEM);
        }

        private void Update()
        {
            if (detector.CanRun)
            {
                currentOutput = DetermineBreathingState();
                _mfsm.Update();
                //string message = $"current State: {(States)_mfsm.GetCurrentState().ID}" +
                //    $"currentOutput {currentOutput}";
                //print(message);
                //displayText.stateText = message;
                UpdateText();

            }
            else
            {
                displayText.text = "idle";
            }
        }

        private BreathingOutPut DetermineBreathingState()
        {
            sample.Enqueue(detector.breathingOutPut);

            while(sample.Count > numberOfSample ) sample.Dequeue(); 

            var data = sample.ToArray();
            int inhaleCounter = 0;
            int exhaleCounter = 0;
            for(int i = 0; i < data.Length; i++)
            {
                BreathingOutPut val = data[i];
                switch (val)
                {
                    case BreathingOutPut.INHALE:
                        inhaleCounter++;
                        break;
                    case BreathingOutPut.EXHALE:
                        exhaleCounter++; break;
                }
            }
            //does the counting
            if (inhaleCounter == 0 &&
                exhaleCounter == 0)
            {
                return BreathingOutPut.SILENCE;
            }
            else if(inhaleCounter > exhaleCounter)
            {
                return BreathingOutPut.INHALE;
            }
            else if(exhaleCounter > inhaleCounter)
            {
                return BreathingOutPut.EXHALE;
            }
            return BreathingOutPut.SILENCE;
        }

        public void UpdateInhaleValueUI()
        {
            float normaliseVal = Mathf.Min(inhaleElapseTime, MaximumInhaleTimer) / maximumInhaleTimer;
            UpdateFillColor(normaliseVal);
        }

        public void UpdateExhaleValueUI()
        {
            float normaliseVal = Mathf.Min(exhaleElapseTime, maximumExhaleTimer) / maximumExhaleTimer;
            float normaliseValInhale = Mathf.Min(inhaleElapseTime, MaximumInhaleTimer) / maximumInhaleTimer;

            UpdateFillColor(Mathf.Clamp01(normaliseValInhale - normaliseVal)); 
        }

        public void ResetValueUI()
        {
            StartCoroutine(ResetImage());
        }

        IEnumerator ResetImage()
        {
            while(breathingImage.fillAmount > 0.1)
            {
                
                UpdateFillColor(breathingImage.fillAmount - Time.deltaTime * speedToReset) ;
                yield return null;
            }

            UpdateFillColor(0);
        }

        void UpdateText()
        {
            var currentState = (States)_mfsm.GetCurrentState().ID;
            switch (currentState)
            {
                case States.SILENCE:
                    stateText.text = "Waiting for inhale";
                    break;
                case States.INHALING:
                    stateText.text = "Inhaling";
                    break;
                case States.EXHALING:
                    stateText.text = "Exhaling";
                    break;
                case States.WAITING_FOR_EXHALE:
                    stateText.text = "Waiting for Exhaling";
                    break;
                case States.COOL_DOWN:
                    stateText.text = "Cool Down";
                    break;
            }
        }

        public void ActivateBreathingPanel()
        {
            breathingPanel.SetActive(true);
        }

        public void Initialize()
        {
            detector = GetComponent<BreathingDetection>();
            sample = new Queue<BreathingOutPut>();

            //reset the ui panel
            breathingPanel.SetActive(false);

            //seting up the fsm
            _mfsm = new FSM();
            _mfsm.Add(new SilenceState(_mfsm, (int)States.SILENCE, this));
            _mfsm.Add(new InhaleState(_mfsm, (int)States.INHALING, this));
            _mfsm.Add(new ExhaleState(_mfsm, (int)States.EXHALING, this));
            _mfsm.Add(new WaitForExhaleState(_mfsm, (int)States.WAITING_FOR_EXHALE, this));
            _mfsm.Add(new CoolDownState(_mfsm, (int)States.COOL_DOWN, this));
            _mfsm.SetCurrentState((int)States.SILENCE);

            EventSystem.dialog.AddListener(DialogEvents.ACTIVATE_BREATHING, ActivateBreathingPanel);
        }

        void UpdateFillColor(float normaliseValue)
        {
            breathingImage.color = breathingColorScheme.Evaluate(normaliseValue);
            breathingImage.fillAmount = normaliseValue;
        }
    }


    #region states
    public class BreathingState : FSMState
    {
        protected BreathingAnxietyReduction anxietyReducer;
        protected BreathingOutPut curState => anxietyReducer.currentOutput;
        public BreathingState(FSM fsm, int id, BreathingAnxietyReduction anxietyReduction) : base(fsm, id)
        {
            this.anxietyReducer = anxietyReduction;
        }
    }

    public class SilenceState : BreathingState
    {
        public SilenceState(FSM fsm, int id, BreathingAnxietyReduction anxietyReduction) : base(fsm, id, anxietyReduction)
        {
        }

        public override void Update()
        {
            if(curState == BreathingOutPut.INHALE)
            {
                mFsm.SetCurrentState((int)States.INHALING);
            }
        }
    }
    public class InhaleState : BreathingState
    {
        public InhaleState(FSM fsm, int id, BreathingAnxietyReduction anxietyReduction) : base(fsm, id, anxietyReduction)
        {
        }

        public override void Update()
        {
            switch (curState)
            {
                case BreathingOutPut.SILENCE:
                    mFsm.SetCurrentState((int)States.WAITING_FOR_EXHALE);
                    break;
                case BreathingOutPut.INHALE:
                    anxietyReducer.inhaleElapseTime += Time.deltaTime;
                    break;
                case BreathingOutPut.EXHALE:
                    mFsm.SetCurrentState((int)States.EXHALING);
                    break;
            }

            anxietyReducer.UpdateInhaleValueUI();
        }

        public override void Exit()
        {
            anxietyReducer.inhaleElapseTime = Mathf.Min(
                anxietyReducer.MaximumInhaleTimer, 
                anxietyReducer.inhaleElapseTime);
        }
    }
    public class ExhaleState : BreathingState
    {
        EventManager<PlayerEvents> em = EventSystem.player;

        public ExhaleState(FSM fsm, int id, BreathingAnxietyReduction anxietyReduction) : base(fsm, id, anxietyReduction)
        {
        }

        public override void Update()
        {
            switch (curState)
            {
                case BreathingOutPut.SILENCE:
                    mFsm.SetCurrentState((int)States.COOL_DOWN);
                    break;
                case BreathingOutPut.EXHALE:
                    anxietyReducer.exhaleElapseTime += Time.deltaTime;
                    break;
            }
            anxietyReducer.UpdateExhaleValueUI();

        }


        public override void Exit()
        {
            bool hasInhale = anxietyReducer.inhaleElapseTime > 0;
            if (!hasInhale) return;
            anxietyReducer.exhaleElapseTime = Mathf.Min(anxietyReducer.exhaleElapseTime, anxietyReducer.MaximumExhaleTimer);
            CalculateAnxietyReduction();
            anxietyReducer.exhaleElapseTime = 0f;
            anxietyReducer.inhaleElapseTime = 0f;

            anxietyReducer.ResetValueUI();
        }

        void CalculateAnxietyReduction()
        {
            //number of 
            float minInhaleTimer = anxietyReducer.MaximumInhaleTimer / (float)3;
            float minExhaleTimer = anxietyReducer.MaximumExhaleTimer / (float)3;

            if (anxietyReducer.inhaleElapseTime < minInhaleTimer || anxietyReducer.exhaleElapseTime < minExhaleTimer) return;


            //sum up the inhale and exhale elapse time to get the amount of 
            float percentageAchieve = Mathf.InverseLerp(0, 2,
                (anxietyReducer.inhaleElapseTime - minInhaleTimer / 
                anxietyReducer.MaximumInhaleTimer - minInhaleTimer ) +
                (anxietyReducer.exhaleElapseTime - minExhaleTimer / 
                anxietyReducer.MaximumExhaleTimer - minExhaleTimer)
                );

            em.TriggerEvent<float>(PlayerEvents.ANXIETY_BREATHE, percentageAchieve);
        }
    }

    public class WaitForExhaleState : BreathingState
    {
        //put a timerOffset here
        public WaitForExhaleState(FSM fsm, int id, BreathingAnxietyReduction anxietyReduction) : base(fsm, id, anxietyReduction)
        {
        }

        public override void Update()
        {
            if (curState == BreathingOutPut.EXHALE) 
                mFsm.SetCurrentState((int)States.EXHALING);
        }
    }

    public class CoolDownState : BreathingState
    {
        float elapseTime = 0f;


        public CoolDownState(FSM fsm, int id, BreathingAnxietyReduction anxietyReduction) : base(fsm, id, anxietyReduction)
        {
        }

        public override void Enter()
        {
            elapseTime = 0f;
        }

        public override void Update()
        {
            if(elapseTime < anxietyReducer.CoolDownPeriod)
            {
                elapseTime += Time.deltaTime;
            }
            else
            {
                mFsm.SetCurrentState((int)States.SILENCE);
            }
        }
    }

    public enum States
    {
        SILENCE,
        INHALING,
        EXHALING,
        WAITING_FOR_EXHALE,
        COOL_DOWN
    }

    #endregion
}