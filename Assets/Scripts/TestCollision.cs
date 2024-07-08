using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("We touched tips");
    }

    public void SelectTest()
    {
        Debug.Log("WE SELECT THOSE");
    }
}
