using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SphereCollider))]
public class HandsScanning : MonoBehaviour
{
    [SerializeField]private InputActionReference inputActionReference;
    [SerializeField]private SphereCollider sphereCollider;


    private void OnEnable()
    {
        inputActionReference.action.performed += OnActionPerformed;
        inputActionReference.action.canceled += OnActionCanceled;

    }

    private void OnActionPerformed(InputAction.CallbackContext obj) => sphereCollider.enabled = true;

    private void OnActionCanceled(InputAction.CallbackContext obj) => sphereCollider.enabled = false;
}
