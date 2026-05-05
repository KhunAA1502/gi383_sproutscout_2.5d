using UnityEngine;

public enum ItemType
{
    Seed, RangedWeapon, Melee
}

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon; // <--- ÃÙ»àÁÅç´·Õè¨ÐâªÇìã¹ªèÍ§¡ÃÐà»ëÒ
    public ItemType itemType;

    [Header("Crop Settings")]
    public int vegetableHealth; //
    public float timeToSprout = 5f;
    public float timeToMature = 10f;

    [Header("Growth Models (ã¹©Ò¡)")]
    public GameObject seedModelPrefab;   // ÃÐÂÐ 1: ¶Ø§àÁÅç´ËÃ×Í¡Í§´Ô¹
    public GameObject sproutModelPrefab; // ÃÐÂÐ 2: µé¹ÍèÍ¹
    public GameObject matureModelPrefab; // ÃÐÂÐ 3: âµàµçÁ·Õè

    [Header("Resulting Weapon")]
    public GameObject weaponPrefab; // àÁ×èÍà¡çºáÅéÇ¨Ðä´éÍÒÇØ¸ªÔé¹¹Õé
}