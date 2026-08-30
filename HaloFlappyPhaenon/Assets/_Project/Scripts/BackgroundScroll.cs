using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [Header("Configuracion de Movimiento")]
    [Tooltip("Velocidad a la que se desplaza el fondo hacia la izquierda.")]
    [SerializeField] private float scrollSpeed = 1.5f;
    private float spriteWidth;

    private void Awake()
    {
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        if (transform.position.x <= -spriteWidth)
        {
            transform.position += new Vector3(spriteWidth * 2f, 0, 0);
        }
    }
}