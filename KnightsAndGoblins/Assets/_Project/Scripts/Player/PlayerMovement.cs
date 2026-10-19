using UnityEngine;

/// <summary>
/// Controla el desplazamiento Top-Down en 8 direcciones y las animaciones de locomocion del jugador.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuracion de Movimiento")]
    [Tooltip("Velocidad de desplazamiento del caballero en unidades por segundo.")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private bool isFacingRight = true;

    // Hash estatico para optimizar la consulta al parametro en la maquina de estados
    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 1. Captura de entradas crudas (WASD o Flechas direccionales)
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        // Normalizamos el vector para evitar que moverse en diagonal sea 41% mas rapido (Pitágoras)
        moveInput = new Vector2(inputX, inputY).normalized;

        // 2. Actualizacion de la Maquina de Estados (Animator)
        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        animator.SetBool(IsWalkingHash, isMoving);

        // 3. Orientacion horizontal del personaje (Flip)
        if (inputX > 0 && !isFacingRight)
        {
            FlipCharacter();
        }
        else if (inputX < 0 && isFacingRight)
        {
            FlipCharacter();
        }
    }

    private void FixedUpdate()
    {
        // En Unity, los calculos del motor de fisicas se ejecutan en FixedUpdate
        rb.linearVelocity = moveInput * moveSpeed;
    }

    /// <summary>
    /// Invierte la escala en el eje X para voltear al personaje y todos sus objetos hijos (armas/hitboxes).
    /// </summary>
    private void FlipCharacter()
    {
        isFacingRight = !isFacingRight;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1f;
        transform.localScale = currentScale;
    }
}