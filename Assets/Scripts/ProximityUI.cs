using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityUI : MonoBehaviour
{
    [SerializeField]
    GameObject interactableUI;
    [SerializeField]
    float maxDist;
    [SerializeField]
    Transform player;


    // Start is called before the first frame update
    void Start()
    {
        interactableUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= maxDist)
        {
            interactableUI.SetActive(true);
        }
        else
        {
            interactableUI.SetActive(false);
        }
    }
}
