using UnityEngine;

public enum ItemType
{
    Seed, RangedWeapon, Melee
}

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;

    [Header("Planting Data")]
    public PlantData plantData;

    [Header("Resulting Weapon")]
    public GameObject weaponPrefab;
}