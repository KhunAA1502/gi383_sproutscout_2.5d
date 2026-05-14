using UnityEngine;

[CreateAssetMenu(menuName = "Game/Seed Database")]
public class SeedDatabase : ScriptableObject
{
    public ItemData[] seeds = new ItemData[] { };

    public ItemData GetRandomSeed()
    {
        if (seeds == null || seeds.Length == 0)
        {
            Debug.LogWarning("[SeedDatabase] ไม่มี seed ในฐานข้อมูล");
            return null;
        }
        return seeds[Random.Range(0, seeds.Length)];
    }
}
