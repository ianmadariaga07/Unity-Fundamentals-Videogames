using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Reproductores (Componentes inyectados)")]
    [Tooltip("Reproductor para la musica de fondo.")]
    [SerializeField] private AudioSource bgmSource;
    [Tooltip("Reproductor para los efectos de sonido.")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Archivos de Audio (Payloads)")]
    [SerializeField] private AudioClip inGameMusic;
    [SerializeField] private AudioClip gruntPartyClip;
    [SerializeField] private AudioClip gameOverClip;
    [Tooltip("Sonido corto al atravesar un obstaculo y sumar puntos.")]
    [SerializeField] private AudioClip scoreClip;

    private void Awake()
    {
        // Proteccion del Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PlayInGameMusic();
    }

    /// <summary>
    /// Inicia la musica de fondo en bucle.
    /// </summary>
    public void PlayInGameMusic()
    {
        if (bgmSource != null && inGameMusic != null)
        {
            bgmSource.clip = inGameMusic;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    /// <summary>
    /// Detiene subitamente la musica de fondo.
    /// </summary>
    public void StopMusic()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }

    /// <summary>
    /// Dispara la secuencia asincrona de Game Over.
    /// </summary>
    /// <summary>
    /// Dispara la secuencia asincrona de Game Over condicionada al record.
    /// </summary>
    /// <param name="isNewRecord">True si el jugador rompio su puntaje historico.</param>
    public void PlayDeathSequence(bool isNewRecord)
    {
        StopMusic();
        StartCoroutine(DeathSequenceRoutine(isNewRecord));
    }

    // Corrutina iteradora: Maneja la logica de audio sin bloquear el Main Thread
    private IEnumerator DeathSequenceRoutine(bool isNewRecord)
    {
        if (sfxSource != null)
        {
            if (isNewRecord && gruntPartyClip != null)
            {
                // Ruta A: Nuevo Record
                sfxSource.PlayOneShot(gruntPartyClip);
                yield return new WaitForSecondsRealtime(gruntPartyClip.length);
            }
            else if (!isNewRecord && gameOverClip != null)
            {
                // Ruta B: Muerte normal sin record
                sfxSource.PlayOneShot(gameOverClip);
                yield return new WaitForSecondsRealtime(gameOverClip.length);
            }
        }
    }

    /// <summary>
    /// Reproduce el efecto de sonido de puntuacion sin interrumpir otros efectos.
    /// </summary>
    public void PlayScoreSound()
    {
        if (sfxSource != null && scoreClip != null)
        {
            sfxSource.PlayOneShot(scoreClip);
        }
    }
}