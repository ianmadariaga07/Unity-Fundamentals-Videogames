using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Administra la vitalidad de una entidad (Enemigo o Fauna), su reaccion visual al impacto,
/// la entrega de puntaje, el sistema probabilistico de botin y efectos visuales de muerte.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Estadisticas de Vitalidad")]
    [Tooltip("Puntos de vida totales de la entidad.")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int currentHealth;

    [Header("Feedback Visual")]
    [Tooltip("Color de destello al recibir dano.")]
    [SerializeField] private Color hitColor = Color.red;
    [Tooltip("Duracion en segundos del destello rojo.")]
    [SerializeField] private float flashDuration = 0.22f;
    [Tooltip("Prefab opcional que se instancia cuando la entidad muere (ej. Explosion para el Boss).")]
    [SerializeField] private GameObject deathEffectPrefab;

    [Header("Recompensa y Botin (Loot Table)")]
    [Tooltip("Puntos que otorga al jugador al ser derrotado.")]
    [SerializeField] private int scoreValue = 10;
    [Tooltip("Probabilidad de soltar un objeto al morir (0.75 = 75% de soltar item, 25% nada).")]
    [Range(0f, 1f)]
    [SerializeField] private float dropChance = 0.75f;
    [Tooltip("Lista de Prefabs coleccionables que puede soltar (ej. Oro, Carne, Madera).")]
    [SerializeField] private GameObject[] possibleDrops;

    [Tooltip("Marca esta casilla UNICAMENTE en el Boss para disparar el Fin de Juego (Victoria) al derrotarlo.")]
    [SerializeField] private bool triggersVictoryOnDeath = false;

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

        // 1. Otorga puntaje al jugador
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddScore(scoreValue);
        }

        // 2. Desactiva colisiones y movimiento
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null && agent.enabled && agent.isOnNavMesh) agent.isStopped = true;

        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null) ai.enabled = false;

        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);

        // 3. Evalua la tabla de botin
        TrySpawnLoot();

        // 4. Instancia el efecto de muerte si tiene uno asignado (Explosion del Boss)
        if (deathEffectPrefab != null)
        {
            GameObject vfx = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, 0.8f); // Liberamos memoria despues de la animacion
        }
        // 5. Si esta entidad es el Jefe Final, dispara la pantalla de Fin de Juego (Victoria)
        if (triggersVictoryOnDeath && GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.TriggerVictory();
        }

        Destroy(gameObject);
        Destroy(gameObject);
    }

    private void TrySpawnLoot()
    {
        if (possibleDrops == null || possibleDrops.Length == 0) return;

        float randomRoll = Random.value;
        if (randomRoll <= dropChance)
        {
            int randomIndex = Random.Range(0, possibleDrops.Length);
            GameObject selectedPrefab = possibleDrops[randomIndex];

            if (selectedPrefab != null)
            {
                Instantiate(selectedPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}