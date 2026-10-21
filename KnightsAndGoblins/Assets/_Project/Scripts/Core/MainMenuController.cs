using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controlador de la escena de Menu de Inicio.
/// Gestiona la navegacion hacia la partida, los subpaneles de Opciones de Volumen y Creditos,
/// y el cierre de la aplicacion ejecutable (Requisito Obligatorio de Rubrica).
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Configuracion de Escenas")]
    [Tooltip("Nombre exacto de la escena principal del juego RPG.")]
    [SerializeField] private string gameplaySceneName = "Gameplay";

    [Header("Paneles del Menu (UI)")]
    [Tooltip("Panel principal con los botones Jugar, Opciones, Creditos y Salir.")]
    [SerializeField] private GameObject mainButtonsPanel;
    [Tooltip("Subpanel que contiene el Slider de configuracion de volumen.")]
    [SerializeField] private GameObject optionsPanel;
    [Tooltip("Subpanel que muestra los nombres de los integrantes del equipo y creditos.")]
    [SerializeField] private GameObject creditsPanel;

    [Header("Control de Volumen Obligatorio")]
    [Tooltip("Slider conectado al volumen maestro del juego.")]
    [SerializeField] private Slider volumeSlider;

    private const string VolumePrefsKey = "RPG_MasterVolume";

    private void Start()
    {
        // Aseguramos que el tiempo corra normal al estar en el menu
        Time.timeScale = 1f;

        ShowMainPanel();

        // Inicializa el Slider con el volumen guardado y escucha sus cambios en tiempo real
        float savedVolume = PlayerPrefs.GetFloat(VolumePrefsKey, 0.8f);
        AudioListener.volume = savedVolume;

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    /// <summary>
    /// Carga la escena principal de juego (Boton Jugar).
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    /// <summary>
    /// Muestra el panel de Opciones de Volumen y oculta los demas.
    /// </summary>
    public void OpenOptions()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    /// <summary>
    /// Muestra el panel de Creditos del equipo y oculta los demas.
    /// </summary>
    public void OpenCredits()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    /// <summary>
    /// Regresa al panel principal de botones desde Opciones o Creditos.
    /// </summary>
    public void ShowMainPanel()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    /// <summary>
    /// Actualiza el volumen global y lo persiste en disco.
    /// </summary>
    public void OnVolumeChanged(float value)
    {
        float clampedVolume = Mathf.Clamp01(value);
        AudioListener.volume = clampedVolume;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVolume(clampedVolume);
        }
        else
        {
            PlayerPrefs.SetFloat(VolumePrefsKey, clampedVolume);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Cierra la aplicacion cuando se ejecuta desde el archivo .exe compilado.
    /// </summary>
    public void QuitGame()
    {
        UnityEngine.Debug.Log("Cerrando aplicacion ejecutable...");
        UnityEngine.Application.Quit();
    }
}