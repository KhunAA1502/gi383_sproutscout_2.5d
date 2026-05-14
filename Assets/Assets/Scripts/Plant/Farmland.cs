using UnityEngine;

public class Farmland : MonoBehaviour
{
    [Header("Farmland Settings")]
    public bool isOccupied = false;
    public Vector2 gridPosition;
    public PlantController currentPlant;

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
        return !isOccupied && currentPlant == null;
    }

    public bool PlantSeed(ItemData seedData, PlantSpawnerUI spawner)
    {
        if (!CanPlantHere() || seedData.plantData == null) return false;

        // สร้าง plant
        GameObject plantObj = Instantiate(seedData.plantData.plantPrefab, transform.position, Quaternion.identity);
        currentPlant = plantObj.GetComponent<PlantController>();

        if (currentPlant != null)
        {
            currentPlant.Init(seedData.plantData);
            currentPlant.gridPosition = gridPosition;
            currentPlant.spawner = spawner;
            currentPlant.farmland = this; // ตั้งค่า reference กลับไปยัง farmland
            isOccupied = true;
            UpdateVisual();
            return true;
        }

        return false;
    }

    public void HarvestPlant()
    {
        if (currentPlant != null)
        {
            // เก็บผลผลิตเข้า inventory
            if (currentPlant.data.harvestItem != null)
            {
                InventoryManager.instance.AddItem(currentPlant.data.harvestItem, 1);
            }

            Destroy(currentPlant.gameObject);
            currentPlant = null;
            isOccupied = false;
            UpdateVisual();
        }
    }

    private void UpdateVisual()
    {
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