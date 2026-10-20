using UnityEngine;

/// <summary>
/// Componente asignado a cada NPC en los escenarios.
/// Detecta la proximidad del jugador mediante un Trigger 2D y despliega sus dialogos personalizados.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class NPCDialogue : MonoBehaviour
{
    [Header("Identidad y Dialogos del NPC")]
    [Tooltip("Nombre o titulo del personaje que aparecera en el encabezado.")]
    [SerializeField] private string npcName = "Aldeano";

    [Tooltip("Lista de parrafos personalizados que dira este NPC.")]
    [TextArea(2, 4)]
    [SerializeField]
    private string[] dialogueLines = new string[]
    {
        "¡Saludos, noble caballero! Que bueno verte con vida.",
        "Los Goblins han invadido nuestras tierras. ¡Ayudanos a recuperar la paz!"
    };

    private bool isPlayerInRange = false;
    private Transform playerTransform;
    private bool isFacingRight = true;

    private void Update()
    {
        if (!isPlayerInRange) return;

        // Mira siempre hacia donde este parado el jugador
        if (playerTransform != null)
        {
            FacePlayer();
        }

        // Permite pasar a la siguiente linea de dialogo presionando la tecla E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.AdvanceDialogue();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerTransform = collision.transform;

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(npcName, dialogueLines);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerTransform = null;

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.HideDialogue();
            }
        }
    }

    private void FacePlayer()
    {
        if (playerTransform.position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (playerTransform.position.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1f;
        transform.localScale = currentScale;
    }
}