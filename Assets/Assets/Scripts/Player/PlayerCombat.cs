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

    void Update()
    {
        for (int i = 0; i < 8; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) SelectFromHotbar(i);
        }

        if (currentWeapon == null) return;

        if (Input.GetMouseButton(0)) currentWeapon.StartUse();

        if (Input.GetMouseButtonUp(0))
        {
            TryPlaceItem();
        }

        currentWeapon.Tick();
    }

    private void TryPlaceItem()
    {
        if (currentWeapon == null || Camera.main == null) return;

        if (UnityEngine.EventSystems.EventSystem.current != null && 
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            if (hit.collider.gameObject.CompareTag("Player")) 
            {
                currentWeapon.ReleaseUse();
                return;
            }

            if (CanPlaceHere(hit.collider.gameObject))
            {
                Collider[] colliders = Physics.OverlapSphere(hit.point, checkRadius, obstacleLayer);

                if (colliders.Length == 0) 
                {
                    PerformPlacement(hit.point);
                }
                else
                {
                    Debug.Log("<color=red>พื้นที่ไม่ว่าง!</color>");
                    currentWeapon.ReleaseUse();
                }
            }
            else
            {
                Debug.Log("<color=yellow>วางไม่ได้:</color> Tag พื้นไม่ถูกต้อง (" + hit.collider.tag + ") ต้องการ Dirt หรือ Platform");
                currentWeapon.ReleaseUse();
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

        if (currentSlotIndex != -1)
        {
            InventorySlot hotbarSlot = InventoryManager.instance.hotbarInventory[currentSlotIndex];
            if (hotbarSlot != null)
            {
                hotbarSlot.item = null;
                hotbarSlot.amount = 0;
            }
            foreach (var slot in FindObjectsByType<ItemSlot>(FindObjectsSortMode.None))
            {
                slot.UpdateSlotUI();
            }
        }

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