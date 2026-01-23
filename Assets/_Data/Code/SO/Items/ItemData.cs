using UnityEngine;

[CreateAssetMenu(menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public int id;             
    public string itemName;
    public ItemType type;

    public Sprite icon;

    
    public int expValue;       
    public int rankValue;      

    public bool stackable = true;
}
