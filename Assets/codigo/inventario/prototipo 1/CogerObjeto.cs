using UnityEngine;

public class CogerObjeto : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float distancia = 3f;// Alcance del raycast
    [SerializeField] private Camera cam;// Cámara del jugador

    [Header("Sonidos")]
    [SerializeField] private string sonidoRecoger;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Lanza un rayo desde el centro de la pantalla (centro de la cámara)
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(ray, out RaycastHit hit, distancia))
            {
                // Verifica si el objeto golpeado tiene el tag "objeto"
                if (hit.collider.CompareTag("objeto"))
                {
                    // Busca si el objeto tiene el componente que implementa Ipickable
                    Ipickable pickable = hit.collider.GetComponent<Ipickable>();
                    if (pickable != null)
                    {
                        if (!string.IsNullOrEmpty(sonidoRecoger))
                            AudioManager.Instance.Play(sonidoRecoger);

                        pickable.PickItem();
                    }
                }
            }
        }
    }
}





