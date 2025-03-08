using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConnection : MonoBehaviour
{
    LineRenderer lineRenderer;
    [SerializeField] Transform target;

    private void Awake()
    {
        EventSystem.level.AddListener(LevelEvents.FINISH_TUTORIAL, OnEndTutorial);
    }
    
    private void OnDestroy()
    {
        EventSystem.level.RemoveListener(LevelEvents.FINISH_TUTORIAL, OnEndTutorial);
    }
    
    void OnEndTutorial()
    {
        EventSystem.level.RemoveListener(LevelEvents.FINISH_TUTORIAL, OnEndTutorial);
        gameObject.SetActive(false);
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        lineRenderer.SetPosition(0, transform.localPosition);
        lineRenderer.SetPosition(1, transform.InverseTransformPoint(target.position));
    }
}