using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapBehaviour : MonoBehaviour, IScriptLoadQueuer
{
    [SerializeField] float yOffset = 6f;
    Transform playerTransform;

    public void Initialize()
    {
        playerTransform = GameData.player.PlayerTransform;
    }

    private void Awake()
    {
        ScriptLoadSequencer.Enqueue(this, (int)LevelLoadSequence.LEVEL);
    }

    private void Update()
    {
        Vector3 finalPosition = playerTransform.position;
        finalPosition.y = yOffset;
        transform.position = finalPosition;
    }
}
