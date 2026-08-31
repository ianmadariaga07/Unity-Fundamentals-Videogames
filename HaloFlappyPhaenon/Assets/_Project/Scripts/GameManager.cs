using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Implementacion del patron Singleton para acceso global estatico
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Proteccion de instancia unica
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void GameOver()
    {
        Debug.Log("Game Over: La nave ha colisionado.");

        // Time.timeScale controla el reloj interno del motor fisico. 
        // Al ponerlo en 0, congelamos todo (gravedad, movimiento, spawners).
        Time.timeScale = 0f;
    }
}