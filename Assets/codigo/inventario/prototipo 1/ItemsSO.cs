using UnityEngine;


[CreateAssetMenu(fileName = "Items", menuName = "Scriptable Objects/Items")]
public class ItemsSO : ScriptableObject
{
    [Header("Properties")]
    public GameObject pickPrefab;
    public GameObject handPrefab;
    public itemType item_type;
    public Sprite item_sprite;

}

public enum itemType { Tarjeta, llaves, Notas };