using UnityEngine;

public class Farmland : MonoBehaviour
{
    [Header("Farmland Settings")]
    public bool isOccupied = false;
    public int maxPlants = 0; // 0 = unlimited
    public Vector2 gridPosition;
    
    private System.Collections.Generic.List<PlantController> activePlants = new System.Collections.Generic.List<PlantController>();

    [Header("Visual Feedback")]
    public Material availableMaterial;
    public Material occupiedMaterial;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateVisual();
    }

    public bool CanPlantHere()
    {
        activePlants.RemoveAll(p => p == null);
        if (maxPlants <= 0) return true;
        return activePlants.Count < maxPlants;
    }

    public bool PlantSeed(ItemData seedData, PlantSpawnerUI spawner, Vector3? placementPos = null)
    {
        if (!CanPlantHere() || seedData.plantData == null) return false;

        // ใช้ตำแหน่งที่ส่งมา หรือถ้าไม่มีให้ใช้กลาง Farmland
        Vector3 spawnPosition = placementPos ?? transform.position;
        spawnPosition.y = transform.position.y + 0.1f; // ป้องกันจม

        // สร้าง plant
        GameObject plantObj = Instantiate(seedData.plantData.plantPrefab, spawnPosition, Quaternion.identity);
        PlantController newPlant = plantObj.GetComponent<PlantController>();

        if (newPlant != null)
        {
            newPlant.Init(seedData.plantData);
            newPlant.gridPosition = gridPosition;
            newPlant.spawner = spawner;
            newPlant.farmland = this; 
            
            activePlants.Add(newPlant);
            UpdateVisual();
            return true;
        }

        return false;
    }

    public void HarvestPlant(PlantController plant = null)
    {
        activePlants.RemoveAll(p => p == null);

        // ถ้าส่งเจาะจงต้นมา ให้ลบต้นนั้น
        if (plant != null)
        {
            if (plant.data.harvestItem != null)
            {
                InventoryManager.instance.AddItem(plant.data.harvestItem, 1);
            }
            activePlants.Remove(plant);
            Destroy(plant.gameObject);
        }
        // ถ้าไม่ส่งมา (เช่นคลิกขวา) ให้ลบต้นล่าสุด
        else if (activePlants.Count > 0)
        {
            PlantController lastPlant = activePlants[activePlants.Count - 1];
            if (lastPlant.data.harvestItem != null)
            {
                InventoryManager.instance.AddItem(lastPlant.data.harvestItem, 1);
            }
            Destroy(lastPlant.gameObject);
            activePlants.RemoveAt(activePlants.Count - 1);
        }
        
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        activePlants.RemoveAll(p => p == null);
        isOccupied = activePlants.Count > 0;
        
        if (meshRenderer != null)
        {
            meshRenderer.material = isOccupied ? occupiedMaterial : availableMaterial;
        }
    }

    private void OnMouseEnter()
    {
        // Highlight เมื่อเมาส์ hover (ถ้าต้องการ)
    }

    private void OnMouseExit()
    {
        // ยกเลิก highlight
    }
}