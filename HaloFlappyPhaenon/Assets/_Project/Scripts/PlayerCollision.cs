using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    // Callback nativo de Unity: Se dispara cuando el Rigidbody2D choca contra un Collider solido.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Cualquier choque fisico (las columnas Forerunner) detona el Game Over
        GameManager.Instance.GameOver();
    }

    // Callback nativo de Unity: Se dispara cuando atravesamos un Collider marcado como "Is Trigger".
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Por ahora solo mandamos un log. Mas adelante conectaremos esto al sistema de puntos.
        Debug.Log("Punto anotado: Trigger atravesado.");
    }
}