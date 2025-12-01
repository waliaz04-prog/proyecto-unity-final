using UnityEngine;
using System.Collections;

public class AbrirPuertaDeslizante : MonoBehaviour, IInteractable
{
    [Header("Puerta a mover")]
    public Transform puerta; // Asigna la puerta que se moverá

    [Header("Movimiento")]
    public Vector3 direccionMovimiento = Vector3.right; // Dirección del movimiento
    public float distanciaMovimiento = 2f;              // Qué tan lejos se moverá
    public float velocidad = 2f;                        // Velocidad del movimiento

    private Vector3 posicionInicial;
    private Vector3 posicionAbierta;
    private bool puertaAbierta = false;
    private bool enAnimacion = false;

    [Header("Sonidos")]
    [SerializeField] private string sonidoAbrir;
    [SerializeField] private string sonidoCerrar;

    private void Start()
    {
        if (puerta == null)
        {
            Debug.LogWarning("No se asignó una puerta, usando el objeto actual.");
            puerta = transform;
        }

        // Guardar posición inicial y calcular la posición abierta
        posicionInicial = puerta.position;
        posicionAbierta = puerta.position + direccionMovimiento.normalized * distanciaMovimiento;
    }

    public void Interact()
    {
        if (enAnimacion || puerta == null) return;

        puertaAbierta = !puertaAbierta;

        //  reproducir sonido según acción
        if (puertaAbierta && !string.IsNullOrEmpty(sonidoAbrir))
            AudioManager.Instance.Play(sonidoAbrir);
        else if (!puertaAbierta && !string.IsNullOrEmpty(sonidoCerrar))
            AudioManager.Instance.Play(sonidoCerrar);

        StopAllCoroutines();
        StartCoroutine(MoverPuerta(puertaAbierta));
    }

    private IEnumerator MoverPuerta(bool abrir)
    {
        enAnimacion = true;
        Vector3 objetivo = abrir ? posicionAbierta : posicionInicial;

        while (Vector3.Distance(puerta.position, objetivo) > 0.01f)
        {
            puerta.position = Vector3.Lerp(puerta.position, objetivo, Time.deltaTime * velocidad);
            yield return null;
        }

        puerta.position = objetivo;
        enAnimacion = false;
    }
}
