using UnityEngine;

/// <summary>
/// Gestiona los ataques cuerpo a cuerpo (OverlapCircle) y a distancia (Instanciacion de proyectiles) del jugador.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Ataque Cuerpo a Cuerpo (Espada)")]
    [Tooltip("Punto de origen del golpe (Hijo del Player ubicado frente a la espada).")]
    [SerializeField] private Transform attackPoint;
    [Tooltip("Radio del circulo de impacto de la espada.")]
    [SerializeField] private float meleeRange = 1.2f;
    [Tooltip("Dano infligido por cada espadazo.")]
    [SerializeField] private int meleeDamage = 25;

    [Header("Ataque a Distancia (Arco / Proyectil)")]
    [Tooltip("Prefab de la flecha que se instanciara al disparar.")]
    [SerializeField] private GameObject arrowPrefab;
    [Tooltip("Tiempo de enfriamiento entre ataques en segundos.")]
    [SerializeField] private float attackCooldown = 0.45f;

    private Animator animator;
    private float nextAttackTime = 0f;

    private static readonly int MeleeAttackHash = Animator.StringToHash("meleeAttack");
    private static readonly int RangedAttackHash = Animator.StringToHash("rangedAttack");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Time.time < nextAttackTime)
        {
            return;
        }

        // Ataque Cuerpo a Cuerpo: Clic Izquierdo o Tecla J
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J))
        {
            ExecuteMeleeAttack();
            nextAttackTime = Time.time + attackCooldown;
        }
        // Ataque a Distancia: Clic Derecho o Tecla K
        else if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.K))
        {
            ExecuteRangedAttack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void ExecuteMeleeAttack()
    {
        animator.SetTrigger(MeleeAttackHash);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayMeleeSFX();

        // Si olvidaste asignar AttackPoint en el Inspector, usa la posicion del propio Player como respaldo
        Vector2 hitOrigin = attackPoint != null ? attackPoint.position : transform.position;

        // Consulta espacial sin filtro restrictivo de capas para garantizar la deteccion en 2D
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(hitOrigin, meleeRange);

        foreach (Collider2D hit in hitColliders)
        {
            // Ignoramos al propio jugador para que no se autoinflija dano con su espada
            if (hit.gameObject == gameObject || hit.CompareTag("Player"))
            {
                continue;
            }

            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(meleeDamage);
            }
        }
    }

    private void ExecuteRangedAttack()
    {
        animator.SetTrigger(RangedAttackHash);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayBowSFX();

        if (arrowPrefab == null) return;

        Vector3 spawnPos = attackPoint != null ? attackPoint.position : transform.position;
        bool facingRight = transform.localScale.x >= 0f;
        Quaternion arrowRotation = facingRight ? Quaternion.identity : Quaternion.Euler(0f, 0f, 180f);

        Instantiate(arrowPrefab, spawnPos, arrowRotation);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 gizmoCenter = attackPoint != null ? attackPoint.position : transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gizmoCenter, meleeRange);
    }
}