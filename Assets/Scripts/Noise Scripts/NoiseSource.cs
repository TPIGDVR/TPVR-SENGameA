using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class NoiseSource : MonoBehaviour
{
    public float _noiseValue;
    public string Name { get => name; }
    public float NoiseValue { get => _noiseValue; }
    public Vector3 Position { get => transform.position; }
    private Outline outline;
    private void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }
    public void ShowOutline()
    {
        outline.enabled = true;
        NoiseUI.Instance.SpawnText(this);
    }

    public void HideOutline()
    {
        outline.enabled = false;
        NoiseUI.Instance.HideText();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, NoiseValue);
    }
}
