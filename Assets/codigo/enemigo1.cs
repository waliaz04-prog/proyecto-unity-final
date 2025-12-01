using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Enemigo1 : MonoBehaviour
{
    public enum EstadoEnemigo { Patrulla, Persiguiendo, Buscando, Capturando }
    public EstadoEnemigo estadoActual = EstadoEnemigo.Patrulla;

    [Header("Componentes")]
    public Animator ani;
    private NavMeshAgent agent;
    private Transform Player;
    private PlayerMove playerScript;

    [Header("Waypoints")]
    public List<Transform> waypoints;
    private int waypointActual = 0;
    public float distanciaParada = 1.2f;

    [Header("Patrulla")]
    public float tiempoEsperaWaypoint = 3f;
    private float tiempoEsperaActual = 0f;

    [Header("Velocidades")]
    public float velocidadPatrulla = 1.5f;
    public float velocidadCaptura = 3.5f;

    [Header("Detección del jugador")]
    public float rangoVision = 10f;
    public float anguloVision = 90f;
    public float distanciaCaptura = 1.5f;

    [Header("Capas ignoradas (vidrio, reja, etc.)")]
    public LayerMask capasIgnoradas;

    [Header("Búsqueda")]
    public float radioBusqueda = 3f;
    public float tiempoBusqueda = 5f;
    private float contadorBusqueda = 0f;
    private Vector3 ultimaPosicionVista;

    private bool playerCapturado = false;
    private float velocidadPersecucion;

    [Header("Sonidos")]
    [SerializeField] private string sonidoRespiracion;
    [SerializeField] private string sonidoPasos;
    [SerializeField] private string sonidoDeteccion;
    [SerializeField] private string sonidoCaptura;
    [SerializeField] private string sonidoPerdidaJugador;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();

        GameObject objPlayer = GameObject.Find("Player");
        if (objPlayer != null)
        {
            Player = objPlayer.transform;
            playerScript = objPlayer.GetComponent<PlayerMove>();
        }

        if (waypoints.Count > 0)
            agent.SetDestination(waypoints[waypointActual].position);

        agent.stoppingDistance = distanciaParada;
        agent.speed = velocidadPatrulla;

        //  reproducir respiración ambiente
        if (!string.IsNullOrEmpty(sonidoRespiracion))
            AudioManager.Instance.Play(sonidoRespiracion);
    }

    void Update()
    {
        if (playerCapturado || Player == null) return;

        bool puedeVer = PuedeVerPlayer();

        if (puedeVer)
        {
            if (estadoActual != EstadoEnemigo.Persiguiendo)
            {
                //  sonido de detección (solo una vez)
                if (!string.IsNullOrEmpty(sonidoDeteccion))
                    AudioManager.Instance.Play(sonidoDeteccion);
            }

            ultimaPosicionVista = Player.position;
            estadoActual = EstadoEnemigo.Persiguiendo;
        }

        switch (estadoActual)
        {
            case EstadoEnemigo.Patrulla:
                Patrullar();
                break;

            case EstadoEnemigo.Persiguiendo:
                if (puedeVer)
                    IrACapturarJugador();
                else
                    IniciarBusqueda();
                break;

            case EstadoEnemigo.Buscando:
                BuscarJugador();
                break;

            case EstadoEnemigo.Capturando:
                break;
        }

        ActualizarAnimaciones();

        //  pasos cuando se mueve
        if (agent.velocity.magnitude > 0.1f && !playerCapturado)
        {
            if (!string.IsNullOrEmpty(sonidoPasos))
                AudioManager.Instance.Play(sonidoPasos);
        }
        else
        {
            if (!string.IsNullOrEmpty(sonidoPasos))
                AudioManager.Instance.Stop(sonidoPasos);
        }
    }

    bool PuedeVerPlayer()
    {
        if (Player == null) return false;

        Vector3 direccion = (Player.position - transform.position).normalized;
        float distancia = Vector3.Distance(transform.position, Player.position);

        if (distancia > rangoVision) return false;

        float angulo = Vector3.Angle(transform.forward, direccion);
        if (angulo > anguloVision / 2f) return false;

        if (Physics.Raycast(transform.position + Vector3.up, direccion, out RaycastHit hit, rangoVision, ~capasIgnoradas))
        {
            if (hit.collider.CompareTag("Player"))
                return true;
        }

        return false;
    }

    void Patrullar()
    {
        agent.speed = velocidadPatrulla;

        if (waypoints.Count == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= distanciaParada)
        {
            tiempoEsperaActual += Time.deltaTime;
            if (tiempoEsperaActual >= tiempoEsperaWaypoint)
            {
                waypointActual = (waypointActual + 1) % waypoints.Count;
                agent.SetDestination(waypoints[waypointActual].position);
                tiempoEsperaActual = 0f;
            }
        }
    }

    public float distance;

    void IrACapturarJugador()
    {
        if (Player == null) return;

        agent.isStopped = false;
        agent.speed = velocidadCaptura;
        agent.SetDestination(Player.position);

        Vector3 dir = (Player.position - transform.position);
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 5f * Time.deltaTime);
        }

        float distancia = Vector3.Distance(transform.position, Player.position);
        distance = distancia;
        if (distancia <= distanciaCaptura)
            CapturarJugador();
    }

    void CapturarJugador()
    {
        if (playerCapturado) return;

        playerCapturado = true;
        estadoActual = EstadoEnemigo.Capturando;
        agent.isStopped = true;

        if (ani != null)
            ani.SetTrigger("ataque");

        //  sonido de captura
        if (!string.IsNullOrEmpty(sonidoCaptura))
            AudioManager.Instance.Play(sonidoCaptura);

        if (playerScript != null)
            playerScript.Morir();

        // detener respiración y pasos
        if (!string.IsNullOrEmpty(sonidoRespiracion))
            AudioManager.Instance.Stop(sonidoRespiracion);
        if (!string.IsNullOrEmpty(sonidoPasos))
            AudioManager.Instance.Stop(sonidoPasos);
    }

    void IniciarBusqueda()
    {
        estadoActual = EstadoEnemigo.Buscando;
        contadorBusqueda = tiempoBusqueda;
        agent.SetDestination(ultimaPosicionVista);

        //  sonido al perder al jugador
        if (!string.IsNullOrEmpty(sonidoPerdidaJugador))
            AudioManager.Instance.Play(sonidoPerdidaJugador);
    }

    void BuscarJugador()
    {
        agent.speed = velocidadCaptura;
        if (!agent.pathPending && agent.remainingDistance <= distanciaParada)
        {
            contadorBusqueda -= Time.deltaTime;

            if (contadorBusqueda > 0)
            {
                Vector3 destino = ultimaPosicionVista + new Vector3(
                    Random.Range(-radioBusqueda, radioBusqueda),
                    0,
                    Random.Range(-radioBusqueda, radioBusqueda)
                );

                if (NavMesh.SamplePosition(destino, out NavMeshHit hit, radioBusqueda, NavMesh.AllAreas))
                    agent.SetDestination(hit.position);
            }
            else
            {
                estadoActual = EstadoEnemigo.Patrulla;
                if (waypoints.Count > 0)
                    agent.SetDestination(waypoints[waypointActual].position);
            }
        }
    }

    void ActualizarAnimaciones()
    {
        if (ani == null) return;

        bool caminando = (estadoActual == EstadoEnemigo.Patrulla && agent.velocity.magnitude > 0.1f);
        bool corriendo = (estadoActual == EstadoEnemigo.Persiguiendo);

        ani.SetBool("walk", caminando);
        ani.SetBool("run", corriendo);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoVision);

        Vector3 izquierda = Quaternion.Euler(0, -anguloVision / 2, 0) * transform.forward;
        Vector3 derecha = Quaternion.Euler(0, anguloVision / 2, 0) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + izquierda * rangoVision);
        Gizmos.DrawLine(transform.position, transform.position + derecha * rangoVision);
    }
}
