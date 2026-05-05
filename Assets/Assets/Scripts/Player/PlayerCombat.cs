using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Weapon currentWeapon;
    private ItemData currentItemData;
    public Transform spawnPoint;
    public LayerMask groundLayer;

    [Header("Placement Settings")]
    public float checkRadius = 0.5f; // รัศมีเช็คการวางซ้อน
    public LayerMask obstacleLayer; // Layer สำหรับของที่วางไปแล้ว

    void Update()
    {
        // เลือกไอเทมจาก Hotbar 1-8
        for (int i = 0; i < 8; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) SelectFromHotbar(i);
        }

        if (currentWeapon == null) return;

        // การใช้งาน (คลิกซ้ายค้างเพื่อชาร์จ/เล็ง)
        if (Input.GetMouseButton(0)) currentWeapon.StartUse();

        // ปล่อยปุ่มเพื่อพยายามวาง
        if (Input.GetMouseButtonUp(0))
        {
            TryPlaceItem();
        }

        currentWeapon.Tick();
    }

    private void TryPlaceItem()
    {
        if (currentWeapon == null || Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            // --- ขั้นตอนที่ 1: เช็คประเภทพื้นผิว (Tag Check) ---
            if (CanPlaceHere(hit.collider.gameObject))
            {
                // --- ขั้นตอนที่ 2: เช็คพื้นที่ว่าง (Overlap Check) ---
                Collider[] colliders = Physics.OverlapSphere(hit.point, checkRadius, obstacleLayer);

                if (colliders.Length == 0) // พื้นที่ว่างและ Tag ถูกต้อง
                {
                    PerformPlacement(hit.point);
                }
                else
                {
                    Debug.Log("ตรงนี้มีของวางอยู่แล้ว!");
                    currentWeapon.ReleaseUse();
                }
            }
            else
            {
                Debug.Log("พื้นผิวนี้ไม่เหมาะกับไอเทมประเภทนี้!");
                currentWeapon.ReleaseUse();
            }
        }
        else { currentWeapon.ReleaseUse(); }
    }

    private bool CanPlaceHere(GameObject groundObject)
    {
        if (currentItemData == null) return false;

        // เมล็ดผัก (Seed) ต้องวางบน Tag "Dirt" เท่านั้น
        if (currentItemData.itemType == ItemType.Seed)
        {
            return groundObject.CompareTag("Dirt");
        }

        // อาวุธระยะไกล (RangedWeapon) ต้องวางบน Tag "Platform" เท่านั้น
        if (currentItemData.itemType == ItemType.RangedWeapon)
        {
            return groundObject.CompareTag("Platform");
        }

        return true;
    }

    private void PerformPlacement(Vector3 spawnPos)
    {
        currentWeapon.ReleaseUse();
        currentWeapon.transform.SetParent(null);
        currentWeapon.transform.position = spawnPos + new Vector3(0, 0.1f, 0);
        currentWeapon.transform.rotation = Quaternion.identity;

        if (currentWeapon is Bean beanSentry) // ถ้าเป็นผัก ให้ตั้งค่าเลือด
        {
            beanSentry.SetupSentry(currentItemData.vegetableHealth);
        }

        currentWeapon.ActivateAutoFire();
        currentWeapon = null;
        currentItemData = null;
    }

    private void SelectFromHotbar(int index)
    {
        ItemData selectedItem = InventoryManager.instance.hotbarInventory[index];
        if (selectedItem != null) EquipItem(selectedItem);
    }

    public void EquipItem(ItemData item)
    {
        if (item == null || item.weaponPrefab == null) return;
        if (currentWeapon != null) Destroy(currentWeapon.gameObject);

        currentItemData = item;
        GameObject weaponObj = Instantiate(item.weaponPrefab, spawnPoint);
        currentWeapon = weaponObj.GetComponent<Weapon>();
    }
}