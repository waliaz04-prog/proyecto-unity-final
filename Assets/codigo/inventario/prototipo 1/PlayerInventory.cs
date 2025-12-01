using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [Header("General")]
    public List<ItemsSO> inventoryList = new List<ItemsSO>();
    public int maxCapacity = 6;

    public Transform hand;
    public Transform throwPos;
    public GameObject objectInHand;

    public int selectedItem;
    public float playerReach;
    [SerializeField] GameObject throwItem_gameobject;

    [Space(20)]
    [Header("Keys")]
    [SerializeField] KeyCode throwItemKey = KeyCode.Q;

    [Space(20)]
    [Header("UI")]
    [SerializeField] Image[] inventorySlotImage = new Image[6];
    [SerializeField] Sprite emptySlotSprite;

    [SerializeField] Camera cam;
    [SerializeField] GameObject pickUpItem_gameobject;

    [Header("Sonidos")]
    [SerializeField] private string sonidoRecoger;
    [SerializeField] private string sonidoSoltar;

    private void Update()
    {
        // Soltar objeto
        if (Input.GetKeyDown(throwItemKey) && objectInHand != null)
        {
            SoltarObjeto();
        }

        // Selección de slots con teclas 1-6
        for (int i = 1; i <= 6; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) && inventoryList.Count >= i)
            {
                selectedItem = i - 1;
                NewItemSelected();
            }
        }
    }

    // Agrega un objeto al inventario
    public void AgregarObjeto(ItemsSO item)
    {
        if (inventoryList.Count >= maxCapacity)
        {
            Debug.Log("Inventario lleno");
            return;
        }

        inventoryList.Add(item);
        ActualizarInventarioUI();

        if (!string.IsNullOrEmpty(sonidoRecoger))
            AudioManager.Instance.Play(sonidoRecoger);
    }

    // Quita un objeto y actualiza imágenes
    public void SoltarObjeto()
    {
        if (objectInHand != null)
        {
            Destroy(objectInHand);
        }

        // Instancia el objeto físico que se suelta
        Instantiate(inventoryList[selectedItem].pickPrefab, throwPos.position, throwPos.rotation);

        // Elimina del inventario
        inventoryList.RemoveAt(selectedItem);

        // Actualiza imágenes en el Canvas
        ActualizarInventarioUI();

        if (!string.IsNullOrEmpty(sonidoSoltar))
            AudioManager.Instance.Play(sonidoSoltar);

        // Si no hay más objetos, limpia la mano
        if (inventoryList.Count == 0)
        {
            objectInHand = null;
            return;
        }

        // Selecciona otro automáticamente
        selectedItem = Mathf.Clamp(selectedItem, 0, inventoryList.Count - 1);
        NewItemSelected();
    }

    // Muestra el objeto seleccionado en la mano
    private void NewItemSelected()
    {
        if (inventoryList.Count == 0) return;

        if (objectInHand != null)
            Destroy(objectInHand);

        objectInHand = Instantiate(inventoryList[selectedItem].handPrefab, hand.position, hand.rotation, hand);
        objectInHand.name = inventoryList[selectedItem].handPrefab.name;
    }

    // Actualiza imágenes del inventario en el Canvas
    private void ActualizarInventarioUI()
    {
        for (int i = 0; i < inventorySlotImage.Length; i++)
        {
            if (i < inventoryList.Count && inventoryList[i] != null)
            {
                inventorySlotImage[i].sprite = inventoryList[i].item_sprite;
                inventorySlotImage[i].color = Color.white;
            }
            else
            {
                inventorySlotImage[i].sprite = emptySlotSprite;
                inventorySlotImage[i].color = new Color(1, 1, 1, 0.3f);
            }
        }
    }
}



public interface Ipickable
{
    void PickItem();
}
