using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerFarming : MonoBehaviour
{
    private ItemData currentSeedData;
    private int currentSlotIndex = -1;
    private GameObject currentSeedPreview;
    public bool IsPlantingSeed { get; private set; }

    [Header("Farming Settings")]
    public Transform holdPoint;
    public float seedPreviewScale = 0.5f;
    public LayerMask groundLayer;
    public float checkRadius = 0.5f;
    public LayerMask obstacleLayer;
    public float placementOffset = 0f;
    public float gridSize = 1f; // ต้องตรงกับ PlantSpawnerUI.gridSize

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // คลิกขวาเพื่อเก็บผัก (Harvest)
        {
            TryHarvest();
        }

        if (!IsPlantingSeed) return;

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceSeed();
        }
    }

    private void TryHarvest()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Farmland farmland = hit.collider.GetComponent<Farmland>();
            if (farmland != null)
            {
                farmland.HarvestPlant();
                Debug.Log("[PlayerFarming] คลิกขวา: เก็บผักออกจาก Farmland");
            }
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

        SpawnSeedPreview();

        Debug.Log($"[PlayerFarming] เลือกเมล็ดสำหรับปลูก: {seedData.itemName} (slot {slotIndex})");
    }

    public void CancelPlanting()
    {
        if (!IsPlantingSeed) return;

        Debug.Log("[PlayerFarming] ยกเลิกการปลูกผัก");
        IsPlantingSeed = false;
        currentSeedData = null;
        currentSlotIndex = -1;
        DestroySeedPreview();
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
        Farmland farmland = hit.collider.GetComponent<Farmland>();
        if (farmland != null && farmland.CanPlantHere())
        {
            Vector3 placePos = hit.point;
            if (farmland.PlantSeed(seedData, null, placePos))
            {
                // ปรับจากเดิมที่อาจจะสั่ง CancelPlanting() ให้มาใช้ Cleanup แทน
                PerformPlantingCleanup();
            }
        }
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

    private void SpawnSeedPreview()
    {
        DestroySeedPreview();

        if (holdPoint == null)
        {
            PlayerCombat combat = FindFirstObjectByType<PlayerCombat>();
            if (combat != null)
            {
                holdPoint = combat.spawnPoint;
            }
        }

        if (holdPoint == null || currentSeedData == null || currentSeedData.plantData == null || currentSeedData.plantData.plantPrefab == null)
        {
            return;
        }

        currentSeedPreview = Instantiate(currentSeedData.plantData.plantPrefab, holdPoint.position, holdPoint.rotation, holdPoint);
        currentSeedPreview.transform.localPosition = Vector3.zero;
        currentSeedPreview.transform.localRotation = Quaternion.identity;
        currentSeedPreview.transform.localScale = Vector3.one * seedPreviewScale;

        // ป้องกันให้ preview ไม่ชนกับอะไร
        foreach (var collider in currentSeedPreview.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
        foreach (var collider2D in currentSeedPreview.GetComponentsInChildren<Collider2D>())
        {
            collider2D.enabled = false;
        }
        foreach (var rb in currentSeedPreview.GetComponentsInChildren<Rigidbody>())
        {
            Destroy(rb);
        }
        foreach (var rb2D in currentSeedPreview.GetComponentsInChildren<Rigidbody2D>())
        {
            Destroy(rb2D);
        }
    }

    private void DestroySeedPreview()
    {
        if (currentSeedPreview != null)
        {
            Destroy(currentSeedPreview);
            currentSeedPreview = null;
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
    private void PerformPlantingCleanup()
    {
        if (currentSlotIndex != -1 && InventoryManager.instance != null)
        {
            // เข้าถึงข้อมูลช่องไอเทมใน Hotbar
            InventorySlot hotbarSlot = InventoryManager.instance.hotbarInventory[currentSlotIndex];

            if (hotbarSlot != null && hotbarSlot.item != null)
            {
                hotbarSlot.amount--; // ลดจำนวนเมล็ด

                if (hotbarSlot.amount <= 0)
                {
                    // ถ้าเมล็ดหมด
                    hotbarSlot.item = null;
                    hotbarSlot.amount = 0;
                    CancelPlanting(); // ยกเลิกโหมดปลูกและลบ Preview
                    Debug.Log("[PlayerFarming] เมล็ดหมดแล้ว เคลียร์มือ");
                }
                else
                {
                    // ถ้ายังมีเมล็ดเหลือ: ไม่ต้องเรียก CancelPlanting 
                    // เพื่อให้ IsPlantingSeed ยังเป็น true และ Preview ยังคงอยู่
                    Debug.Log($"[PlayerFarming] เมล็ดเหลือ {hotbarSlot.amount} อัน สามารถปลูกต่อได้ทันที");
                }

                // อัปเดต UI ทุกช่อง
                foreach (var slot in FindObjectsOfType<ItemSlot>())
                {
                    slot.UpdateSlotUI();
                }
            }
        }
    }
}
