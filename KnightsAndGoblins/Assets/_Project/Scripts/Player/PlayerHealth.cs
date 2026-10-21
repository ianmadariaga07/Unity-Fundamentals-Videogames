using System.Collections;
using UnityEngine;

/// <summary>
/// Gestiona los puntos de vida del jugador, el feedback visual de dano y el estado de derrota.
/// Implementa la interfaz IDamageable para recibir ataques de cualquier tipo de enemigo.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Configuracion de Salud")]
    [Tooltip("Vida maxima del caballero al iniciar la partida.")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Feedback Visual")]
    [Tooltip("Color de destello al recibir un golpe.")]
    [SerializeField] private Color damageFlashColor = Color.red;
    [SerializeField] private float flashDuration = 0.15f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isDead = false;

    // Propiedades publicas de solo lectura para que la UI (Sprint 5) consulte la vida
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Metodo exigido por el contrato IDamageable. Reduce la salud del jugador.
    /// </summary>
    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        StartCoroutine(DamageFlashRoutine());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Permite recuperar vida al recolectar carne o pociones en el mapa.
    /// </summary>
    public void Heal(int healAmount)
    {
        if (isDead) return;
        currentHealth = Mathf.Clamp(currentHealth + healAmount, 0, maxHealth);
    }

    private IEnumerator DamageFlashRoutine()
    {
        spriteRenderer.color = damageFlashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Game Over: El heroe ha caido en combate.");

        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.TriggerGameOver();
        }

        gameObject.SetActive(false);
    }
}