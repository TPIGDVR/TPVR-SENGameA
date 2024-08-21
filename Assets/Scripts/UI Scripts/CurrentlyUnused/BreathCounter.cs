//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.InputSystem;
//using UnityEngine.XR.Interaction.Toolkit;
//using UI;
//using Unity.VisualScripting;
//using UnityEditor;

//public class BreathCounter : UIBase
//{
//    [Header("breath counter")]
//    public Scrollbar progressBar;
//    //private float leftTriggerPressed, rightTriggerPressed;
//    //public float size;
//    public GameObject handle;
//    public TapManager tapManager; // Reference to the TapManager script.
//    public ProximityHapticFeedback proximityHapticFeedback;
//    public Material barMat;
//    public Color barColor;
//    private SlideShowImage imageComponent;


//    public LayerMask obstructionLayer;
//    public GameObject visualizer;
//    [Range(0,1)]
//    [SerializeField] private float BreathIncreaseRatePerBar; 
//    //increase rate make sure to follow the time.deltatime
//    private float increaseRate { get {
//            //one bar is about 0.25 fill
//            //one charge is about 0.25 = 1second
//            return (0.35f / BreathIncreaseRatePerBar) * Time.deltaTime;
//        }}
//    //if you want to reduce the anxiety by 25% then the reduction to be 1 - 25% = 0.75
//    //since we are just doing an overall reduction
//    [Range(0, 1)]
//    [SerializeField] private float anxietyReductionForDeep = 0.75f;
//    [Range(0, 1)]
//    [SerializeField] private float anxietyReductionForMedium = 0.85f;
//    [Range(0, 1)]
//    [SerializeField] private float anxietyReductionForShallow = 0.92f;
//    [Range(0, 1)]
//    [SerializeField] private float anxietyReductionForlittle = 0.96f;
//    //will tell if the breath counter can run or not
//    private bool canBreath = false;
//    private bool hasFinishedBreathing = false;
//    private BreathingState currentState;
//    private BreathingState prevState;
//    EventManager<PlayerEvents> em = EventSystem.player;

//    void Start()
//    {
//        SetScrollbarSize(0f);
//        //barMat.color = barColor;
//        imageComponent = handle.GetComponent<SlideShowImage>();

        
//    }

//    protected override void Update()
//    {
//        base.Update();
//        if (progressBar.size != 0)
//        {
//            visualizer.SetShow(true);
//            ChangeBarColor();
//        }
//        else
//        {
//            //once the visualizer is set to zero decide on the effect
//            visualizer.SetShow(false);
//            DecideEffect();
//        }

//        Breath();
//    }

//    public void SetScrollbarSize(float size)
//    {
//        // Ensure the size stays within the range [0, 1]
//        size = Mathf.Clamp01(size);

//        // Set the size of the scrollbar handle
//        progressBar.size = size;
//    }

//    public void Breath()
//    {
//        if (canBreath)
//        {
//            hasFinishedBreathing = false;
//            SetScrollbarSize(progressBar.size + increaseRate);
//        }
//        else 
//        {
//            if (!hasFinishedBreathing)
//            {
//                hasFinishedBreathing = true;
//                ///record down the previous breath state
//                prevState = currentState;
//            }
//            SetScrollbarSize(progressBar.size - increaseRate);
//        }
//        DecideBreathingState();
        
//    }

//    private void DecideBreathingState()
//    {
//        if(progressBar.size > 0.75)
//        {
//            currentState = BreathingState.Deep;
//        }
//        else if(progressBar.size > 0.5)
//        {
//             currentState= BreathingState.Medium;
//        }
//        else if(progressBar.size > 0.25)
//        {
//            currentState = BreathingState.Shallow;
//        }
//        else if(progressBar.size > 0.1)
//        {
//            currentState = BreathingState.litte;
//        }
//    }

//    //probably have to change this
//    public void ChangeBarColor()
//    {
//        switch (currentState)
//        {
//            case BreathingState.Deep:
//                imageComponent.color = new Color32(0, 255, 255, 255);
//                break;
//            case BreathingState.Medium:
//                imageComponent.color = new Color32(0, 255, 0, 255);
//                break;
//            case BreathingState.Shallow:
//                imageComponent.color = new Color32(255, 165, 0, 255);
//                break;
//            case BreathingState.None:
//                imageComponent.color = new Color32(255, 0, 0, 255);
//                break;
//        }
//    }

//    public void StartBreathCounter()
//    {
//        canBreath = true;
//    }

//    public void DecideEffect()
//    {
//        switch (prevState)
//        {
//            case BreathingState.litte:
//                proximityHapticFeedback.timeWithinMaxDistance *= anxietyReductionForlittle;
//                Breath(anxietyReductionForlittle);
//                break;
//            case BreathingState.Shallow:
//                proximityHapticFeedback.timeWithinMaxDistance *= anxietyReductionForShallow;
//                Breath(anxietyReductionForShallow);
//                break;
//            case BreathingState.Medium:
//                proximityHapticFeedback.timeWithinMaxDistance *= anxietyReductionForMedium;
//                Breath(anxietyReductionForMedium);
//                break;
//            case BreathingState.Deep:
//                proximityHapticFeedback.timeWithinMaxDistance *= anxietyReductionForDeep;
//                Breath(anxietyReductionForDeep);
//                break;
//        }
//        //reset the state
//        prevState = BreathingState.None;

//        void Breath(float decrease)
//        {
//            em.TriggerEvent<float>(PlayerEvents.ANXIETY_BREATHE, decrease);
//        }
//    }

//    public void StopBreathCounter()
//    {
//        canBreath = false;
//    }

//    private enum BreathingState
//    {
//        None,
//        litte,
//        Shallow,
//        Medium,
//        Deep,
//    }
//}
