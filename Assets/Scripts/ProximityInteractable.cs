using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityInteractable : MonoBehaviour
{
    // Start is called before the first frame update
    Outline outline;
    public Transform player;
    public GameObject intBall;
    void Start()
    {
        intBall.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.position, this.transform.position) < 1f)
        {
            intBall.SetActive(true);
        }
        else
        {
            intBall.SetActive(false);
        }
    }
}
