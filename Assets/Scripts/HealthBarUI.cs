using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Slider slider;
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 2.2f, 0f);

    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void OnEnable()
    {
        health.OnHealthChanged.AddListener(UpdateBar);
    }

    void OnDisable()
    {
        health.OnHealthChanged.RemoveListener(UpdateBar);
    }

    void Start()
    {
        slider.maxValue = health.MaxHealth;
        slider.value = health.CurrentHealth;
    }

    void LateUpdate()
    {
        if (followTarget != null)
            transform.position = followTarget.position + worldOffset;

        if (mainCamera != null)
            transform.forward = mainCamera.transform.forward; 
    }

    private void UpdateBar(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}