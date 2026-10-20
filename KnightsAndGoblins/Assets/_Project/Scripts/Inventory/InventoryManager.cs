using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Tipos de recursos coleccionables disponibles en los escenarios del mundo.
/// </summary>
public enum ResourceType
{
    Gold,
    Meat,
    Wood
}

/// <summary>
/// Servicio centralizado (Singleton) que administra el inventario del jugador,
/// el sistema global de puntaje y la actualizacion de la Interfaz de Usuario (HUD).
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("HUD Principal (Siempre Visible)")]
    [Tooltip("Imagen de relleno (Filled) o Slider para la barra de vida del jugador.")]
    [SerializeField] private Slider healthSlider;
    [Tooltip("Texto para mostrar los puntos de vida numericos (ej. 100/100).")]
    [SerializeField] private TextMeshProUGUI healthValueText;
    [Tooltip("Texto para mostrar el puntaje global acumulado.")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Panel de Inventario (Tecla I o Siempre Visible)")]
    [Tooltip("Panel contenedor del inventario de recursos.")]
    [SerializeField] private GameObject inventoryPanel;
    [Tooltip("Contador de bolsas de oro recolectadas.")]
    [SerializeField] private TextMeshProUGUI goldCountText;
    [Tooltip("Contador de piezas de carne recolectadas.")]
    [SerializeField] private TextMeshProUGUI meatCountText;
    [Tooltip("Contador de troncos de madera recolectados.")]
    [SerializeField] private TextMeshProUGUI woodCountText;

    private int currentScore = 0;
    private int goldCount = 0;
    private int meatCount = 0;
    private int woodCount = 0;

    private PlayerHealth playerHealth;

    public int CurrentScore => currentScore;
    public int GoldCount => goldCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }

        RefreshAllUI();
    }

    private void Update()
    {
        // Sincroniza la barra de vida del HUD con la salud actual del caballero
        UpdateHealthHUD();

        // Permite abrir/cerrar la mochila de inventario presionando la tecla I
        if (Input.GetKeyDown(KeyCode.I) && inventoryPanel != null)
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }

    /// <summary>
    /// Agrega un objeto recolectado al inventario e incrementa el puntaje global.
    /// </summary>
    public void AddItem(ResourceType type, int amount, int pointsToAward)
    {
        switch (type)
        {
            case ResourceType.Gold:
                goldCount += amount;
                break;
            case ResourceType.Meat:
                meatCount += amount;
                break;
            case ResourceType.Wood:
                woodCount += amount;
                break;
        }

        AddScore(pointsToAward);
        RefreshAllUI();
    }

    /// <summary>
    /// Incrementa el puntaje al recolectar objetos o derrotar enemigos.
    /// </summary>
    public void AddScore(int points)
    {
        currentScore += points;
        RefreshAllUI();
    }

    private void UpdateHealthHUD()
    {
        if (playerHealth == null) return;

        if (healthSlider != null)
        {
            healthSlider.maxValue = playerHealth.MaxHealth;
            healthSlider.value = playerHealth.CurrentHealth;
        }

        if (healthValueText != null)
        {
            healthValueText.text = $"HP: {playerHealth.CurrentHealth}/{playerHealth.MaxHealth}";
        }
    }

    private void RefreshAllUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Puntaje: {currentScore}";
        }

        if (goldCountText != null)
        {
            goldCountText.text = $"Oro: {goldCount}";
        }

        if (meatCountText != null)
        {
            meatCountText.text = $"Carne: {meatCount}";
        }

        if (woodCountText != null)
        {
            woodCountText.text = $"Madera: {woodCount}";
        }
    }
}