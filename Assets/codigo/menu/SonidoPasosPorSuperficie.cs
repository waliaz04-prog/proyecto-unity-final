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
    public AudioClip sonidoDucto;
    public AudioClip sonidoPiso;

    AudioSource sonidoPlayer;

    private CharacterController charCtrl;
    private PlayerMove playerMove;

    public Transform detectorDePiso;


    private void Start()
    {
        charCtrl = GetComponent<CharacterController>();
        playerMove = GetComponent<PlayerMove>();
        sonidoPlayer = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // No sonar si está muerto o controles bloqueados
        if (playerMove != null && (playerMove.estaMuerto || Time.timeScale == 0))
            return;

        // Verifica si se está moviendo
        bool seMueve = (playerMove.movx != 0 || playerMove.movz != 0) && charCtrl.isGrounded;

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
    public bool ray;
    private void ReproducirSonidoSegunSuperficie()
    {
        RaycastHit hit;
        ray = Physics.Raycast(detectorDePiso.position, -detectorDePiso.up, out hit, 1f);
        
        if (ray)
        {
            int layer = hit.collider.gameObject.layer;

            // Distingue superficie
            if (((1 << layer) & capaDucto) != 0 && (playerMove.movx != 0 || playerMove.movz != 0))
            {
                // Está en un ducto
                if (sonidoPlayer.clip != null || sonidoPlayer.clip != sonidoDucto)
                {
                    sonidoPlayer.clip = sonidoDucto;
                }
                if(!sonidoPlayer.isPlaying)
                    sonidoPlayer.Play();
            }
            else if (((1 << layer) & capaPiso) != 0 && (playerMove.movx != 0 || playerMove.movz != 0))
            {
                if (sonidoPlayer.clip != null || sonidoPlayer.clip != sonidoPiso)
                {
                    sonidoPlayer.clip = sonidoPiso;
                }
                if(!sonidoPlayer.isPlaying)
                    sonidoPlayer.Play();
            }
            else if (playerMove.movx == 0 && playerMove.movz == 0)
            {
                sonidoPlayer.clip = null;
                sonidoPlayer.Stop();

            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(detectorDePiso.position, detectorDePiso.position + Vector3.down * 1f);
    }
}