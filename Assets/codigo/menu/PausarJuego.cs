using UnityEngine;
using UnityEngine.SceneManagement;

public class PausarJuego : MonoBehaviour
{
    public GameObject menuPauda;
    public bool juegoPausado = false;

    [Header("Referencias del jugador")]
    public PlayerMove playerMove;
    public Playerlook playerLook;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado) Reanudar();
            else Pausar();
        }

        // Cursor visible solo en pausa
        Cursor.lockState = juegoPausado ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = juegoPausado;
    }

    public void Reanudar()
    {
        menuPauda.SetActive(false);
        Time.timeScale = 1;
        juegoPausado = false;

        if (playerMove != null) playerMove.enabled = true;
        if (playerLook != null) playerLook.enabled = true;
    }

    public void Pausar()
    {
        menuPauda.SetActive(true);
        Time.timeScale = 0;
        juegoPausado = true;

        if (playerMove != null) playerMove.enabled = false;
        if (playerLook != null) playerLook.enabled = false;
    }

    public void Reiniciar()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void Salir()
    {
        Application.Quit();
    }
}
