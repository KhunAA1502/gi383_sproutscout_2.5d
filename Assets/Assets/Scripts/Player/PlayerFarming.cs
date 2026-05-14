using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerFarming : MonoBehaviour
{
    private ItemData currentSeedData;
    private int currentSlotIndex = -1;
    public bool IsPlantingSeed { get; private set; }

    [Header("Farming Settings")]
    public LayerMask groundLayer;
    public float checkRadius = 0.5f;
    public LayerMask obstacleLayer;
    public float placementOffset = 0f;
    public float gridSize = 1f; // ต้องตรงกับ PlantSpawnerUI.gridSize

    void Update()
    {
        if (!IsPlantingSeed) return;

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceSeed();
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelPlanting();
        }
    }

    public void EquipSeed(ItemData seedData, int slotIndex = -1)
    {
        if (seedData == null)
        {
            Debug.LogError("[PlayerFarming] EquipSeed เรียกด้วย seedData เป็น null");
            return;
        }

        if (seedData.itemType != ItemType.Seed)
        {
            Debug.LogWarning($"[PlayerFarming] EquipSeed ไม่สามารถใช้ไอเท็มประเภทนี้ได้: {seedData.itemType}");
            return;
        }

        currentSeedData = seedData;
        currentSlotIndex = slotIndex;
        IsPlantingSeed = true;

        Debug.Log($"[PlayerFarming] เลือกเมล็ดสำหรับปลูก: {seedData.itemName} (slot {slotIndex})");
    }

    public void CancelPlanting()
    {
        if (!IsPlantingSeed) return;

        Debug.Log("[PlayerFarming] ยกเลิกการปลูกผัก");
        IsPlantingSeed = false;
        currentSeedData = null;
        currentSlotIndex = -1;
    }

    public void TryPlaceSeedFromScreenPosition(ItemData seedData, int slotIndex, Vector2 screenPosition)
    {
        if (seedData == null)
        {
            Debug.LogError("[PlayerFarming] TryPlaceSeedFromScreenPosition เรียกด้วย seedData เป็น null");
            return;
        }

        if (seedData.itemType != ItemType.Seed)
        {
            Debug.LogWarning($"[PlayerFarming] ไม่สามารถปลูกไอเท็มประเภทนี้ได้: {seedData.itemType}");
            return;
        }

        if (Camera.main == null)
        {
            Debug.LogError("[PlayerFarming] Camera.main เป็น null");
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("[PlayerFarming] หยุดการปลูกเพราะเมาส์อยู่บน UI");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            ProcessSeedPlacement(seedData, slotIndex, hit);
        }
        else
        {
            Debug.Log($"<color=orange>Raycast ไม่โดนพื้น:</color> Ground Layer Mask = {groundLayer.value}\nลองตรวจสอบว่า Ground Layer รวม Farmland หรือไม่");
        }
    }

    private void ProcessSeedPlacement(ItemData seedData, int slotIndex, RaycastHit hit)
    {
        Debug.Log($"[PlayerFarming] Raycast พบพื้น: {hit.point}");

        if (!hit.collider.CompareTag("Farmland"))
        {
            Debug.Log("<color=yellow>วางไม่ได้: สามารถปลูกได้เฉพาะพื้นที่ Farmland เท่านั้น</color>");
            return;
        }

        // Snap ตำแหน่งให้ตรงกับ Grid
        Vector3 snappedPos = SnapToGrid(hit.point);
        Vector2 gridKey = new Vector2(snappedPos.x, snappedPos.z);

        // ตรวจสอบว่าช่องนี้ว่างหรือไม่
        if (PlantSpawnerUI.occupiedTiles.ContainsKey(gridKey) && PlantSpawnerUI.occupiedTiles[gridKey])
        {
            Debug.Log($"<color=red>[PlayerFarming] ช่องนี้มีผักแล้ว!</color> ตำแหน่ง: {gridKey}");
            return;
        }

        Vector3 placePos = new Vector3(snappedPos.x, placementOffset + 0.5f, snappedPos.z);
        Collider[] colliders = Physics.OverlapSphere(placePos, checkRadius, obstacleLayer);
        if (colliders.Length > 0)
        {
            Debug.Log("<color=red>พื้นที่ไม่ว่างสำหรับปลูกผัก!</color>");
            return;
        }

        if (seedData.plantData == null || seedData.plantData.plantPrefab == null)
        {
            Debug.LogError($"[PlayerFarming] Planting failed: plantPrefab หรือ plantData ยังไม่ได้ตั้งค่าใน ItemData ({seedData.itemName})");
            return;
        }

        Debug.Log($"[PlayerFarming] Instantiate plantPrefab: {seedData.plantData.plantPrefab.name} at {placePos}");
        GameObject newPlant = Instantiate(seedData.plantData.plantPrefab, placePos, Quaternion.identity);

        if (newPlant == null)
        {
            Debug.LogError("[PlayerFarming] Instantiate plantPrefab ล้มเหลว");
            return;
        }

        if (newPlant.TryGetComponent(out PlantController plantController))
        {
            Debug.Log($"[PlayerFarming] PlantController พบใน prefab: {newPlant.name}");
            plantController.Init(seedData.plantData);
            plantController.gridPosition = gridKey;
            
            // Mark ช่องนี้เป็น occupied
            PlantSpawnerUI.occupiedTiles[gridKey] = true;
        }
        else
        {
            Debug.LogWarning($"[PlayerFarming] Plant prefab ไม่มี PlantController component: {newPlant.name}");
        }

        Debug.Log($"<color=green>ปลูกผักสำเร็จ:</color> {seedData.itemName} ที่ตำแหน่ง {placePos}");
        DecrementHotbarSlot(slotIndex);
        CancelPlanting();
    }

    // ฟังก์ชันสำหรับ Snap ตำแหน่งให้ตรงกับ Grid
    private Vector3 SnapToGrid(Vector3 position)
    {
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float z = Mathf.Round(position.z / gridSize) * gridSize;
        return new Vector3(x, position.y, z);
    }

    private void DecrementHotbarSlot(int slotIndex)
    {
        if (InventoryManager.instance == null) return;

        if (slotIndex < 0 || slotIndex >= InventoryManager.instance.hotbarInventory.Count) return;

        InventorySlot slot = InventoryManager.instance.hotbarInventory[slotIndex];
        if (slot != null && slot.item != null)
        {
            slot.amount--;
            if (slot.amount <= 0)
            {
                slot.item = null;
                slot.amount = 0;
            }
            foreach (var itemSlot in FindObjectsOfType<ItemSlot>())
            {
                itemSlot.UpdateSlotUI();
            }
        }
    }

    private void TryPlaceSeed()
    {
        Debug.Log("[PlayerFarming] TryPlaceSeed() เรียกใช้งาน");

        if (Camera.main == null)
        {
            Debug.LogError("[PlayerFarming] Camera.main เป็น null");
            return;
        }

        if (currentSeedData == null)
        {
            Debug.LogError("[PlayerFarming] currentSeedData เป็น null");
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("[PlayerFarming] หยุดการปลูกเพราะเมาส์อยู่บน UI");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Debug.Log($"[PlayerFarming] Raycast พบพื้น: {hit.collider.name} Tag={hit.collider.tag}");
            ProcessSeedPlacement(currentSeedData, currentSlotIndex, hit);
        }
        else
        {
            Debug.Log($"<color=orange>Raycast ไม่โดนพื้น:</color> Ground Layer Mask = {groundLayer.value}\nลองตรวจสอบว่า Ground Layer รวม Farmland หรือไม่");
        }
    }
}
