using UnityEngine;
using System.Collections;

public class AbrirCajon : MonoBehaviour, IInteractable
{
    public float velocidadMovimiento = 2f;
    public float distanciaApertura = 0.5f;
    public bool moverEnEjeX = true;

    private bool cajonAbierto = false;
    private Vector3 posicionInicial;
    private Vector3 posicionAbierta;
    private bool enAnimacion = false;

    private void Start()
    {
        posicionInicial = transform.position;
        posicionAbierta = moverEnEjeX
            ? posicionInicial + transform.right * distanciaApertura
            : posicionInicial + transform.forward * distanciaApertura;
    }

    public void Interact()
    {
        if (!enAnimacion)
        {
            cajonAbierto = !cajonAbierto;
            StopAllCoroutines();
            StartCoroutine(MoverCajon(cajonAbierto));
        }
    }

    private IEnumerator MoverCajon(bool abrir)
    {
        enAnimacion = true;
        Vector3 objetivo = abrir ? posicionAbierta : posicionInicial;

        while (Vector3.Distance(transform.position, objetivo) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, objetivo, Time.deltaTime * velocidadMovimiento);
            yield return null;
        }

        transform.position = objetivo;
        enAnimacion = false;
    }
}
