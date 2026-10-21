using UnityEngine;

/// <summary>
/// Controlador especializado para el Jefe Final (Rey Goblin).
/// Gestiona la secuencia de dialogo previo al combate (Boss Fight Intro) y efectos explosivos de ataque.
/// </summary>
[RequireComponent(typeof(EnemyAI))]
[RequireComponent(typeof(EnemyHealth))]
public class BossController : MonoBehaviour
{
    [Header("Identidad y Dialogo de Boss Fight")]
    [Tooltip("Nombre del Jefe que se mostrara en el pergamino.")]
    [SerializeField] private string bossName = "Rey Goblin Kael'Thar";

    [Tooltip("Lineas de dialogo previas al combate contra el Jefe.")]
    [TextArea(2, 4)]
    [SerializeField]
    private string[] bossFightDialogue = new string[]
    {
        "¡Ja, ja, ja! ¿Un solitario caballero osa pisar mi bastion del desierto?",
        "¡El oro de Luna Fria ahora me pertenece y tu armadura adornara mi trono!",
        "¡Preparate para volar en pedazos con mi dinamita real! [Combate Iniciado]"
    };

    [Header("Sensores de Encuentro")]
    [Tooltip("Distancia a la que se dispara el dialogo del Jefe por primera vez.")]
    [SerializeField] private float dialogueTriggerDistance = 8.5f;
    [Tooltip("Prefab opcional de explosion que se instancia al golpear al jugador.")]
    [SerializeField] private GameObject explosionEffectPrefab;

    private EnemyAI bossAI;
    private Transform playerTransform;
    private bool hasTriggeredIntro = false;
    private bool isInIntroDialogue = false;
    private int currentDialogueStep = 0;

    private void Awake()
    {
        bossAI = GetComponent<EnemyAI>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (!hasTriggeredIntro && distanceToPlayer <= dialogueTriggerDistance)
        {
            StartBossIntro();
        }

        if (isInIntroDialogue)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentDialogueStep++;
                if (currentDialogueStep < bossFightDialogue.Length)
                {
                    if (DialogueManager.Instance != null)
                    {
                        DialogueManager.Instance.AdvanceDialogue();
                    }
                }
                else
                {
                    EndBossIntroAndFight();
                }
            }
            else if (distanceToPlayer <= 4.5f || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                EndBossIntroAndFight();
            }
        }
    }

    private void StartBossIntro()
    {
        hasTriggeredIntro = true;
        isInIntroDialogue = true;
        currentDialogueStep = 0;

        if (bossAI != null)
        {
            bossAI.enabled = false;
        }

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(bossName, bossFightDialogue);
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBossFightMusic();
            AudioManager.Instance.PlayBossSFX();
        }
    }

    private void EndBossIntroAndFight()
    {
        isInIntroDialogue = false;

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.HideDialogue();
        }

        if (bossAI != null)
        {
            bossAI.enabled = true;
        }

        if (explosionEffectPrefab != null && playerTransform != null)
        {
            GameObject vfx = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, 0.8f);
        }
    }
}