using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Servicio centralizado (Singleton) para musica de fondo (Exploracion, Boss Fight y Victoria),
/// opcion de silenciar musica (Mute), efectos de sonido (SFX) y control de volumen persistente.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private const string VolumePrefsKey = "RPG_MasterVolume";
    private const string MuteMusicPrefsKey = "RPG_MuteMusic";

    [Header("Fuentes de Audio")]
    [Tooltip("AudioSource dedicado a la musica de fondo.")]
    [SerializeField] private AudioSource musicSource;
    [Tooltip("AudioSource dedicado a efectos de sonido.")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Pistas de Musica")]
    [Tooltip("Musica de fondo principal de la escena actual (Menu o Gameplay).")]
    [SerializeField] private AudioClip backgroundMusic;
    [Tooltip("Musica intensa exclusiva para el combate contra el Jefe Final.")]
    [SerializeField] private AudioClip bossFightMusic;
    [Tooltip("Musica o fanfarria que suena al derrotar al Jefe Final y ganar la partida.")]
    [SerializeField] private AudioClip victoryMusic;

    [Header("Efectos de Sonido (SFX)")]
    [Tooltip("Sonido al hacer clic en cualquier boton de la interfaz.")]
    [SerializeField] private AudioClip buttonClickSfx;
    [Tooltip("Sonido al atacar cuerpo a cuerpo con la espada.")]
    [SerializeField] private AudioClip meleeAttackSfx;
    [Tooltip("Sonido al disparar una flecha con el arco.")]
    [SerializeField] private AudioClip bowShootSfx;
    [Tooltip("Sonido del Jefe Final (al iniciar pelea o lanzar dinamita).")]
    [SerializeField] private AudioClip bossActionSfx;

    [Header("Controles de UI en Escena (Opcional)")]
    [Tooltip("Slider de volumen de la escena actual.")]
    [SerializeField] private Slider volumeSlider;
    [Tooltip("Toggle (casilla) opcional para quitar/activar la musica.")]
    [SerializeField] private Toggle muteMusicToggle;

    private float currentVolume = 0.8f;
    private bool isMusicMuted = false;

    public float CurrentVolume => currentVolume;
    public bool IsMusicMuted => isMusicMuted;

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

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        currentVolume = PlayerPrefs.GetFloat(VolumePrefsKey, 0.8f);
        isMusicMuted = PlayerPrefs.GetInt(MuteMusicPrefsKey, 0) == 1;

        ApplyAudioSettings();
    }

    private void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(currentVolume);
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        if (muteMusicToggle != null)
        {
            muteMusicToggle.SetIsOnWithoutNotify(isMusicMuted);
            muteMusicToggle.onValueChanged.AddListener(SetMusicMuted);
        }

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

    public void SetVolume(float newVolume)
    {
        currentVolume = Mathf.Clamp01(newVolume);
        ApplyAudioSettings();
        PlayerPrefs.SetFloat(VolumePrefsKey, currentVolume);
        PlayerPrefs.Save();
    }

    public void SetMusicMuted(bool muted)
    {
        isMusicMuted = muted;
        ApplyAudioSettings();
        PlayerPrefs.SetInt(MuteMusicPrefsKey, isMusicMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleMusicMute()
    {
        SetMusicMuted(!isMusicMuted);
        PlayButtonClickSFX();
    }

    private void ApplyAudioSettings()
    {
        AudioListener.volume = currentVolume;

        if (musicSource != null)
        {
            musicSource.volume = currentVolume;
            musicSource.mute = isMusicMuted;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = currentVolume;
        }
    }

    /// <summary>
    /// Cambia dinamicamente la pista musical al tema del Jefe Final.
    /// </summary>
    public void PlayBossFightMusic()
    {
        if (bossFightMusic == null || musicSource == null) return;
        if (musicSource.clip == bossFightMusic && musicSource.isPlaying) return;

        musicSource.clip = bossFightMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    /// <summary>
    /// Detiene la musica de combate y reproduce el audio de Victoria al mostrar la pantalla final.
    /// </summary>
    public void PlayVictoryMusic()
    {
        if (victoryMusic == null || musicSource == null) return;

        musicSource.Stop();
        musicSource.clip = victoryMusic;
        musicSource.loop = false; // Suena una vez como fanfarria triunfal (pon true si es cancion en bucle)
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, currentVolume);
    }

    public void PlayButtonClickSFX() => PlaySFX(buttonClickSfx);
    public void PlayMeleeSFX() => PlaySFX(meleeAttackSfx);
    public void PlayBowSFX() => PlaySFX(bowShootSfx);
    public void PlayBossSFX() => PlaySFX(bossActionSfx);
}