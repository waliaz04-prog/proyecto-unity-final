using UnityEngine;
using UnityEngine.UI;

public class PanelAjustes : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private Slider sliderSonidos;

    [Header("UI Textos (opcional)")]
    [SerializeField] private Text textoMusica;
    [SerializeField] private Text textoSonidos;

    [Header("Claves de guardado")]
    private const string KEY_VOL_MUSICA = "VolumenMusica";
    private const string KEY_VOL_SONIDOS = "VolumenSonidos";

    private float volumenMusica = 1f;
    private float volumenSonidos = 1f;

    private void Awake()
    {
        // Cargar configuraciones guardadas
        volumenMusica = PlayerPrefs.GetFloat(KEY_VOL_MUSICA, 1f);
        volumenSonidos = PlayerPrefs.GetFloat(KEY_VOL_SONIDOS, 1f);

        // Aplicar inmediatamente
        AplicarVolumenes();

        // Configurar sliders
        if (sliderMusica != null)
        {
            sliderMusica.value = volumenMusica;
            sliderMusica.onValueChanged.AddListener(CambiarVolumenMusica);
        }

        if (sliderSonidos != null)
        {
            sliderSonidos.value = volumenSonidos;
            sliderSonidos.onValueChanged.AddListener(CambiarVolumenSonidos);
        }

        ActualizarTextos();
    }

    private void CambiarVolumenMusica(float valor)
    {
        volumenMusica = valor;
        GuardarYAplicar();
    }

    private void CambiarVolumenSonidos(float valor)
    {
        volumenSonidos = valor;
        GuardarYAplicar();
    }

    private void GuardarYAplicar()
    {
        PlayerPrefs.SetFloat(KEY_VOL_MUSICA, volumenMusica);
        PlayerPrefs.SetFloat(KEY_VOL_SONIDOS, volumenSonidos);
        PlayerPrefs.Save();
        AplicarVolumenes();
        ActualizarTextos();
    }

    private void AplicarVolumenes()
    {
        // ?? Aplica volúmenes al AudioManager
        if (AudioManager.Instance != null)
        {
            foreach (var s in AudioManager.Instance.Musica)
            {
                if (s.source != null)
                {
                    // Si el sonido es de efecto, usa volumen de efectos
                    if (s.soundefect)
                        s.source.volume = s.volume * volumenSonidos;
                    else
                        s.source.volume = s.volume * volumenMusica;
                }
            }
        }

        // También afecta el volumen global de AudioListener
        AudioListener.volume = Mathf.Max(volumenMusica, volumenSonidos);
    }

    private void ActualizarTextos()
    {
        if (textoMusica != null)
            textoMusica.text = Mathf.RoundToInt(volumenMusica * 100) + "%";

        if (textoSonidos != null)
            textoSonidos.text = Mathf.RoundToInt(volumenSonidos * 100) + "%";
    }
}
