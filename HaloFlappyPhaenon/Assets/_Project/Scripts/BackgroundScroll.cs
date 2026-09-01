using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Tooltip("Velocidad a la que se mueve el fondo hacia la izquierda.")]
    [SerializeField] private float scrollSpeed = 2f;

    [Tooltip("Micro-ajuste para solapar los bordes y eliminar la raya de renderizado (seam).")]
    [SerializeField] private float overlapTolerance = 0.05f;

    // Atributo encapsulado para almacenar la medida dinamica del sprite
    private float spriteWidth;

    private void Start()
    {
        // Inyeccion de dependencia nativa: extraemos el componente renderer
        // bounds.size.x nos da la anchura total en unidades de Unity
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        // Desplazamiento usando el motor de Unity (independiente de los FPS)
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        // Limite absoluto: Si la posicion X rebasa por completo el ancho negativo, se reposiciona.
        if (transform.position.x <= -spriteWidth)
        {
            // Sumamos el doble del ancho, pero restamos la tolerancia de solapamiento.
            // Esto obliga a los fondos a cruzarse levemente, matando la costura visual.
            float resetPosition = (spriteWidth * 2f) - overlapTolerance;
            transform.position += new Vector3(resetPosition, 0, 0);
        }
    }
}