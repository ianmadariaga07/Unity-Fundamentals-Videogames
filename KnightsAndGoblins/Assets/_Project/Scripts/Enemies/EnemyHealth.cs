using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Administra la vitalidad de un enemigo, su reaccion visual al impacto y su destruccion en memoria.
/// Implementa IDamageable para ser detectado por la espada y las flechas del jugador.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Estadisticas de Vitalidad")]
    [Tooltip("Puntos de vida totales del enemigo.")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int currentHealth;

    [Header("Feedback Visual")]
    [Tooltip("Color de destello al recibir dano.")]
    [SerializeField] private Color hitColor = Color.red;
    [Tooltip("Duracion en segundos del destello rojo.")]
    [SerializeField] private float flashDuration = 0.22f;

    [Header("Recompensa (Sprint 5)")]
    [Tooltip("Puntos que otorga al jugador al ser derrotado.")]
    [SerializeField] private int scoreValue = 10;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;
    private bool isDead = false;

    public int ScoreValue => scoreValue;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;

        if (currentHealth > 0)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(HitFlashRoutine());
        }
        else
        {
            StartCoroutine(DeathSequenceRoutine());
        }
    }

    private IEnumerator HitFlashRoutine()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }

    private IEnumerator DeathSequenceRoutine()
    {
        isDead = true;

        // Desactivamos colisiones e IA mientras muestra el destello rojo final
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null && agent.isOnNavMesh) agent.isStopped = true;

        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null) ai.enabled = false;

        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);

        Destroy(gameObject);
    }
}