using Patterns;
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
        [SerializeField] public ActionsMap[] actionMap;
        public Dictionary<Controls, InputAction> dict;
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

        //mapping allow one to adjust the action with the enum called controls
        [Serializable]
        public class Mapping
        {
            public Controls controls;
            public InputAction correspondingAction;
        }

        //the events that is related to the controls
        [Serializable]
        public class ActionsMap
        {
            public Controls action;
            public UnityEvent OnPress;
            public UnityEvent OnHold;
            public UnityEvent OnRelease;

            public void Pressed(InputAction.CallbackContext context)
            {
                print($"{action} is being press");
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

        public void AddOnPressEvent(Controls assignControl, UnityAction onPress)
        {
            print($"Adding");

            foreach (var map in actionMap)
            {
                print($"{map.action} in actionmap");
                if(map.action == assignControl)
                {
                    map.OnPress.AddListener(onPress);
                    return;
                }
            }
            throw new NullReferenceException("Assign a mapping to the designated assign control before adding an event!");
        }

        public void AddOnHoldEvent(Controls assignControl, UnityAction onHold)
        {
            foreach (var map in actionMap)
            {
                if (map.action == assignControl)
                {
                    map.OnHold.AddListener(onHold);
                    return;
                }
            }
            throw new NullReferenceException("Assign a mapping to the designated assign control before adding an event!");
        }

        public void AddOnReleaseEvent(Controls assignControl, UnityAction onRelease)
        {
            foreach (var map in actionMap)
            {
                if (map.action == assignControl)
                {
                    map.OnRelease.AddListener(onRelease);
                    return;

                }
            }
            throw new NullReferenceException("Assign a mapping to the designated assign control before adding an event!");
        }


        public void RemovePressEvent(Controls assignControl, UnityAction onPress)
        {
            foreach (var map in actionMap)
            {
                if (map.action == assignControl)
                {
                    map.OnPress.RemoveListener(onPress);
                    return;

                }
            }
            throw new NullReferenceException("Assign a mapping to the designated assign control before adding an event!");
        }

        public void RemoveHoldEvent(Controls assignControl, UnityAction onHold)
        {
            foreach (var map in actionMap)
            {
                if (map.action == assignControl)
                {
                    map.OnHold.RemoveListener(onHold);
                    return;

                }
            }
            throw new NullReferenceException("Assign a mapping to the designated assign control before adding an event!");
        }

        public void RemoveReleaseEvent(Controls assignControl, UnityAction onRelease)
        {
            foreach (var map in actionMap)
            {
                if (map.action == assignControl)
                {
                    map.OnRelease.RemoveListener(onRelease);
                    return;

                }
            }
            throw new NullReferenceException("Assign a mapping to the designated assign control before adding an event!");
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

}