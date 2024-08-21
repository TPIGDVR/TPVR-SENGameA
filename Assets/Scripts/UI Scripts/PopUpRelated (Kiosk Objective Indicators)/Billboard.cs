using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Transform mainTransform;
    [SerializeField]
    Transform secondaryTarget;
    void Start()
    {
        mainTransform = Camera.main.transform;

    }

    private void LateUpdate()
    {
        if(secondaryTarget != null)
        {
            var rotationL = Quaternion.LookRotation(mainTransform.forward);
            Vector3 eularRotationL = rotationL.eulerAngles;
            eularRotationL.z = 0;
            transform.rotation = Quaternion.Euler(eularRotationL);
            return;
        }
        //transform.LookAt(transform.position + mainTransform.rotation * Vector3.forward,
        //    mainTransform.rotation * Vector3.up);
        //transform.rotation =                                                   
        var rotation = Quaternion.LookRotation(mainTransform.forward);
        Vector3 eularRotation = rotation.eulerAngles;
        eularRotation.z = 0;
        transform.rotation = Quaternion.Euler(eularRotation);

    }
}
