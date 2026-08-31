using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    [Header("Configuracion de Desplazamiento")]
    [Tooltip("Debe coincidir con la velocidad del script BackgroundScroll para un efecto visual coherente.")]
    [SerializeField] private float speed = 1f;

    [Tooltip("Coordenada X en la que el objeto se destruira para liberar memoria RAM.")]
    [SerializeField] private float destroyXPosition = -15f;

    private void Update()
    {
        // Traslacion vectorial constante hacia la izquierda
        transform.position += Vector3.left * speed * Time.deltaTime;

        // Garbage Collection Manual (Recoleccion de basura).
        // En videojuegos no podemos dejar objetos infinitos en memoria. Si sale de la pantalla, lo destruimos.
        if (transform.position.x <= destroyXPosition)
        {
            Destroy(gameObject);
        }
    }
}