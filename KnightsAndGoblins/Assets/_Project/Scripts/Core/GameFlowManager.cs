using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controla el flujo maestro de la partida: Historia inicial, Menu de Pausa con opciones de audio,
/// pantalla de Game Over, pantalla de Victoria y retorno al Menu Principal.
/// </summary>
public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    [Header("Paneles de Estado del Juego (UI)")]
    [SerializeField] private GameObject storyPanel;
    [Tooltip("Panel del Menu de Pausa durante la partida.")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;

    [Header("Controles de Audio en el Menu de Pausa")]
    [Tooltip("Slider de volumen ubicado dentro del PausePanel.")]
    [SerializeField] private Slider pauseVolumeSlider;
    [Tooltip("Toggle (casilla) opcional dentro del PausePanel para quitar la musica.")]
    [SerializeField] private Toggle pauseMuteToggle;

    [Header("Textos de Resumen Final (Opcional)")]
    [SerializeField] private TextMeshProUGUI finalScoreVictoryText;
    [SerializeField] private TextMeshProUGUI finalScoreGameOverText;

    [Header("Configuracion de Escenas")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isGameEnded = false;
    private bool isPaused = false;

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
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        if (storyPanel != null)
        {
            storyPanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        // Vincula los controles de audio del menu de pausa con el AudioManager
        if (AudioManager.Instance != null)
        {
            if (pauseVolumeSlider != null)
            {
                pauseVolumeSlider.SetValueWithoutNotify(AudioManager.Instance.CurrentVolume);
                pauseVolumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetVolume);
            }

            if (pauseMuteToggle != null)
            {
                pauseMuteToggle.SetIsOnWithoutNotify(AudioManager.Instance.IsMusicMuted);
                pauseMuteToggle.onValueChanged.AddListener(AudioManager.Instance.SetMusicMuted);
            }
        }
    }

    private void Update()
    {
        if (storyPanel != null && storyPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                CloseStoryAndStartGame();
            }
            return;
        }

        // Permite pausar o reanudar con la tecla Escape si la partida sigue en curso
        if (!isGameEnded && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void CloseStoryAndStartGame()
    {
        PlayClickSound();
        if (storyPanel != null)
        {
            storyPanel.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Abre o cierra el Menu de Pausa (conectado al Boton de Pausa del HUD y a la tecla ESC).
    /// </summary>
    public void TogglePauseMenu()
    {
        if (isGameEnded) return;
        if (storyPanel != null && storyPanel.activeSelf) return;

        isPaused = !isPaused;
        PlayClickSound();

        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }

        // Sincroniza visualmente el slider/toggle al abrir la pausa
        if (isPaused && AudioManager.Instance != null)
        {
            if (pauseVolumeSlider != null)
                pauseVolumeSlider.SetValueWithoutNotify(AudioManager.Instance.CurrentVolume);
            if (pauseMuteToggle != null)
                pauseMuteToggle.SetIsOnWithoutNotify(AudioManager.Instance.IsMusicMuted);
        }

        Time.timeScale = isPaused ? 0f : 1f;
    }

    /// <summary>
    /// Boton Reanudar dentro del Menu de Pausa.
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;
        PlayClickSound();

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Permite silenciar/activar la musica desde un Boton dentro del Menu de Pausa.
    /// </summary>
    public void ToggleMuteInPause()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMusicMute();
        }
    }

    public void TriggerGameOver()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        if (pausePanel != null) pausePanel.SetActive(false);

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

    public void TriggerVictory()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        if (pausePanel != null) pausePanel.SetActive(false);

        // Reproduce el nuevo audio de victoria al vencer al Boss
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayVictoryMusic();
        }

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

    public void RestartGameplay()
    {
        PlayClickSound();
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void ReturnToMainMenu()
    {
        PlayClickSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void PlayClickSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSFX();
        }
    }
}