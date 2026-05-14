using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Weapon currentWeapon;
    private ItemData currentItemData;
    private int currentSlotIndex = -1; 
    public Transform spawnPoint;
    public LayerMask groundLayer;

    [Header("Placement Settings")]
    public float checkRadius = 0.5f; 
    public LayerMask obstacleLayer; 
    public float placementOffset = 0f; 

    private void OnEnable()
    {
        HotbarController.OnHotbarSlotSelected += HandleHotbarSelection;
    }

    private void OnDisable()
    {
        HotbarController.OnHotbarSlotSelected -= HandleHotbarSelection;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // คลิกขวาเพื่อลบอาวุธ
        {
            TryRemoveItem();
        }

        if (currentWeapon == null) return;

        if (Input.GetMouseButton(0)) currentWeapon.StartUse();

        if (Input.GetMouseButtonUp(0))
        {
            TryPlaceItem();
        }

        currentWeapon.Tick();
    }

    private void TryRemoveItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            WeaponPlatform platform = hit.collider.GetComponent<WeaponPlatform>();
            if (platform != null)
            {
                platform.RemoveWeapon();
                Debug.Log("[PlayerCombat] คลิกขวา: ลบอาวุธออกจาก Platform");
            }
        }
    }

    private void HandleHotbarSelection(int slotIndex, ItemSlot slot)
    {
        // HotbarController จัดการการเลือกและ equip แล้ว ไม่ต้องทำอะไรเพิ่ม
    }

    private void TryPlaceItem()
    {
        if (currentWeapon == null || Camera.main == null) return;

        Debug.Log($"[PlayerCombat] TryPlaceItem() - currentItemData={currentItemData?.itemName ?? "null"}");

        if (UnityEngine.EventSystems.EventSystem.current != null && 
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("[PlayerCombat] หยุด: เมาส์อยู่บน UI");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.Log($"[PlayerCombat] Raycast: groundLayer={groundLayer.value}");

        if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            Debug.Log($"[PlayerCombat] Raycast พบ: {hit.collider.name} | Tag: {hit.collider.tag} | Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

            if (hit.collider.gameObject.CompareTag("Player")) 
            {
                Debug.Log("[PlayerCombat] ชนกับ Player ยกเลิก");
                currentWeapon.ReleaseUse();
                return;
            }

            // ตรวจสอบชนิดไอเท็มและพื้นที่
            if (currentItemData.itemType == ItemType.RangedWeapon)
            {
                Debug.Log("[PlayerCombat] เป็น RangedWeapon ค้นหา WeaponPlatform");

                // วางอาวุธระยะไกลบน Platform
                WeaponPlatform platform = hit.collider.GetComponent<WeaponPlatform>();
                if (platform == null)
                {
                    Debug.Log($"<color=yellow>[PlayerCombat] WeaponPlatform is NULL บน {hit.collider.name}</color>");
                    currentWeapon.ReleaseUse();
                    return;
                }

                Debug.Log("[PlayerCombat] พบ WeaponPlatform");

                if (!platform.CanPlaceWeaponHere())
                {
                    Debug.Log("<color=red>[PlayerCombat] Platform มีอาวุธอยู่แล้ว!</color>");
                    currentWeapon.ReleaseUse();
                    return;
                }

                Debug.Log("[PlayerCombat] Platform ว่าง เรียก PlaceWeapon()");

                // วางอาวุธผ่าน Platform script โดยส่งตำแหน่งที่คลิกไปด้วย
                if (platform.PlaceWeapon(currentItemData, this, hit.point))
                {
                    Debug.Log($"<color=white>PLACEMENT:</color> วางอาวุธสำเร็จ: {currentItemData.itemName}");
                    PerformPlacementCleanup();
                }
                else
                {
                    Debug.LogError("[PlayerCombat] PlaceWeapon() return false");
                    currentWeapon.ReleaseUse();
                }
            }
            else if (currentItemData.itemType == ItemType.Melee)
            {
                Debug.Log("<color=yellow>[PlayerCombat] อาวุธระยะใกล้ ไม่สามารถวางได้</color>");
                currentWeapon.ReleaseUse();
            }
            else
            {
                // กรณีอื่นๆ (ถ้ามี)
                PerformPlacement(hit.point);
            }
        }
        else 
        {
            Debug.Log("<color=orange>Raycast ไม่โดนพื้น:</color> คลิกไม่โดน Layer ที่กำหนด");
            currentWeapon.ReleaseUse();
        }
    }

    private bool CanPlaceHere(GameObject groundObject)
    {
        if (currentItemData == null) return false;
        if (currentItemData.itemType == ItemType.Seed) return groundObject.CompareTag("Dirt");
        if (currentItemData.itemType == ItemType.RangedWeapon) return groundObject.CompareTag("Platform");
        // Melee ไม่สามารถวางได้
        if (currentItemData.itemType == ItemType.Melee) return false;
        return true;
    }

    private void PerformPlacement(Vector3 spawnPos)
    {
        if (currentWeapon == null) return;

        Debug.Log($"<color=white>PLACEMENT:</color> กำลังวางวัตถุชื่อ: <b>{currentWeapon.gameObject.name}</b>");

        currentWeapon.ReleaseUse();
        currentWeapon.transform.SetParent(null);
        
        Vector3 finalPos = spawnPos + new Vector3(0, placementOffset, 0);
        currentWeapon.transform.position = finalPos;
        currentWeapon.transform.rotation = Quaternion.identity;

        if (currentWeapon.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (currentWeapon is Bean beanSentry)
        {
            int health = (currentItemData != null && currentItemData.plantData != null)
                ? currentItemData.plantData.vegetableHealth
                : 3;
            beanSentry.SetupSentry(health);
        }

        currentWeapon.ActivateAutoFire();
    }

    private void PerformPlacementCleanup()
    {
        // ลดจำนวนไอเท็ม 1 ชิ้น
        if (currentSlotIndex != -1 && InventoryManager.instance != null)
        {
            InventorySlot hotbarSlot = InventoryManager.instance.hotbarInventory[currentSlotIndex];
            if (hotbarSlot != null && hotbarSlot.item != null)
            {
                hotbarSlot.amount--;
                Debug.Log($"[PlayerCombat] ลดไอเท็ม: {hotbarSlot.item.itemName} เหลือ {hotbarSlot.amount} ชิ้น");

                // ถ้าหมดแล้ว ลบออก
                if (hotbarSlot.amount <= 0)
                {
                    hotbarSlot.item = null;
                    hotbarSlot.amount = 0;
                    Debug.Log("[PlayerCombat] ไอเท็มหมดแล้ว ลบออกจาก inventory");
                }

                // อัปเดต UI
                foreach (var slot in FindObjectsOfType<ItemSlot>())
                {
                    slot.UpdateSlotUI();
                }
            }
        }

        // Reset weapon state
        currentWeapon = null;
        currentItemData = null;
        currentSlotIndex = -1;
    }
    

    private void SelectFromHotbar(int index)
    {
        InventorySlot selectedSlot = InventoryManager.instance.hotbarInventory[index];
        ItemData selectedItem = selectedSlot != null ? selectedSlot.item : null;
        if (selectedItem != null) EquipItem(selectedItem, index);
    }

    public void EquipItem(ItemData item, int index = -1)
    {
        if (item == null || item.weaponPrefab == null) return;
        
        if (item.weaponPrefab.name.ToLower().Contains("preview"))
        {
            Debug.LogError($"STOP! ItemData '{item.itemName}' uses a Preview prefab!");
            return;
        }

        currentSlotIndex = index;
        foreach (Transform child in spawnPoint) { Destroy(child.gameObject); }

        currentItemData = item;
        GameObject weaponObj = Instantiate(item.weaponPrefab, spawnPoint);
        currentWeapon = weaponObj.GetComponent<Weapon>();

        // ปรับขนาด weapon preview ให้เล็กลง
        weaponObj.transform.localScale = Vector3.one * 0.6f;

        if (currentWeapon == null)
        {
            Debug.LogError($"Error: Prefab '{weaponObj.name}' missing Weapon/Bean script!");
        }
        else
        {
            Debug.Log($"<color=cyan>EQUIPPED:</color> {weaponObj.name}");
        }
    }
}