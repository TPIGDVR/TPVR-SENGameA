using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayerMovementScript : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ);
        transform.position += move * moveSpeed * Time.deltaTime;
    }
}
