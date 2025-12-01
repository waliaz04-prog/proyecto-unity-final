using UnityEngine;
using UnityEngine.UI;

public class ToolbarController : MonoBehaviour
{
    [Header("Referencias UI")]
    public RectTransform toolbarRect;         // el panel de la toolbar
    public Button toggleButton;               // botón para mostrar/ocultar
    public Button dockButton;                 // botón para alternar dock/undock

    [Header("Dock settings")]
    public Vector2 dockAnchoredPosition = new Vector2(0, -30f); // posición relativa al ancla cuando docked
    public Vector2 dockAnchorMin = new Vector2(0.5f, 1f);       // ejemplo: ancla superior centro
    public Vector2 dockAnchorMax = new Vector2(0.5f, 1f);

    private bool isDocked = true;
    private bool isVisible = true;
    private UIDraggable draggable;

    void Start()
    {
        if (toolbarRect == null) toolbarRect = GetComponent<RectTransform>();
        draggable = toolbarRect.GetComponent<UIDraggable>();

        if (toggleButton != null) toggleButton.onClick.AddListener(ToggleVisibility);
        if (dockButton != null) dockButton.onClick.AddListener(ToggleDock);

        // Inicializar en modo docked (anclar arriba)
        Dock();
    }

    public void ToggleVisibility()
    {
        isVisible = !isVisible;
        toolbarRect.gameObject.SetActive(isVisible);
    }

    public void ToggleDock()
    {
        if (isDocked) Undock();
        else Dock();
    }

    public void Dock()
    {
        isDocked = true;
        // Ajusta anclas para "anclar" arriba-centro (ejemplo)
        toolbarRect.anchorMin = dockAnchorMin;
        toolbarRect.anchorMax = dockAnchorMax;
        toolbarRect.pivot = new Vector2(0.5f, 1f);
        toolbarRect.anchoredPosition = dockAnchoredPosition;
        if (draggable != null) draggable.draggable = false;
        UpdateDockButtonLabel();
    }

    public void Undock()
    {
        isDocked = false;
        // Pon anclas a esquina superior izquierda del canvas para permitir mover libremente en pixel space
        // (otra opción: mantener las anclas en 0-1, pero aquí simplificamos)
        toolbarRect.anchorMin = new Vector2(0f, 1f);
        toolbarRect.anchorMax = new Vector2(0f, 1f);
        toolbarRect.pivot = new Vector2(0f, 1f);
        // conservar la posición actual en pantalla: convertimos la posición global a anchored
        Vector2 currentAnchored = toolbarRect.anchoredPosition;
        toolbarRect.anchoredPosition = currentAnchored;
        if (draggable != null) draggable.draggable = true;
        UpdateDockButtonLabel();
    }

    void UpdateDockButtonLabel()
    {
        if (dockButton == null) return;
        Text t = dockButton.GetComponentInChildren<Text>();
        if (t != null) t.text = isDocked ? "Undock" : "Dock";
    }

    // Opcional: permite anclar de nuevo programáticamente con un botón
    public void SnapBackToDock()
    {
        Dock();
    }
}
