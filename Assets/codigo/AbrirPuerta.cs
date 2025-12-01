using UnityEngine;
using System.Collections;

public class AbrirPuerta : MonoBehaviour, IInteractable
{
    [Header("Configuración")]
    public float velocidadRotacion = 2f;
    public float anguloApertura = 90f;

    private bool puertaAbierta = false;
    private Quaternion rotacionInicial;
    private Quaternion rotacionAbierta;
    private bool enAnimacion = false;

    [Header("Sonidos")]
    [SerializeField] private string sonidoAbrir;
    [SerializeField] private string sonidoCerrar;

    private void Start()
    {
        rotacionInicial = transform.rotation;
        rotacionAbierta = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + anguloApertura, transform.eulerAngles.z);
    }

    public void Interact()
    {
        if (enAnimacion) return;

        puertaAbierta = !puertaAbierta;
        StopAllCoroutines();
        StartCoroutine(RotarPuerta(puertaAbierta));

        if (puertaAbierta && !string.IsNullOrEmpty(sonidoAbrir))
            AudioManager.Instance.Play(sonidoAbrir);
        else if (!puertaAbierta && !string.IsNullOrEmpty(sonidoCerrar))
            AudioManager.Instance.Play(sonidoCerrar);
    }

    private IEnumerator RotarPuerta(bool abrir)
    {
        enAnimacion = true;
        Quaternion objetivo = abrir ? rotacionAbierta : rotacionInicial;

        while (Quaternion.Angle(transform.rotation, objetivo) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, objetivo, Time.deltaTime * velocidadRotacion);
            yield return null;
        }

        transform.rotation = objetivo;
        enAnimacion = false;
    }
}
