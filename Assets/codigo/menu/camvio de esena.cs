using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioDeEscenaInteractuable : MonoBehaviour, IInteractable
{
    [Header("Nombre exacto de la escena a cargar")]
    [SerializeField] private string nombreEscena;

    // --- 1. Método del sistema IInteractable ---
    public void Interact()
    {
        CambiarEscena();
    }

    // --- 2. Método opcional si quieres cambiar clickeando ---
    private void OnMouseDown()
    {
        CambiarEscena();
    }

    // --- 3. Función compartida ---
    private void CambiarEscena()
    {
        if (!string.IsNullOrEmpty(nombreEscena))
        {
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogWarning("No se ha asignado un nombre de escena en el inspector.");
        }
    }
}
