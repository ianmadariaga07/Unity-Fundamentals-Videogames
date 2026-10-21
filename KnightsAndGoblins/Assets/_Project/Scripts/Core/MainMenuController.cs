using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controlador de la escena de Menu de Inicio con soporte para Opciones de Volumen,
/// Silenciado de Musica (Mute), Creditos, SFX de botones y salida al escritorio.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Configuracion de Escenas")]
    [SerializeField] private string gameplaySceneName = "Gameplay";

    [Header("Paneles del Menu (UI)")]
    [SerializeField] private GameObject mainButtonsPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Controles de Audio Obligatorios")]
    [SerializeField] private Slider volumeSlider;
    [Tooltip("Toggle (casilla) opcional en el panel de opciones para quitar la musica.")]
    [SerializeField] private Toggle muteMusicToggle;

    private const string VolumePrefsKey = "RPG_MasterVolume";
    private const string MuteMusicPrefsKey = "RPG_MuteMusic";

    private void Start()
    {
        Time.timeScale = 1f;
        ShowMainPanel(playSound: false);

        float savedVolume = PlayerPrefs.GetFloat(VolumePrefsKey, 0.8f);
        bool savedMute = PlayerPrefs.GetInt(MuteMusicPrefsKey, 0) == 1;

        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(savedVolume);
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (muteMusicToggle != null)
        {
            muteMusicToggle.SetIsOnWithoutNotify(savedMute);
            muteMusicToggle.onValueChanged.AddListener(OnMuteMusicChanged);
        }
    }

    public void PlayGame()
    {
        PlayClickSound();
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OpenOptions()
    {
        PlayClickSound();
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    public void OpenCredits()
    {
        PlayClickSound();
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    public void ShowMainPanel()
    {
        ShowMainPanel(playSound: true);
    }

    private void ShowMainPanel(bool playSound)
    {
        if (playSound) PlayClickSound();
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    public void OnVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVolume(value);
        }
    }

    public void OnMuteMusicChanged(bool isMuted)
    {
        PlayClickSound();
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicMuted(isMuted);
        }
    }

    /// <summary>
    /// Permite alternar el silencio de la musica desde un Boton normal si prefieres usar Boton en vez de Toggle.
    /// </summary>
    public void ToggleMuteMusicButton()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMusicMute();
        }
    }

    private void PlayClickSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSFX();
        }
    }

    public void QuitGame()
    {
        PlayClickSound();
        UnityEngine.Debug.Log("Cerrando aplicacion ejecutable...");
        UnityEngine.Application.Quit();

    }
}