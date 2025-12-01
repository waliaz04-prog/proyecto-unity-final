using UnityEngine;


// Este script es el que debe de ir en el objeto de la escena, es decir el que vas a agarrar

public class ItemPickable : MonoBehaviour, Ipickable
{
    public ItemsSO itemScriptableObject;
    public PlayerInventory inventory;

    [Header("Sonido")]
    public string sonidoPick; // Nombre del sonido asignado en el AudioManager

    public void PickItem()
    {
        if (!string.IsNullOrEmpty(sonidoPick))
            AudioManager.Instance.Play(sonidoPick);

        inventory = FindObjectOfType<PlayerInventory>();  // Si mi lista aun no llega a su capacidad maxima
        if (inventory.inventoryList.Count < inventory.maxCapacity)
        {
            inventory.AgregarObjeto(itemScriptableObject);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("No tienes espacio en el inventario");
        }
    }
}