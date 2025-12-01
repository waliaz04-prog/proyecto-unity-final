using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Taser : MonoBehaviour
{
    [Header("Recarga")]
    [Tooltip("Tiempo (segundos) que tarda en recargarse por completo")]
    public float tiempoRecarga = 5f;

    [Header("Disparo")]
    [Tooltip("Distancia máxima del taser")]
    public float alcance = 10f;
    [Tooltip("Duración del aturdimiento en segundos")]
    public float duracionAturdimiento = 3f;
    [Tooltip("Capas que puede golpear (ej. 'Enemigos')")]
    public LayerMask capasObjetivo;

    [Header("Opciones")]
    [Tooltip("Transform desde donde se hace el raycast. Si es null usa Camera.main")]
    public Transform origenDisparo;

    [Header("Sonidos")]
    [SerializeField] private string sonidoDisparo;          // Sonido al disparar
    [SerializeField] private string sonidoImpacto;          // Sonido al golpear
    [SerializeField] private string sonidoRecargaLoop;      // Sonido mientras se recarga (loop)
    [SerializeField] private string sonidoRecargaCompleta;  // Sonido al terminar recarga

    // Estado interno
    private float cargaActual = 0f;
    private bool estaRecargando = false;

    void Start()
    {
        if (origenDisparo == null && Camera.main != null)
            origenDisparo = Camera.main.transform;

        cargaActual = tiempoRecarga; // empieza lleno
    }

    void Update()
    {
        // recarga pasiva
        if (cargaActual < tiempoRecarga)
        {
            if (!estaRecargando)
            {
                //  Inicia sonido de recarga (loop)
                if (!string.IsNullOrEmpty(sonidoRecargaLoop))
                    AudioManager.Instance.Play(sonidoRecargaLoop);
                estaRecargando = true;
            }

            cargaActual += Time.deltaTime;

            if (cargaActual >= tiempoRecarga)
            {
                cargaActual = tiempoRecarga;

                //  Detiene el loop de recarga
                if (!string.IsNullOrEmpty(sonidoRecargaLoop))
                    AudioManager.Instance.Stop(sonidoRecargaLoop);

                //  Reproduce sonido de recarga completa
                if (!string.IsNullOrEmpty(sonidoRecargaCompleta))
                    AudioManager.Instance.Play(sonidoRecargaCompleta);

                estaRecargando = false;
            }
        }

        // disparo con clic izquierdo
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            IntentarDisparar();
        }
    }

    public float GetCargaNormalized()
    {
        if (tiempoRecarga <= 0f) return 1f;
        return Mathf.Clamp01(cargaActual / tiempoRecarga);
    }

    void IntentarDisparar()
    {
        if (cargaActual < tiempoRecarga)
        {
            Debug.Log("Taser: recargando (" + GetCargaNormalized().ToString("P0") + ")");
            return;
        }

        // gastar la carga
        cargaActual = 0f;

        //  reproducir sonido de disparo
        if (!string.IsNullOrEmpty(sonidoDisparo))
            AudioManager.Instance.Play(sonidoDisparo);

        // origen y dirección
        Vector3 origen = origenDisparo != null ? origenDisparo.position : transform.position;
        Vector3 direccion = origenDisparo != null ? origenDisparo.forward : transform.forward;

        if (Physics.Raycast(origen, direccion, out RaycastHit hit, alcance, capasObjetivo.value == 0 ? ~0 : capasObjetivo))
        {
            GameObject objetivo = hit.collider.gameObject;
            Debug.Log("Taser: golpeó a " + objetivo.name);

            //  sonido de impacto
            if (!string.IsNullOrEmpty(sonidoImpacto))
                AudioManager.Instance.Play(sonidoImpacto);

            // aplicar aturdimiento
            Stunnable st = objetivo.GetComponent<Stunnable>();
            if (st != null)
            {
                st.Stun(duracionAturdimiento);
            }
            else
            {
                StartCoroutine(AturdirDirecto(objetivo, duracionAturdimiento));
            }
        }
        else
        {
            Debug.Log("Taser: no hay objetivo en alcance.");
        }
    }

    IEnumerator AturdirDirecto(GameObject objetivo, float duracion)
    {
        NavMeshAgent agent = objetivo.GetComponent<NavMeshAgent>();
        Animator animator = objetivo.GetComponent<Animator>();
        Enemigo1 enemigo1 = objetivo.GetComponent<Enemigo1>();

        if (agent != null) agent.isStopped = true;
        if (animator != null) animator.enabled = false;
        if (enemigo1 != null) enemigo1.enabled = false;

        Debug.Log($"Taser: {objetivo.name} aturdido por {duracion}s (plan B).");
        yield return new WaitForSeconds(duracion);

        if (agent != null) agent.isStopped = false;
        if (animator != null) animator.enabled = true;
        if (enemigo1 != null) enemigo1.enabled = true;

        Debug.Log($"Taser: {objetivo.name} recuperado.");
    }
}
