using UnityEngine;

/// <summary>
/// Gestiona el seguimiento suave de la camara entre escenarios y el control de zoom panoramico.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Objetivo de Seguimiento")]
    [Tooltip("Transform del jugador al que seguira la camara a traves de los escenarios.")]
    [SerializeField] private Transform target;

    [Header("Suavizado de Movimiento")]
    [Tooltip("Tiempo de amortiguacion al desplazarse entre escenarios.")]
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Visualizacion de Escenarios (Zoom)")]
    [Tooltip("Tamaño ortografico normal durante el combate.")]
    [SerializeField] private float normalOrthoSize = 6f;
    [Tooltip("Tamaño ortografico amplio para visualizar escenarios completos (Tecla Tab).")]
    [SerializeField] private float panoramicOrthoSize = 14f;
    [SerializeField] private float zoomSpeed = 5f;

    private Camera cam;
    private Vector3 currentVelocity = Vector3.zero;
    private bool isPanoramicView = false;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        // Permite alternar una vista panoramica de los escenarios presionando Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isPanoramicView = !isPanoramicView;
        }

        float targetSize = isPanoramicView ? panoramicOrthoSize : normalOrthoSize;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);
    }

    private void LateUpdate()
    {
        // LateUpdate se ejecuta despues de que el jugador se movio, evitando vibraciones (jittering)
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);
    }
}