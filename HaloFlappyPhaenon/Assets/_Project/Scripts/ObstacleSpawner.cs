using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El Prefab del obstáculo (columnas) que se va a generar.")]
    [SerializeField] private GameObject obstaclePrefab;

    [Header("Configuración de Generación")]
    [Tooltip("Tiempo en segundos entre la aparición de cada columna.")]
    [SerializeField] private float spawnRate = 2f;

    [Tooltip("Límite inferior para la posición Y aleatoria.")]
    [SerializeField] private float minY = -2.5f;

    [Tooltip("Límite superior para la posición Y aleatoria.")]
    [SerializeField] private float maxY = 2.5f;

    private float timer = 0f;

    private void Update()
    {
        // Acumulamos el tiempo transcurrido en cada frame
        timer += Time.deltaTime;

        // Evaluamos si el tiempo superó nuestra tasa de generación
        if (timer >= spawnRate)
        {
            SpawnObstacle();
            timer = 0f; // Reiniciamos el temporizador
        }
    }

    private void SpawnObstacle()
    {
        // Calculamos una altura aleatoria dentro de los márgenes establecidos
        float randomY = Random.Range(minY, maxY);

        // El punto de aparición será la posición X del Spawner, pero con la Y aleatoria
        Vector3 spawnPosition = new Vector3(transform.position.x, randomY, 0f);

        // Instanciamos el Prefab en la escena
        Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
    }
}