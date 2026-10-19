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
    [SerializeField] private float meleeRange = 0.8f;
    [Tooltip("Dano infligido por cada espadazo.")]
    [SerializeField] private int meleeDamage = 25;
    [Tooltip("Capas (Layers) que seran detectadas como enemigos.")]
    [SerializeField] private LayerMask enemyLayers;

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
        // Control de cadencia de ataque (Cooldown)
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

        if (attackPoint == null) return;

        // Consulta espacial: detecta todos los colliders dentro del circulo de golpe
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, meleeRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(meleeDamage);
            }
        }
    }

    private void ExecuteRangedAttack()
    {
        animator.SetTrigger(RangedAttackHash);

        if (arrowPrefab == null || attackPoint == null) return;

        // Si localScale.x es positivo mira a la derecha (0 grados); si es negativo mira a la izquierda (180 grados)
        bool facingRight = transform.localScale.x >= 0f;
        Quaternion arrowRotation = facingRight ? Quaternion.identity : Quaternion.Euler(0f, 0f, 180f);

        Instantiate(arrowPrefab, attackPoint.position, arrowRotation);
    }

    // Dibuja el radio de ataque en la ventana Scene para depuracion visual
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, meleeRange);
    }
}