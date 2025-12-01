using UnityEngine;
using System.Collections;

public class AbrirPuertaDoble : MonoBehaviour, IInteractable
{
    [Header("Puertas a mover")]
    public Transform puertaIzquierda;
    public Transform puertaDerecha;

    [Header("Configuración del Movimiento")]
    public Vector3 direccionBase = Vector3.right;
    public float distanciaMovimiento = 2f;
    public float velocidad = 2f;

    private Vector3 puertaIzqPosInicial;
    private Vector3 puertaDerPosInicial;
    private Vector3 puertaIzqPosAbierta;
    private Vector3 puertaDerPosAbierta;

    private bool puertasAbiertas = false;
    private bool enAnimacion = false;

    [Header("Sonidos")]
    [SerializeField] private string sonidoAbrir;
    [SerializeField] private string sonidoCerrar;

    private void Start()
    {
        puertaIzqPosInicial = puertaIzquierda.position;
        puertaDerPosInicial = puertaDerecha.position;

        puertaIzqPosAbierta = puertaIzquierda.position - direccionBase.normalized * distanciaMovimiento;
        puertaDerPosAbierta = puertaDerecha.position + direccionBase.normalized * distanciaMovimiento;
    }

    public void Interact()
    {
        if (enAnimacion) return;

        puertasAbiertas = !puertasAbiertas;
        StopAllCoroutines();
        StartCoroutine(MoverPuertas(puertasAbiertas));

        if (puertasAbiertas && !string.IsNullOrEmpty(sonidoAbrir))
            AudioManager.Instance.Play(sonidoAbrir);
        else if (!puertasAbiertas && !string.IsNullOrEmpty(sonidoCerrar))
            AudioManager.Instance.Play(sonidoCerrar);
    }

    private IEnumerator MoverPuertas(bool abrir)
    {
        enAnimacion = true;

        Vector3 objetivoIzq = abrir ? puertaIzqPosAbierta : puertaIzqPosInicial;
        Vector3 objetivoDer = abrir ? puertaDerPosAbierta : puertaDerPosInicial;

        while (Vector3.Distance(puertaIzquierda.position, objetivoIzq) > 0.01f ||
               Vector3.Distance(puertaDerecha.position, objetivoDer) > 0.01f)
        {
            puertaIzquierda.position = Vector3.Lerp(puertaIzquierda.position, objetivoIzq, Time.deltaTime * velocidad);
            puertaDerecha.position = Vector3.Lerp(puertaDerecha.position, objetivoDer, Time.deltaTime * velocidad);
            yield return null;
        }

        puertaIzquierda.position = objetivoIzq;
        puertaDerecha.position = objetivoDer;

        enAnimacion = false;
    }
}
