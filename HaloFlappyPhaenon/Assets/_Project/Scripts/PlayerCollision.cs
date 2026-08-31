using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    // Callback nativo de Unity: Se dispara cuando el Rigidbody2D choca contra un Collider solido.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Cualquier choque fisico detona el estado de derrota
        GameManager.Instance.GameOver();
    }

    // Callback nativo de Unity: Se dispara cuando atravesamos un Collider marcado como "Is Trigger".
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Invocamos el metodo del Singleton para sumar 1 punto a la UI
        GameManager.Instance.AddScore();
    }
}