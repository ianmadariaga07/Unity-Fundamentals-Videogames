using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Paneles de Interfaz")]
    [Tooltip("Asigna aqui el panel de creditos desde la jerarquia.")]
    [SerializeField] private GameObject creditsPanel;

    private void Start()
    {
        // Estado inicial: Panel de creditos inactivo
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Carga la escena del juego principal. 
    /// El indice 1 corresponde al orden en el Build Settings.
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Muestra el panel de creditos superpuesto.
    /// </summary>
    public void ShowCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Oculta el panel de creditos para volver al menu principal.
    /// </summary>
    public void HideCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Metodo publico invocado por el boton "Salir". 
    /// Cierra el ejecutable del juego.
    /// </summary>
    public void QuitGame()
    {
        // Nota Arquitectonica: Application.Quit() es ignorado dentro del Editor de Unity.
        // Solo funciona al compilar el .exe o .apk. Por ello dejamos un log para depuracion.
        Debug.Log("Cerrando aplicacion... (System.exit equivalente)");
        Application.Quit();
    }
}