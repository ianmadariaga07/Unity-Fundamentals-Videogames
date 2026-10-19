using UnityEngine;

/// <summary>
/// Controla la trayectoria rectilinea, colision e impacto del proyectil para el ataque a distancia.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Header("Parametros del Proyectil")]
    [Tooltip("Velocidad de vuelo de la flecha en unidades por segundo.")]
    [SerializeField] private float speed = 12f;
    [Tooltip("Puntos de dano que inflige al impactar un enemigo.")]
    [SerializeField] private int damage = 15;
    [Tooltip("Tiempo maximo de vida en segundos antes de autodestruirse para liberar memoria.")]
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Impulsa la flecha en su eje local derecho (que ya viene orientado segun hacia donde mire el Player)
        rb.linearVelocity = transform.right * speed;

        // Garbage Collection automatico si no choca con nada tras 'lifeTime' segundos
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignoramos colisiones con el propio jugador o con otros sensores Trigger
        if (collision.CompareTag("Player") || collision.isTrigger)
        {
            return;
        }

        // Polimorfismo: Si el objeto impactado implementa IDamageable, le aplicamos dano
        IDamageable damageableTarget = collision.GetComponent<IDamageable>();
        if (damageableTarget != null)
        {
            damageableTarget.TakeDamage(damage);
        }

        // Destruimos la flecha al impactar contra un enemigo o contra un obstaculo solido
        Destroy(gameObject);
    }
}