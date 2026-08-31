using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Requerido para recargar la escena

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Interfaz de Usuario (UI)")]
    [Tooltip("Texto para los puntos en tiempo real.")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Tooltip("Panel que agrupa la pantalla de Game Over.")]
    [SerializeField] private GameObject gameOverPanel;

    [Tooltip("Texto para mostrar la puntuacion final en la pantalla de Game Over.")]
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Tooltip("Texto para mostrar el record historico (High Score).")]
    [SerializeField] private TextMeshProUGUI highScoreText;

    private int currentScore = 0;

    // Clave constante para almacenar el record en el disco del dispositivo
    private const string HIGHSCORE_KEY = "HighScore";

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
        // Garantiza que el juego fluya a velocidad normal al cargar la escena
        Time.timeScale = 1f;

        // Garantiza que el panel de derrota este oculto al iniciar
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void AddScore()
    {
        currentScore++;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    public void GameOver()
    {
        // 1. Congelamos el motor de fisicas
        Time.timeScale = 0f;

        // 2. Logica de persistencia de datos (Record)
        // Extraemos el record guardado. Si no existe, devuelve 0 por defecto.
        int currentHighScore = PlayerPrefs.GetInt(HIGHSCORE_KEY, 0);

        if (currentScore > currentHighScore)
        {
            currentHighScore = currentScore;
            PlayerPrefs.SetInt(HIGHSCORE_KEY, currentHighScore);
            PlayerPrefs.Save(); // Forzamos el guardado en disco duro
        }

        // 3. Mostramos la interfaz de Game Over
        if (gameOverPanel != null)
        {
            finalScoreText.text = "PUNTOS: " + currentScore.ToString();
            highScoreText.text = "RÉCORD: " + currentHighScore.ToString();
            gameOverPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Metodo publico que sera invocado por el evento OnClick del boton en el Canvas.
    /// </summary>
    public void RestartGame()
    {
        // Recarga la escena activa actual para reiniciar todo desde cero
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}