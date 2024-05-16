using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinChair : MonoBehaviour
{
    [SerializeField] private GameObject instruction;
    [SerializeField] private float activationRange = 1.5f;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position , player.transform.position) <= activationRange)
        {
            instruction.SetActive(true);
        }
        else { instruction.SetActive(false); }
    }

    public void Win()
    {
        SceneManager.LoadScene("Win Scene");
        Debug.Log("WINNER WINNER CHICKEN DINNER");
    }
}
