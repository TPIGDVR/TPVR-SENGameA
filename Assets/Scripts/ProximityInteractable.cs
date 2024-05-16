using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityInteractable : MonoBehaviour
{
    // Start is called before the first frame update
    Outline outline;
    public Transform player;
    void Start()
    {
        outline = GetComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.position, this.transform.position) < 3f)
        {
            outline.enabled = true;
        }
        else
        {
            outline.enabled = false;
        }
    }
}
