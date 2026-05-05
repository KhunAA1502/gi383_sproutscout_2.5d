using UnityEngine;

public enum ItemType
{
    Monster,
    Weapon,
    Crop
}

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public GameObject monsterPrefab; // Prefab ของมอนสเตอร์ผักที่จะเสกออกมาสู้
    public int healthRestore; 
    public GameObject weaponPrefab; 
}