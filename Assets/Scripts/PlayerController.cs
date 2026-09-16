using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Health))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 1f;

    private CharacterController controller;
    private Animator animator;
    private Camera mainCamera;
    private Health selfHealth;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isDead;

    private InputSystem_Actions controls;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool attackPressed;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        selfHealth = GetComponent<Health>();
        mainCamera = Camera.main;
        controls = new InputSystem_Actions();

        selfHealth.OnDeath.AddListener(Die);
    }

    void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Jump.performed += OnJumpPerformed;
        controls.Player.Attack.performed += OnAttackPerformed;
    }

    void OnDisable()
    {
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Attack.performed -= OnAttackPerformed;
        controls.Player.Disable();
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx) => jumpPressed = true;
    private void OnAttackPerformed(InputAction.CallbackContext ctx) => attackPressed = true;

    void Update()
    {
        moveInput = controls.Player.Move.ReadValue<Vector2>();

        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        HandleAttack();
        ApplyGravity();
        UpdateAnimator();

        jumpPressed = false;
        attackPressed = false;
    }

    private void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }
    }

    private void HandleMovement()
    {
        Vector2 input = Vector2.ClampMagnitude(moveInput, 1f);
        if (input.sqrMagnitude < 0.0001f) return;

        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = camForward * input.y + camRight * input.x;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        bool rightClickHeld = Mouse.current != null && Mouse.current.rightButton.isPressed;
        if (!rightClickHeld && moveDirection.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }
    }

    private void HandleAttack()
    {
        if (attackPressed)
        {
            animator.SetTrigger("Attack");
        }
    }
     public void PunchHit()
    {
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRadius);

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Monster")) continue;

            Health targetHealth = hit.GetComponentInParent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(selfHealth.AttackPower);
            }
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void UpdateAnimator()
    {
        float currentSpeed = moveInput.magnitude;
        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetBool("IsDead", true);
        StartCoroutine(FallThenDisable());
    }

    private System.Collections.IEnumerator FallThenDisable()
    {
        moveInput = Vector2.zero;

        while (!controller.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            yield return null;
        }

        controller.enabled = false;
        enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}