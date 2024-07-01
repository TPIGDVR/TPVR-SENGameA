using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Assets.HelpfulScripts
{
    public class ControllerManager : MonoBehaviour
    {
        [Header("Mapping")]
        //currently referencing the quest 3 controller
        [SerializeField] private Mapping[] mapping;
        [Header("Actions")]
        [SerializeField] private ActionsMap[] actionMap;
        Dictionary<Controls, InputAction> dict;
        private void Awake()
        {
            dict = new Dictionary<Controls , InputAction>();
            foreach (var control in mapping)
            {
                dict.Add(control.controls, control.correspondingAction);
            }
            
            foreach(var action in actionMap)
            {
                var inputAction = dict[action.action];
                inputAction.started += action.Pressed;
                inputAction.performed += action.Performed;
                inputAction.canceled += action.Released;
            }
        }

        private void OnDestroy()
        {
            foreach (var action in actionMap)
            {
                var inputAction = dict[action.action];
                inputAction.started -= action.Pressed;
                inputAction.performed -= action.Performed;
                inputAction.canceled -= action.Released;
            }
        }

        private void OnEnable()
        {
            foreach(var control in mapping)
            {
                control.correspondingAction.Enable();
            }
        }

        private void OnDisable()
        {
            foreach( var control in mapping)
            {
                control.correspondingAction.Disable();
            }
        }

        public enum Controls
        {
            //left controls
            LeftJoyStick,
            YButton,
            XButton,
            LeftTrigger,
            LeftGrab,

            //right controls
            RightJoyStick,
            BButton,
            AButton,
            RightTrigger,
            RightGrab,

            //if u want to add additional controls. add from here onwards :)
        }
        [Serializable]
        public class Mapping
        {
            public Controls controls;
            public InputAction correspondingAction;
        }

        [Serializable]
        public class ActionsMap
        {
            public Controls action;
            public UnityEvent OnPress;
            public UnityEvent OnHold;
            public UnityEvent OnRelease;

            public void Pressed(InputAction.CallbackContext context)
            {
                OnPress.Invoke();
            }

            public void Performed(InputAction.CallbackContext context)
            {
                OnHold.Invoke();
            }

            public void Released(InputAction.CallbackContext context)
            {
                OnRelease.Invoke();
            }
        }

        public void OnPress()
        {
            print("button click");
        }

        public void OnHold()
        {
            print("button on hold");
        }

        public void OnRelease()
        {
            print("button on release");
        }
    }
}