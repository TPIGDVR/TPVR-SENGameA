using Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/kiosk lines")]
public class KioskLines : ScriptableObject
{
    public KLine[] Lines;
}

[System.Serializable]
public class KLine : Line
{
    [Header("Image related")]
    public Sprite image;
    public Vector2 preferredDimension;
    [Header("Others")]
    public AudioClip clip;
    public float duration = 3f;
}