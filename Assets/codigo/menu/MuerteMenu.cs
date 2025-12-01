using UnityEngine;
using UnityEngine.SceneManagement;

public class MuerteMenu : MonoBehaviour
{
    [Header("UI de Muerte")]
    public GameObject panelMuerte;

    [Header("Referencias del jugador")]
    public PlayerMove playerMove;
    public Playerlook playerLook;
    public Camera camJugador;

    private bool jugadorMuerto = false;

    private void Start()
    {
        if (panelMuerte != null)
            panelMuerte.SetActive(false);
    }

    //  Este método se llama cuando el jugador muere
    public void ActivarPantallaMuerte()
    {
        jugadorMuerto = true;

        if (panelMuerte != null)
            panelMuerte.SetActive(true);

        // Detiene el tiempo (igual que pausa)
        Time.timeScale = 0f;

        // Mostrar el cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Desactivar control de jugador
        if (playerMove != null)
            playerMove.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;

        if (camJugador != null)
            camJugador.enabled = false;
    }

    //  BOTONES (igual que el menú de pausa)
    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu"); // Cambia "Menu" por el nombre real de tu escena
    }

    public void Salir()
    {
        Application.Quit();
    }
}
