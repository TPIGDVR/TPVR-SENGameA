using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using System.Linq.Expressions; // Ensure you import the appropriate namespace.

public class ProximityHapticFeedback : MonoBehaviour
{
    public XRBaseControllerInteractor xRRightBaseController;
    public XRBaseControllerInteractor xRLeftBaseController;

    //public Transform musicBox;
    public List<Transform> musicBoxes;
    public float maxDistance = 7f;
    public float minDistance = 1f;
    public float minVolume = 0.1f; // Minimum volume
    public float maxVolume = 1f;   // Maximum volume
    public float maxAmplitude = 1f;
    public float rhythmMultiplier = 2f;
    public float maxDuration = 5f; // Maximum faintDuration to trigger lens distortion
    public float deathTimer = 25f;
    [Tooltip("the anxiety bar when the player starts the game")]
    [SerializeField] private float startingAnxietyScale = 0.65f;
    private float anxietyScale;

    public GameObject lensDistortion;
    public GameObject lensFlare;
    public GameObject vignette;
    public GameObject talkingAudio;
    public AudioSource audioSource;

    private float distance;
    public float timeWithinMaxDistance = 0f;
    public bool isWithinMaxDistance = false;

    public Image vibrationBar;
    public Image anxietyBar;
    [Header("Logo")]
    public Image anxietyLogo;
    public Sprite[] anxietyLogoSprite = new Sprite[4]; //do the change by ascending and only allow 4
    private float overallAmplitude;

    public Volume volume; // Reference to the Volume component.
    private Vignette vignetteEffect;
    public float startIntensity = 0.1f;
    public float targetIntensity = 1f;
    public float faintDuration = 8f; // Time in seconds to go from 0 to 1.
    private bool faint = false;

    private float initialAudioVolume;

    public float timeDecayRate = 0.5f; // Adjust the decay rate as needed
 
    PostProcessing postProcessing;

    public bool Faint { get => faint; }

    void Start()
    {
        volume.profile.TryGet(out vignetteEffect);
        lensDistortion.SetActive(true);
        lensFlare.SetActive(false);
        audioSource.volume = minVolume;
        audioSource.Play();

        postProcessing = lensDistortion.GetComponent<PostProcessing>();
    }

    void Update()
    {
        distance = GetClosestMusicBoxFromPlayer();
        CheckIfMaxDistance();
        RunNormally();
        UpdateHapticFeedback();
        vibrationBar.fillAmount = overallAmplitude;

        float progress = Mathf.InverseLerp(0f, 25f, timeWithinMaxDistance);
        UpdateProgressBar(progress);
        ChangeLogo(progress);
        postProcessing.UpdatePostProcess(progress);


        //functions
        float GetClosestMusicBoxFromPlayer()
        {
            initialAudioVolume = audioSource.volume;

            float closestDistance = float.MaxValue;

            foreach (Transform box in musicBoxes)
            {
                float boxDistance = Vector3.Distance(transform.position, box.position);

                if (boxDistance < closestDistance)
                {
                    closestDistance = boxDistance;
                    //musicBox = box;
                }
            }

            return closestDistance;
        }
        void ChangeLogo(float progress)
        {
            Sprite chosenSprite;
            if (progress < 0.25f)
            {//less than 25 percentage
                chosenSprite = anxietyLogoSprite[0];
            }
            else if (progress < .5f)
            {
                chosenSprite = anxietyLogoSprite[1];
            }
            else if (progress < .75f)
            {
                chosenSprite = anxietyLogoSprite[2];
            }
            else
            {
                chosenSprite = anxietyLogoSprite[3];
            }

            anxietyLogo.sprite = chosenSprite;
        }
        void UpdateProgressBar(float progress)
        {
            anxietyBar.fillAmount = progress;
        }

        void RunNormally()
        {
            if (Faint) return;

            if (isWithinMaxDistance)
            {
                timeWithinMaxDistance += Time.deltaTime * anxietyScale; //uncomment when not testing
            }
            else
            {
                // Gradually decrease timeWithinMaxDistance to 0.
                timeWithinMaxDistance -= timeDecayRate * Time.deltaTime;
            }
            timeWithinMaxDistance = Mathf.Clamp(timeWithinMaxDistance, 0f, 25f); // Ensure it stays within the valid range
            if(timeWithinMaxDistance == deathTimer && !Faint)
            {
                faint = true;
                StartFaint();
            }
        }
        void CheckIfMaxDistance()
        {
            if (distance <= maxDistance)
            {
                isWithinMaxDistance = true;
            }
            else
            {
                isWithinMaxDistance = false;
            }
        }
    }

    private void UpdateHapticFeedback()
    {
        if (distance <= maxDistance)
        {
            float proximityAmplitude = Mathf.InverseLerp(maxDistance, minDistance, distance) * maxAmplitude;

            // Run rhythm-based intensity
            float rhythmIntensity = Mathf.Sin(Time.time * rhythmMultiplier);

            // Combine proximity and rhythm intensity for haptic feedback
            overallAmplitude = proximityAmplitude * Mathf.Clamp01(rhythmIntensity);
            //Debug.Log(overallAmplitude);

            // Run volume based on proximity
            if (!Faint)
            {
                float proximityVolume = Mathf.Lerp(minVolume, maxVolume, Mathf.InverseLerp(maxDistance, minDistance, distance));
                audioSource.volume = proximityVolume;
            }
        }
        else
        {
            // Reset overallAmplitude to 0 when the player is outside the proximity zone.
            overallAmplitude = 0f;
        }

        //xRRightBaseController.SendHapticImpulse(overallAmplitude, 0.1f); // Adjust faintDuration as needed
        //xRLeftBaseController.SendHapticImpulse(overallAmplitude, 0.1f);  // Adjust faintDuration as neededp
    }

    private void StartFaint()
    {
        talkingAudio.SetActive(false);
        postProcessing.Dead();
    }

    public void Initialize()
    {
        anxietyScale = startingAnxietyScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}