using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private CharacterController charCtrl;
    private Animator animJugador;

    private float movx;
    private float movz;
    private Vector3 veloY;

    [Header("Movimiento")]
    [SerializeField] private float fuerzaSalto = 5f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float velocidadCaminar = 3f;
    [SerializeField] private float velocidadCorrer = 6f;

    private bool isGrounded;
    private Transform groundCheck;
    [SerializeField] private float radio = 0.4f;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Agacharse")]
    private float alturaOriginal;
    private Vector3 centroOriginal;
    [SerializeField] private float alturaAgachado = 1f;
    [SerializeField] private Transform camTransform;
    [SerializeField] private Transform cuerpoTransform;
    private Vector3 camPosOriginal;
    private Vector3 cuerpoPosOriginal;
    [SerializeField] private float camAgachadoOffset = -0.5f;
    [SerializeField] private float cuerpoAgachadoOffset = -0.5f;

    [SerializeField] private LayerMask capaObstaculos;
    private bool forzadoAgachado = false;

    [Header("Estamina")]
    [SerializeField] private float estaminaMax = 100f;
    [SerializeField] private float consumoPorSegundo = 15f;
    [SerializeField] private float regeneracionPorSegundo = 10f;
    private float estaminaActual;
    private bool corriendo;

    [Header("Cámara y Temblor")]
    [SerializeField] private float distanciaDeteccionEnemigo = 6f;
    [SerializeField] private float intensidadTemblor = 0.15f;
    [SerializeField] private float frecuenciaTemblor = 8f;
    [SerializeField] private LayerMask capaEnemigos;
    private float tiempoTemblor = 0f;

    [Header("Estado del jugador")]
    private bool controlesBloqueados = false;
    public bool estaMuerto { get; private set; } = false;
    private InterfazJugador interfaz;

    [Header("Sonidos")]
    [SerializeField] private string sonidoCaminar;
    [SerializeField] private string sonidoCorrer;
    [SerializeField] private string sonidoSaltar;
    [SerializeField] private string sonidoAgacharse;
    [SerializeField] private string sonidoSuspenso;
    [SerializeField] private string sonidoMuerte;

    private void Awake()
    {
        charCtrl = GetComponent<CharacterController>();
        animJugador = GetComponent<Animator>();
        groundCheck = transform.GetChild(2);

        alturaOriginal = charCtrl.height;
        centroOriginal = charCtrl.center;

        if (camTransform != null) camPosOriginal = camTransform.localPosition;
        if (cuerpoTransform != null) cuerpoPosOriginal = cuerpoTransform.localPosition;

        estaminaActual = estaminaMax;
    }

    private void Start()
    {
        interfaz = FindObjectOfType<InterfazJugador>();
        if (interfaz != null)
            interfaz.ActualizarBarraEstamina(ObtenerPorcentajeEstamina());
    }

    private void Update()
    {
        if (Time.timeScale == 0 || controlesBloqueados || estaMuerto) return;

        MovimientoJugador();
        ManejarSalto();
        ManejarAgachado();
        ManejarCorrer();
        AplicarGravedad();
        ManejarTemblorCamara();

        if (interfaz != null)
            interfaz.ActualizarBarraEstamina(ObtenerPorcentajeEstamina());
    }

    private void MovimientoJugador()
    {
        movx = Input.GetAxis("Horizontal") * Time.deltaTime;
        movz = Input.GetAxis("Vertical") * Time.deltaTime;

        float velocidadActual = corriendo ? velocidadCorrer : velocidadCaminar;
        Vector3 movimiento = transform.right * movx * velocidadActual + transform.forward * movz * velocidadActual;
        charCtrl.Move(movimiento);

        // 🔊 Sonidos de movimiento
        if (movimiento.magnitude > 0.01f && isGrounded)
        {
            if (corriendo && !string.IsNullOrEmpty(sonidoCorrer))
                AudioManager.Instance.Play(sonidoCorrer);
            else if (!corriendo && !string.IsNullOrEmpty(sonidoCaminar))
                AudioManager.Instance.Play(sonidoCaminar);
        }
        else
        {
            if (!string.IsNullOrEmpty(sonidoCaminar)) AudioManager.Instance.Stop(sonidoCaminar);
            if (!string.IsNullOrEmpty(sonidoCorrer)) AudioManager.Instance.Stop(sonidoCorrer);
        }
    }

    private void ManejarSalto()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, radio, whatIsGround);
        if (isGrounded && veloY.y < 0)
            veloY.y = -2f;

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            veloY.y = Mathf.Sqrt(fuerzaSalto * -2f * gravity);
            if (!string.IsNullOrEmpty(sonidoSaltar))
                AudioManager.Instance.Play(sonidoSaltar);
        }
    }

    private void AplicarGravedad()
    {
        veloY.y += gravity * Time.deltaTime;
        charCtrl.Move(veloY * Time.deltaTime);
    }

    private void ManejarAgachado()
    {
        bool quiereAgacharse = Input.GetKey(KeyCode.LeftControl);
        bool hayObstaculoArriba = DetectarObstaculoAlLevantarse();

        if (hayObstaculoArriba)
            forzadoAgachado = true;
        else if (!quiereAgacharse)
            forzadoAgachado = false;

        bool agachado = quiereAgacharse || forzadoAgachado;

        if (agachado)
        {
            charCtrl.height = alturaAgachado;
            charCtrl.center = new Vector3(0, alturaAgachado / 2f, 0);

            if (camTransform != null)
                camTransform.localPosition = camPosOriginal + new Vector3(0, camAgachadoOffset, 0);

            if (cuerpoTransform != null)
                cuerpoTransform.localPosition = cuerpoPosOriginal + new Vector3(0, cuerpoAgachadoOffset, 0);

            if (!string.IsNullOrEmpty(sonidoAgacharse))
                AudioManager.Instance.Play(sonidoAgacharse);
        }
        else
        {
            charCtrl.height = alturaOriginal;
            charCtrl.center = centroOriginal;

            if (camTransform != null)
                camTransform.localPosition = camPosOriginal;

            if (cuerpoTransform != null)
                cuerpoTransform.localPosition = cuerpoPosOriginal;
        }
    }

    private bool DetectarObstaculoAlLevantarse()
    {
        float radioCapsula = charCtrl.radius;
        float alturaCapsula = alturaOriginal;

        Vector3 abajo = transform.position + Vector3.up * radioCapsula;
        Vector3 arriba = transform.position + Vector3.up * (alturaCapsula - radioCapsula);

        return Physics.CheckCapsule(abajo, arriba, radioCapsula, capaObstaculos);
    }

    private void ManejarCorrer()
    {
        bool presionandoShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (presionandoShift && estaminaActual > 0 && (Input.GetAxis("Vertical") > 0 || Input.GetAxis("Horizontal") != 0))
        {
            corriendo = true;
            estaminaActual -= consumoPorSegundo * Time.deltaTime;
        }
        else
        {
            corriendo = false;
            estaminaActual += regeneracionPorSegundo * Time.deltaTime;
        }

        estaminaActual = Mathf.Clamp(estaminaActual, 0, estaminaMax);
    }

    private void ManejarTemblorCamara()
    {
        if (camTransform == null) return;

        Collider[] enemigos = Physics.OverlapSphere(transform.position, distanciaDeteccionEnemigo, capaEnemigos);

        if (enemigos.Length > 0)
        {
            tiempoTemblor += Time.deltaTime * frecuenciaTemblor;
            float shakeX = Mathf.PerlinNoise(tiempoTemblor, 0f) * 2 - 1;
            float shakeY = Mathf.PerlinNoise(0f, tiempoTemblor) * 2 - 1;
            Vector3 offset = new Vector3(shakeX, shakeY, 0f) * intensidadTemblor;
            camTransform.localPosition = camPosOriginal + offset;

            if (!string.IsNullOrEmpty(sonidoSuspenso))
                AudioManager.Instance.Play(sonidoSuspenso);
        }
        else
        {
            camTransform.localPosition = (forzadoAgachado || Input.GetKey(KeyCode.LeftControl))
                ? camPosOriginal + new Vector3(0, camAgachadoOffset, 0)
                : camPosOriginal;

            if (!string.IsNullOrEmpty(sonidoSuspenso))
                AudioManager.Instance.Stop(sonidoSuspenso);
        }
    }

    public float ObtenerPorcentajeEstamina() => estaminaActual / estaminaMax;

    public void Morir()
    {
        if (estaMuerto) return;
        estaMuerto = true;
        controlesBloqueados = true;

        if (animJugador != null)
            animJugador.SetTrigger("muerte");

        if (!string.IsNullOrEmpty(sonidoMuerte))
            AudioManager.Instance.Play(sonidoMuerte);

        if (interfaz != null)
            interfaz.MostrarMensajeMuerte();
    }
}
