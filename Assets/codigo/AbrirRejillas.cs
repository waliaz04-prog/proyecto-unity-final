using UnityEngine;
using System.Collections;

public class AbrirRejillas : MonoBehaviour, IInteractable
{
    [Header("Rejillas a rotar")]
    public Transform rejillaIzquierda;
    public Transform rejillaDerecha;

    [Header("Rotación")]
    public float anguloApertura = 90f;
    public float velocidad = 2f;

    [Header("Requisito de llave")]
    public ItemsSO llaveNecesaria;

    private PlayerInventory inventory;

    private Quaternion izqRotInicial;
    private Quaternion derRotInicial;
    private Quaternion izqRotAbierta;
    private Quaternion derRotAbierta;

    private bool abiertas = false;
    private bool enAnimacion = false;

    [Header("Sonidos")]
    [SerializeField] private string sonidoAbrir;
    [SerializeField] private string sonidoCerrar;
    [SerializeField] private string sonidoSinLlave;

    private void Start()
    {
        izqRotInicial = rejillaIzquierda.rotation;
        derRotInicial = rejillaDerecha.rotation;

        izqRotAbierta = Quaternion.Euler(rejillaIzquierda.eulerAngles + new Vector3(0, -anguloApertura, 0));
        derRotAbierta = Quaternion.Euler(rejillaDerecha.eulerAngles + new Vector3(0, anguloApertura, 0));

        inventory = FindObjectOfType<PlayerInventory>();
    }

    public void Interact()
    {
        if (enAnimacion) return;
        if (inventory == null) return;

        if (inventory.inventoryList.Contains(llaveNecesaria) && inventory.objectInHand != null)
        {
            if (inventory.objectInHand.name == llaveNecesaria.handPrefab.name)
            {
                abiertas = !abiertas;
                StopAllCoroutines();
                StartCoroutine(RotarRejillas(abiertas));

                if (abiertas && !string.IsNullOrEmpty(sonidoAbrir))
                    AudioManager.Instance.Play(sonidoAbrir);
                else if (!abiertas && !string.IsNullOrEmpty(sonidoCerrar))
                    AudioManager.Instance.Play(sonidoCerrar);
            }
            else
            {
                if (!string.IsNullOrEmpty(sonidoSinLlave))
                    AudioManager.Instance.Play(sonidoSinLlave);
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(sonidoSinLlave))
                AudioManager.Instance.Play(sonidoSinLlave);
        }
    }

    private IEnumerator RotarRejillas(bool abrir)
    {
        enAnimacion = true;

        Quaternion objetivoIzq = abrir ? izqRotAbierta : izqRotInicial;
        Quaternion objetivoDer = abrir ? derRotAbierta : derRotInicial;

        while (Quaternion.Angle(rejillaIzquierda.rotation, objetivoIzq) > 0.5f ||
               Quaternion.Angle(rejillaDerecha.rotation, objetivoDer) > 0.5f)
        {
            rejillaIzquierda.rotation = Quaternion.Lerp(rejillaIzquierda.rotation, objetivoIzq, Time.deltaTime * velocidad);
            rejillaDerecha.rotation = Quaternion.Lerp(rejillaDerecha.rotation, objetivoDer, Time.deltaTime * velocidad);
            yield return null;
        }

        rejillaIzquierda.rotation = objetivoIzq;
        rejillaDerecha.rotation = objetivoDer;

        enAnimacion = false;
    }
}
