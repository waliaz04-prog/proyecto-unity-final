using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InterfazJugador : MonoBehaviour
{
    [Header("Barra de Estamina")]
    public Slider barraEstamina;
    public TextMeshProUGUI textoEstamina;

    [Header("Pantalla de Muerte")]
    public GameObject panelMuerte;
    public TextMeshProUGUI textoMuerte;

    [Header("Interacción")]
    public TextMeshProUGUI textoInteraccion;
    public string mensajeInteraccion = "Presione E";
    public float distanciaInteraccion = 3f;
    public LayerMask capasInteractuables;

    private Camera camaraJugador;
    private IInteractable objetoActual;

    private void Start()
    {
        if (panelMuerte != null)
            panelMuerte.SetActive(false);

        if (textoInteraccion != null)
            textoInteraccion.gameObject.SetActive(false);

        camaraJugador = Camera.main;
    }

    private void Update()
    {
        // No detectar ni interactuar si el juego está pausado o muerto
        if (Time.timeScale == 0) return;

        DetectarObjetoInteractuable();

        if (objetoActual != null && Input.GetKeyDown(KeyCode.E))
            objetoActual.Interact();
    }

    public void ActualizarBarraEstamina(float porcentaje)
    {
        if (barraEstamina != null)
            barraEstamina.value = porcentaje;

        if (textoEstamina != null)
            textoEstamina.text = $"{Mathf.RoundToInt(porcentaje * 100)}%";
    }

    public void MostrarMensajeMuerte()
    {
        if (panelMuerte != null)
            panelMuerte.SetActive(true);

        if (textoMuerte != null)
            textoMuerte.text = "Estiraste la pata";

        //  Detener el tiempo (como el menú de pausa)
        Time.timeScale = 0f;

        //  Mostrar el cursor y desbloquearlo (igual que pausa)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //  Desactivar control de jugador
        PlayerMove playerMove = FindObjectOfType<PlayerMove>();
        Playerlook playerLook = FindObjectOfType<Playerlook>();

        if (playerMove != null)
            playerMove.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;

        //  Desactivar cámara del jugador (mismo efecto que pausa)
        if (camaraJugador != null)
            camaraJugador.enabled = false;
    }

    private void DetectarObjetoInteractuable()
    {
        if (camaraJugador == null) return;

        Ray rayo = new Ray(camaraJugador.transform.position, camaraJugador.transform.forward);
        if (Physics.Raycast(rayo, out RaycastHit hit, distanciaInteraccion, capasInteractuables))
        {
            IInteractable interactuable = hit.collider.GetComponent<IInteractable>();
            if (interactuable != null)
            {
                objetoActual = interactuable;
                if (textoInteraccion != null)
                {
                    textoInteraccion.text = mensajeInteraccion;
                    textoInteraccion.gameObject.SetActive(true);
                }
                return;
            }
        }

        objetoActual = null;
        if (textoInteraccion != null)
            textoInteraccion.gameObject.SetActive(false);
    }

    //  BOTONES DEL PANEL DE MUERTE
    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }
}
