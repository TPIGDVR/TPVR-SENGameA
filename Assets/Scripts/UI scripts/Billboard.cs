using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Transform mainTransform;
    void Start()
    {
        mainTransform = Camera.main.transform;
        Debug.Log(mainTransform);
    }

    private void LateUpdate()
    {
        //transform.LookAt(transform.position + mainTransform.rotation * Vector3.forward,
        //    mainTransform.rotation * Vector3.up);
        //transform.rotation =                                                   
        var rotation = Quaternion.LookRotation(mainTransform.forward);
        Vector3 eularRotation = rotation.eulerAngles;
        eularRotation.z = 0;
        transform.rotation = Quaternion.Euler(eularRotation);

    }
}
