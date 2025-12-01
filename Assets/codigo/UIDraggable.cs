using UnityEngine;
using UnityEngine.EventSystems;

public class UIDraggable : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public bool draggable = true;          // Permitir/denegar drag (controlado por ToolbarController)
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 pointerOffset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // Busca el canvas más cercano (arriba en la jerarquía)
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogWarning("UIDraggable: No se encontró Canvas padre. El drag funcionará, pero el cálculo puede fallar.");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!draggable) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out pointerOffset);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!draggable) return;
        if (rectTransform == null) return;

        Vector2 localPointerPos;
        Camera cam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera : null;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, cam, out localPointerPos))
        {
            // Convert localPointerPos (canvas space) to panel anchored position
            Vector2 newAnchoredPos = localPointerPos - pointerOffset;

            // if panel is child of Canvas and uses anchoredPosition:
            rectTransform.anchoredPosition = newAnchoredPos;
        }
    }
}
