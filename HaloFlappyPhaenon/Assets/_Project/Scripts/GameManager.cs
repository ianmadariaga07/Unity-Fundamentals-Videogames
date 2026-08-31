using UnityEngine;
// Libreria requerida para manipular elementos de TextMeshPro
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Interfaz de Usuario")]
    [Tooltip("Referencia al componente de texto del marcador en el Canvas.")]
    [SerializeField] private TextMeshProUGUI scoreText;

    // Encapsulamiento del estado de los puntos
    private int currentScore = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Metodo publico invocado por los Triggers (ScoreZone) para sumar puntos.
    /// </summary>
    public void AddScore()
    {
        currentScore++;
        UpdateScoreUI();
    }

    /// <summary>
    /// Sincroniza el estado logico con la vista (UI).
    /// </summary>
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over: La nave ha colisionado.");
        Time.timeScale = 0f;
    }
}