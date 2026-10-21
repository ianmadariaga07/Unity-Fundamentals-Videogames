using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Servicio centralizado (Singleton) para la reproduccion de musica de fondo, efectos de sonido (SFX)
/// y control global de volumen persistente mediante PlayerPrefs (Requisito Obligatorio de Rubrica).
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private const string VolumePrefsKey = "RPG_MasterVolume";

    [Header("Fuentes de Audio")]
    [Tooltip("AudioSource principal para la musica de fondo en bucle.")]
    [SerializeField] private AudioSource musicSource;
    [Tooltip("AudioSource secundario opcional para efectos de sonido (espada, botones, explosion).")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips de Sonido")]
    [Tooltip("Cancion medieval de fondo para el escenario.")]
    [SerializeField] private AudioClip backgroundMusic;
    [Tooltip("Sonido opcional al hacer clic o atacar.")]
    [SerializeField] private AudioClip actionSfx;

    [Header("Control de Interfaz (Opcional en Escena)")]
    [Tooltip("Slider de UI para ajustar el volumen en tiempo real.")]
    [SerializeField] private Slider volumeSlider;

    private float currentVolume = 0.8f;

    public float CurrentVolume => currentVolume;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }

        // Recupera el volumen guardado en memoria (equivalente a localStorage) o usa 0.8 por defecto
        currentVolume = PlayerPrefs.GetFloat(VolumePrefsKey, 0.8f);
        ApplyVolume(currentVolume);
    }

    private void Start()
    {
        // Vincula automaticamente el Slider de volumen si existe en la escena actual
        if (volumeSlider != null)
        {
            volumeSlider.value = currentVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // Inicia la reproduccion de musica de fondo si hay un clip asignado
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
    }

    /// <summary>
    /// Ajusta el volumen global del juego y guarda la preferencia del usuario.
    /// Este metodo puede conectarse directamente al evento OnValueChanged de un UI Slider.
    /// </summary>
    public void SetVolume(float newVolume)
    {
        currentVolume = Mathf.Clamp01(newVolume);
        ApplyVolume(currentVolume);
        PlayerPrefs.SetFloat(VolumePrefsKey, currentVolume);
        PlayerPrefs.Save();
    }

    private void ApplyVolume(float volume)
    {
        // Controla el bus maestro de Unity y las fuentes individuales
        AudioListener.volume = volume;

        if (musicSource != null)
        {
            musicSource.volume = volume;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
    }

    /// <summary>
    /// Reproduce un efecto de sonido puntual sin interrumpir la musica de fondo.
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, currentVolume);
        }
        else if (musicSource != null)
        {
            musicSource.PlayOneShot(clip, currentVolume);
        }
    }

    public void PlayDefaultActionSFX()
    {
        PlaySFX(actionSfx);
    }
}