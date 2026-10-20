using UnityEngine;
using TMPro;

/// <summary>
/// Servicio centralizado (Singleton) que controla la interfaz grafica de dialogos de los NPCs.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Componentes de Interfaz (UI)")]
    [Tooltip("Panel contenedor del pergamino o caja de dialogo.")]
    [SerializeField] private GameObject dialoguePanel;
    [Tooltip("Texto para mostrar el nombre o rol del NPC.")]
    [SerializeField] private TextMeshProUGUI npcNameText;
    [Tooltip("Texto para mostrar el contenido del dialogo actual.")]
    [SerializeField] private TextMeshProUGUI dialogueBodyText;

    private string[] currentLines;
    private int currentLineIndex = 0;

    public bool IsDialogueActive => dialoguePanel != null && dialoguePanel.activeSelf;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        HideDialogue();
    }

    /// <summary>
    /// Abre el panel de dialogo e inyecta los datos del NPC activo.
    /// </summary>
    public void StartDialogue(string npcName, string[] lines)
    {
        if (lines == null || lines.Length == 0 || dialoguePanel == null) return;

        currentLines = lines;
        currentLineIndex = 0;

        if (npcNameText != null)
        {
            npcNameText.text = npcName;
        }

        dialoguePanel.SetActive(true);
        DisplayCurrentLine();
    }

    /// <summary>
    /// Avanza al siguiente parrafo del arreglo de dialogos o reinicia el ciclo.
    /// </summary>
    public void AdvanceDialogue()
    {
        if (!IsDialogueActive || currentLines == null) return;

        currentLineIndex++;

        if (currentLineIndex < currentLines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            // Al terminar todas las lineas, vuelve a la primera por si el jugador quiere releer
            currentLineIndex = 0;
            DisplayCurrentLine();
        }
    }

    private void DisplayCurrentLine()
    {
        if (dialogueBodyText != null && currentLines.Length > 0)
        {
            string pageIndicator = $" ({currentLineIndex + 1}/{currentLines.Length}) [Presiona E]";
            dialogueBodyText.text = currentLines[currentLineIndex] + pageIndicator;
        }
    }

    /// <summary>
    /// Oculta el panel de dialogo cuando el jugador se aleja del NPC.
    /// </summary>
    public void HideDialogue()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }
}