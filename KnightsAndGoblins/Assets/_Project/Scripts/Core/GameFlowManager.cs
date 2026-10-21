using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Controla el flujo maestro de la partida: introduccion de la Historia del RPG,
/// condicion de derrota (Game Over), condicion de victoria (Fin de Juego al derrotar al Boss)
/// y navegacion entre escenas.
/// </summary>
public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    [Header("Paneles de Estado del Juego (UI)")]
    [Tooltip("Panel que muestra la historia inicial del juego al arrancar la partida.")]
    [SerializeField] private GameObject storyPanel;
    [Tooltip("Panel que se activa cuando la salud del jugador llega a cero.")]
    [SerializeField] private GameObject gameOverPanel;
    [Tooltip("Panel que se activa al derrotar al Jefe Final (Victoria / Fin de Juego).")]
    [SerializeField] private GameObject victoryPanel;

    [Header("Textos de Resumen Final (Opcional)")]
    [Tooltip("Texto en el panel de Victoria para mostrar el puntaje final obtenido.")]
    [SerializeField] private TextMeshProUGUI finalScoreVictoryText;
    [Tooltip("Texto en el panel de Game Over para mostrar el puntaje alcanzado.")]
    [SerializeField] private TextMeshProUGUI finalScoreGameOverText;

    [Header("Configuracion de Escenas")]
    [Tooltip("Nombre exacto de la escena del Menu Principal.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isGameEnded = false;

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
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        // Si existe el panel de historia, lo mostramos al iniciar y pausamos el mundo hasta que el jugador de clic en Comenzar
        if (storyPanel != null)
        {
            storyPanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private void Update()
    {
        // Permite cerrar la ventana de historia inicial también con la tecla Espacio o Enter
        if (storyPanel != null && storyPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                CloseStoryAndStartGame();
            }
        }
    }

    /// <summary>
    /// Cierra el pergamino de introduccion de la historia y reanuda el tiempo del juego.
    /// </summary>
    public void CloseStoryAndStartGame()
    {
        if (storyPanel != null)
        {
            storyPanel.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Activa la pantalla de Game Over cuando el jugador pierde toda su salud.
    /// </summary>
    public void TriggerGameOver()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        if (finalScoreGameOverText != null && InventoryManager.Instance != null)
        {
            finalScoreGameOverText.text = $"Puntaje alcanzado: {InventoryManager.Instance.CurrentScore}";
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    /// <summary>
    /// Activa la pantalla de Fin de Juego (Victoria) al derrotar al Jefe Final.
    /// </summary>
    public void TriggerVictory()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        if (finalScoreVictoryText != null && InventoryManager.Instance != null)
        {
            finalScoreVictoryText.text = $"¡Reino Liberado! Puntaje Final: {InventoryManager.Instance.CurrentScore}";
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    /// <summary>
    /// Reinicia la escena actual de juego (Boton Reintentar / Jugar de Nuevo).
    /// </summary>
    public void RestartGameplay()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    /// <summary>
    /// Regresa a la escena del Menu Principal.
    /// </summary>
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}