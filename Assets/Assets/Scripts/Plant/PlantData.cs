using UnityEngine;

[CreateAssetMenu(fileName = "NewPlantData", menuName = "Farming/PlantData")]
public class PlantData : ScriptableObject 
{
    public string plantName;
    
    [Header("Crop Settings")]
    public int vegetableHealth;
    public float timeToSprout = 5f;
    public float timeToMature = 10f;
    
    [Header("Growth Models")]
    public GameObject seedModel;     // ขั้นที่ 1: เมล็ด
    public GameObject seedlingModel; // ขั้นที่ 2: ต้นอ่อน
    public GameObject matureModel;   // ขั้นที่ 3: สุก
    
    [Header("Planting")]
    public GameObject plantPrefab;   // Prefab ที่จะใช้ปลูกลงในโลก
    public ItemData harvestItem;     // ไอเทมที่จะได้รับเมื่อเก็บเกี่ยว
}