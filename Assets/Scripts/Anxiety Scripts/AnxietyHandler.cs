using UnityEngine;


public class AnxietyHandler : MonoBehaviour
{
    public float _anxietyLevel = 0;
    NoiseProximityHandler _noiseProximityHandler;

    [SerializeField]
    float _maxNoiseLevel;

    [SerializeField]
    float _maxAnxietyLevel,_anxietyIncreaseSpeed;
    [SerializeField]
    float _minAnxietyIncreaseScale = 0;
    [SerializeField]
    float _maxAnxietyIncreaseScale = 3;
    float _anxietyIncreaseScale = 0;

    EventManager em = EventManager.Instance;
    bool canStart = false;

    [SerializeField] private bool test = false;

    private void Start()
    {
        _noiseProximityHandler = GetComponent<NoiseProximityHandler>();
        em.AddListener<float>(Event.ANXIETY_BREATHE, Breath);
        em.AddListener(Event.START_GAME, StartGame);
    }

    private void Update()
    {
        if (!canStart) return; //dont run until it can start
        CalculateAnxietyScaleBasedOffNoiseLevel();
        IncrementAnxietyLevel();

        //clamp the anxiety level.
        _anxietyLevel = Mathf.Clamp(_anxietyLevel,0, _maxAnxietyLevel);
        //trigger the event after calculating the anxiety level
        em.TriggerEvent<float>(Event.ANXIETY_UPDATE, _anxietyLevel / _maxAnxietyLevel);
    }

    void StartGame()
    {
        if (test) return;
        canStart = true;
    }

    /// <summary>
    /// Calculate the anxiety that should be increase based on the noise
    /// that is around the player.
    /// </summary>
    void CalculateAnxietyScaleBasedOffNoiseLevel()
    {
        //change the anxiety increase scale baded on the noise level
        _anxietyIncreaseScale = Mathf.Lerp(_minAnxietyIncreaseScale
            ,_maxAnxietyIncreaseScale
            ,_noiseProximityHandler.TotalNoiseValue / _maxNoiseLevel);
    }

    /// <summary>
    /// Increase the anxiety of the player as time goes on.
    /// </summary>
    void IncrementAnxietyLevel()
    {
        //current anxiety level = current anxiety speed * scale
        _anxietyLevel += (Time.deltaTime * _anxietyIncreaseSpeed) * _anxietyIncreaseScale;
    }

    /// <summary>
    /// Amount to decrease if they manage to breathe properly
    /// </summary>
    /// <param name="decrease">The percentage to decrease it by.</param>
    void Breath(float decrease)
    {
        _anxietyLevel *= decrease;
    }

    #region Debugging purposes
    /// <summary>
    /// Stop the anxiety from increase.
    /// For debugging purposes in the inspector
    /// </summary>
    [ContextMenu("stop anxiety")]
    void StopAnxietyIncrease()
    {
        canStart = false;
        _anxietyLevel = 0;
    }

    /// <summary>
    /// Restart the anxiety to start increasing
    /// For debugging purposes in the inspector
    /// </summary>
    [ContextMenu("StartAnxiety")]
    void StartAnxietyIncrease()
    {
        canStart = true;
    }
    #endregion
}
