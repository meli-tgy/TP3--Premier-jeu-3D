using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; // le Player
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Distance / Zoom")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float zoomSpeed = 5f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 0.15f;
    [SerializeField] private float minPitch = 5f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Start Values")]
    [SerializeField] private float startYaw = 0f;
    [SerializeField] private float startPitch = 20f;

    private InputSystem_Actions controls;
    private float yaw;
    private float pitch;

    void Awake()
    {
        controls = new InputSystem_Actions();
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void Start()
    {
        yaw = startYaw;
        pitch = startPitch;
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleZoom();
        HandleRotation();
        UpdateCameraTransform();
    }

    private void HandleZoom()
    {
        if (Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;
        distance -= scroll * zoomSpeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    private void HandleRotation()
    {
        if (Mouse.current == null) return;

        bool rightHeld = Mouse.current.rightButton.isPressed;
        bool leftHeld = Mouse.current.leftButton.isPressed;

        if (!rightHeld && !leftHeld) return;

       
        Vector2 lookDelta = controls.Player.Look.ReadValue<Vector2>();

        yaw += lookDelta.x * rotationSpeed;
        pitch -= lookDelta.y * rotationSpeed;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (rightHeld)
        {
            Vector3 flatForward = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;
            target.rotation = Quaternion.LookRotation(flatForward);
        }
    }

    private void UpdateCameraTransform()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -distance);

        Vector3 focusPoint = target.position + targetOffset;
        transform.position = focusPoint + offset;
        transform.LookAt(focusPoint);
    }
}