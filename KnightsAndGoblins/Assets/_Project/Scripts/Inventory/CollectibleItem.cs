using UnityEngine;

/// <summary>
/// Representa un objeto recolectable en el mapa (Oro, Carne o Madera).
/// Al entrar en contacto con el jugador, se suma al inventario, otorga puntaje y libera su memoria.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class CollectibleItem : MonoBehaviour
{
    [Header("Configuracion del Coleccionable")]
    [Tooltip("Tipo de recurso que representa este objeto.")]
    [SerializeField] private ResourceType resourceType = ResourceType.Gold;
    [Tooltip("Cantidad de unidades que suma al inventario.")]
    [SerializeField] private int itemAmount = 1;
    [Tooltip("Puntos que otorga al marcador global al ser recolectado.")]
    [SerializeField] private int scorePoints = 25;
    [Tooltip("Puntos de vida que restaura al jugador (exclusivo para Carne).")]
    [SerializeField] private int healAmount = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // 1. Si es carne, curamos al caballero
        if (resourceType == ResourceType.Meat && healAmount > 0)
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
            }
        }

        // 2. Registramos el objeto y los puntos en el InventoryManager
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(resourceType, itemAmount, scorePoints);
        }

        // 3. Destruimos el objeto recolectado de la escena
        Destroy(gameObject);
    }
}