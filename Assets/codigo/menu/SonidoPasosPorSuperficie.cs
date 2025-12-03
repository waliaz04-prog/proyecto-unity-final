using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SonidoPasosPorSuperficie : MonoBehaviour
{
    [Header("Configuración de superficies")]
    [SerializeField] private LayerMask capaPiso;   // Asigna la capa "Piso"
    [SerializeField] private LayerMask capaDucto;  // Asigna la capa "Ducto"

    [Header("Frecuencia de pasos")]
    [SerializeField] private float tiempoEntrePasos = 0.5f; // Intervalo entre sonidos
    private float contadorPasos;

    [Header("Sonidos")]
    [SerializeField] private string sonidoPiso = "pasos";      // Nombre del sonido normal
    [SerializeField] private string sonidoDucto = "pasos_ducto"; // Nombre del sonido metálico

    private CharacterController charCtrl;
    private PlayerMove playerMove;

    private void Start()
    {
        charCtrl = GetComponent<CharacterController>();
        playerMove = GetComponent<PlayerMove>();
    }

    private void Update()
    {
        // No sonar si está muerto o controles bloqueados
        if (playerMove != null && (playerMove.estaMuerto || Time.timeScale == 0))
            return;

        // Verifica si se está moviendo
        bool seMueve = charCtrl.velocity.magnitude > 0.1f && charCtrl.isGrounded;

        if (seMueve)
        {
            contadorPasos -= Time.deltaTime;
            if (contadorPasos <= 0f)
            {
                ReproducirSonidoSegunSuperficie();
                contadorPasos = tiempoEntrePasos;
            }
        }
        else
        {
            contadorPasos = 0f; // Reinicia cuando deja de moverse
        }
    }

    private void ReproducirSonidoSegunSuperficie()
    {
        RaycastHit hit;
        Vector3 origen = transform.position + Vector3.up * 0.2f;

        if (Physics.Raycast(origen, Vector3.down, out hit, 1f))
        {
            int layer = hit.collider.gameObject.layer;

            // Distingue superficie
            if (((1 << layer) & capaDucto) != 0)
            {
                // Está en un ducto
                if (!string.IsNullOrEmpty(sonidoDucto))
                    AudioManager.Instance.Play(sonidoDucto);
            }
            else if (((1 << layer) & capaPiso) != 0)
            {
                // Está en un piso normal
                if (!string.IsNullOrEmpty(sonidoPiso))
                    AudioManager.Instance.Play(sonidoPiso);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 origen = transform.position + Vector3.up * 0.2f;
        Gizmos.DrawLine(origen, origen + Vector3.down * 1f);
    }
}