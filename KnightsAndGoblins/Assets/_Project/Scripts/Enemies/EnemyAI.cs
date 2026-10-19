using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controlador de Inteligencia Artificial para enemigos con soporte hibrido (NavMesh 2D + Cinematica Vectorial).
/// Ejecuta estados de Vigilancia, Persecucion, Retorno y Ataque cuerpo a cuerpo.
/// </summary>
[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    [Header("Sensores de IA")]
    [Tooltip("Velocidad de desplazamiento del enemigo.")]
    [SerializeField] private float moveSpeed = 3.2f;
    [Tooltip("Radio en el que el enemigo detecta al jugador y comienza a perseguirlo.")]
    [SerializeField] private float detectionRange = 6f;
    [Tooltip("Distancia minima a la que se detiene para golpear al jugador.")]
    [SerializeField] private float attackRange = 1.2f;

    [Header("Parametros de Combate")]
    [Tooltip("Dano que inflige al caballero por cada golpe.")]
    [SerializeField] private int attackDamage = 10;
    [Tooltip("Tiempo de espera entre ataques consecutivos.")]
    [SerializeField] private float attackCooldown = 1.2f;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform playerTarget;
    private Vector3 initialGuardPosition;
    private float nextAttackTime = 0f;
    private bool isFacingRight = true;

    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int AttackHash = Animator.StringToHash("attack");

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Guardamos la posicion exacta en 2D (forzando Z = 0)
        initialGuardPosition = new Vector3(transform.position.x, transform.position.y, 0f);
        transform.position = initialGuardPosition;

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }

        // Si el NavMeshAgent no logro engancharse a la malla al iniciar, lo apagamos
        // para que actue el motor de respaldo vectorial sin lanzar warnings en consola.
        if (agent != null && !agent.isOnNavMesh)
        {
            agent.enabled = false;
        }
    }

    private void Update()
    {
        if (playerTarget == null || !playerTarget.gameObject.activeInHierarchy)
        {
            StopMovement();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        // Estado 1: En rango de ataque -> Se detiene y golpea
        if (distanceToPlayer <= attackRange)
        {
            StopMovement();
            FaceTarget(playerTarget.position);
            TryAttackPlayer();
        }
        // Estado 2: En rango de deteccion -> Persigue al jugador
        else if (distanceToPlayer <= detectionRange)
        {
            MoveTowardsTarget(playerTarget.position);
        }
        // Estado 3: Jugador fuera de rango -> Regresa a su puesto de guardia
        else
        {
            ReturnToGuardPost();
        }
    }

    private void MoveTowardsTarget(Vector3 targetPos)
    {
        animator.SetBool(IsWalkingHash, true);
        FaceTarget(targetPos);

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPos);
        }
        else
        {
            // Respaldo cinematico 2D: garantiza el movimiento fluido si el punto no tiene malla
            Vector3 nextPos = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            transform.position = new Vector3(nextPos.x, nextPos.y, 0f);
        }
    }

    private void ReturnToGuardPost()
    {
        float distanceToPost = Vector2.Distance(transform.position, initialGuardPosition);
        if (distanceToPost > 0.25f)
        {
            MoveTowardsTarget(initialGuardPosition);
        }
        else
        {
            StopMovement();
        }
    }

    private void StopMovement()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
        animator.SetBool(IsWalkingHash, false);
    }

    private void TryAttackPlayer()
    {
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackCooldown;
        animator.SetTrigger(AttackHash);

        IDamageable playerDamageable = playerTarget.GetComponent<IDamageable>();
        if (playerDamageable != null)
        {
            playerDamageable.TakeDamage(attackDamage);
        }
    }

    private void FaceTarget(Vector3 targetPos)
    {
        if (targetPos.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (targetPos.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}