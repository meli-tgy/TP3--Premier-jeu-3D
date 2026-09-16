using UnityEngine;
using UnityEngine.InputSystem;

public class DayNightCycle : MonoBehaviour
{
    [Header("Cycle Duration")]
    [Tooltip("Durée réelle (en secondes) d'un cycle complet de 24h virtuelles")]
    [SerializeField] private float dayLengthInSeconds = 12; // en secondes

    [Header("Start")]
    [Tooltip("Heure de départ (0 = minuit, 12 = midi)")]
    [SerializeField] private float startTime = 8f;

    [Header("Light Intensity")]
    [SerializeField] private float maxIntensity = 1.2f;
    [SerializeField] private float minIntensity = 0.05f;

    [Header("Time Acceleration")]
    [Tooltip("Multiplicateur de vitesse appliqué en maintenant la touche T")]
    [SerializeField] private float accelerationMultiplier = 5f;

    private Light sunLight;
    private float currentTime; // en heures

    void Awake()
    {
        sunLight = GetComponent<Light>();
        currentTime = startTime;
    }

    void Update()
    {
        AdvanceTime();
        RotateSun();
        UpdateIntensity();
    }

    private void AdvanceTime()
    {
        float multiplier = 1f;

        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.tKey.isPressed)
        {
            multiplier = accelerationMultiplier;
        }

        float hoursPerSecond = 24f / dayLengthInSeconds;
        currentTime += hoursPerSecond * Time.deltaTime * multiplier;
        currentTime %= 24f; 
    }

    private void RotateSun()
    {
        
        float sunAngle = (currentTime / 24f) * 360f - 90f;
        transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);
    }

    private void UpdateIntensity()
    {
        
        float sunHeight = Vector3.Dot(-transform.forward, Vector3.up);
        float normalizedHeight = Mathf.Clamp01(sunHeight);

        sunLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, normalizedHeight);
    }
}