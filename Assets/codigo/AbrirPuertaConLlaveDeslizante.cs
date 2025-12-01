using UnityEngine;
using System.Collections;

public class AbrirPuertaConLlaveDeslizante : MonoBehaviour, IInteractable
{
    [Header("Puerta a mover")]
    public Transform puerta;

    [Header("Movimiento")]
    public Vector3 direccionMovimiento = Vector3.right;
    public float distanciaMovimiento = 2f;
    public float velocidad = 2f;

    [Header("Requisito de llave")]
    public ItemsSO llaveNecesaria;

    private PlayerInventory inventory;
    private Vector3 posicionInicial;
    private Vector3 posicionAbierta;
    private bool puertaAbierta = false;
    private bool enAnimacion = false;

    [Header("Sonidos")]
    [SerializeField] private string sonidoAbrir;
    [SerializeField] private string sonidoCerrar;
    [SerializeField] private string sonidoSinLlave;

    private void Start()
    {
        if (puerta == null)
        {
            puerta = transform;
        }

        posicionInicial = puerta.position;
        posicionAbierta = puerta.position + direccionMovimiento.normalized * distanciaMovimiento;

        inventory = FindObjectOfType<PlayerInventory>();
    }

    public void Interact()
    {
        if (enAnimacion || puerta == null) return;
        if (inventory == null) return;

        if (!inventory.inventoryList.Contains(llaveNecesaria))
        {
            if (!string.IsNullOrEmpty(sonidoSinLlave))
                AudioManager.Instance.Play(sonidoSinLlave);
            Debug.Log("No tienes la llave requerida para abrir esta puerta.");
            return;
        }

        if (inventory.objectInHand == null || inventory.objectInHand.name != llaveNecesaria.handPrefab.name)
        {
            if (!string.IsNullOrEmpty(sonidoSinLlave))
                AudioManager.Instance.Play(sonidoSinLlave);
            Debug.Log("Tienes la llave, pero no la estás sosteniendo en la mano.");
            return;
        }

        puertaAbierta = !puertaAbierta;
        StopAllCoroutines();
        StartCoroutine(MoverPuerta(puertaAbierta));

        if (puertaAbierta && !string.IsNullOrEmpty(sonidoAbrir))
            AudioManager.Instance.Play(sonidoAbrir);
        else if (!puertaAbierta && !string.IsNullOrEmpty(sonidoCerrar))
            AudioManager.Instance.Play(sonidoCerrar);
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
