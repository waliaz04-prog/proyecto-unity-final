using UnityEngine;

public class interactuar : MonoBehaviour
{
    public LayerMask interactuableLayers;

    [Header("Sonidos")]
    [SerializeField] private string sonidoInteractuar;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(transform.position);

        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 30, interactuableLayers))
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (!string.IsNullOrEmpty(sonidoInteractuar))
                    AudioManager.Instance.Play(sonidoInteractuar);

                hit.collider.GetComponent<IInteractable>()?.Interact();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(transform.position);
        Gizmos.DrawRay(ray.origin, ray.direction);
    }
}
