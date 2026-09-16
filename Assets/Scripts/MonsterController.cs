using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Health))]
public class MonsterController : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float rotationSpeed = 8f;

    private Animator animator;
    private Health selfHealth;
    private Health playerHealth;
    private Transform playerTransform;
    private Collider selfCollider;
    private bool playerInRange;
    private bool isDead;
    private float attackTimer;

    void Awake()
    {
        animator = GetComponent<Animator>();
        selfHealth = GetComponent<Health>();
        selfCollider = GetComponent<Collider>();

        selfHealth.OnDeath.AddListener(Die);
    }

    void Update()
    {
        if (!playerInRange || playerTransform == null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsAttacking", false);
            return;
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool inAttackRange = distance <= attackRange;

        FacePlayer();

        if (inAttackRange)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsAttacking", true);
            HandleAttack();
        }
        else
        {
            animator.SetBool("IsAttacking", false);
            animator.SetFloat("Speed", 1f);
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position);
        direction.y = 0f;
        direction.Normalize();

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void FacePlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position);
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void HandleAttack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f && playerHealth != null)
        {
            playerHealth.TakeDamage(selfHealth.AttackPower);
            attackTimer = attackCooldown;
        }
    }

    public void OnPlayerEnter(Health target, Transform targetTransform)
    {
        if (isDead) return;
        playerInRange = true;
        playerHealth = target;
        playerTransform = targetTransform;
        attackTimer = 0f;
    }

    public void OnPlayerExit()
    {
        playerInRange = false;
        playerHealth = null;
        playerTransform = null;
    }
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetBool("IsDead", true);
        if (selfCollider != null) selfCollider.enabled = false;

        SnapToGround();
        enabled = false;
    }

    private void SnapToGround()
    {
        Vector3 origin = transform.position + Vector3.up * 2f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 10f, ~0, QueryTriggerInteraction.Ignore))
            transform.position = hit.point;
    }
}