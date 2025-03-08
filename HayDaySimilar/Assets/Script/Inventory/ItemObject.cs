using UnityEngine;

public enum ItemType{
    Seed,
    Plant,
    other
};

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemObject : ScriptableObject
{
    public string Itemname;
    public int id, GrowItemId, SeedItemId, GrowTime, SellValue;
    public Sprite icon;
    public ItemType type;
}
