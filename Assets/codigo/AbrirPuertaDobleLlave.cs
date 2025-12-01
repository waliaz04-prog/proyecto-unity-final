using UnityEngine;
using System.Collections;

public class AbrirPuertaDobleLlave : MonoBehaviour, IInteractable
{
    [Header("Puertas a mover")]
    public Transform puertaIzquierda;
    public Transform puertaDerecha;

    [Header("Movimiento")]
    public Vector3 direccionBase = Vector3.right;
    public float distanciaMovimiento = 2f;
    public float velocidad = 2f;

    [Header("Requisito de llave")]
    public ItemsSO llaveNecesaria;

    private PlayerInventory inventory;

    private Vector3 puertaIzqPosInicial;
    private Vector3 puertaDerPosInicial;
    private Vector3 puertaIzqPosAbierta;
    private Vector3 puertaDerPosAbierta;

    private bool puertasAbiertas = false;
    private bool enAnimacion = false;

    [Header("Sonidos")]
    [SerializeField] private string sonidoAbrir;
    [SerializeField] private string sonidoCerrar;
    [SerializeField] private string sonidoSinLlave;

    private void Start()
    {
        // Guardar posiciones iniciales y calcular abiertas
        puertaIzqPosInicial = puertaIzquierda.position;
        puertaDerPosInicial = puertaDerecha.position;

        puertaIzqPosAbierta = puertaIzquierda.position - direccionBase.normalized * distanciaMovimiento;
        puertaDerPosAbierta = puertaDerecha.position + direccionBase.normalized * distanciaMovimiento;

        // Buscar el inventario del jugador
        inventory = FindObjectOfType<PlayerInventory>();
    }

    public void Interact()
    {
        if (enAnimacion) return;

        if (inventory == null)
        {
            Debug.LogWarning("No se encontró PlayerInventory en la escena.");
            return;
        }

        // Verificar que el jugador tenga la llave y que esté equipada
        if (inventory.inventoryList.Contains(llaveNecesaria) && inventory.objectInHand != null)
        {
            if (inventory.objectInHand.name == llaveNecesaria.handPrefab.name)
            {
                puertasAbiertas = !puertasAbiertas;

                //  reproducir sonido de abrir o cerrar
                if (puertasAbiertas && !string.IsNullOrEmpty(sonidoAbrir))
                    AudioManager.Instance.Play(sonidoAbrir);
                else if (!puertasAbiertas && !string.IsNullOrEmpty(sonidoCerrar))
                    AudioManager.Instance.Play(sonidoCerrar);

                StopAllCoroutines();
                StartCoroutine(MoverPuertas(puertasAbiertas));
            }
            else
            {
                Debug.Log("Tienes la llave, pero no la estás sosteniendo en la mano.");
                //  sin llave en mano
                if (!string.IsNullOrEmpty(sonidoSinLlave))
                    AudioManager.Instance.Play(sonidoSinLlave);
            }
        }
        else
        {
            Debug.Log("No tienes la llave requerida.");
            //  sonido de intento sin llave
            if (!string.IsNullOrEmpty(sonidoSinLlave))
                AudioManager.Instance.Play(sonidoSinLlave);
        }
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

        // Posición final exacta
        puertaIzquierda.position = objetivoIzq;
        puertaDerecha.position = objetivoDer;

        enAnimacion = false;
    }
}
