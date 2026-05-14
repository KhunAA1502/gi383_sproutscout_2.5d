using UnityEngine;

public class WeaponPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    public bool isOccupied = false;
    public int maxWeapons = 0; // 0 = unlimited

    private System.Collections.Generic.List<Weapon> activeWeapons = new System.Collections.Generic.List<Weapon>();

    [Header("Visual Feedback")]
    public Material availableMaterial;
    public Material occupiedMaterial;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateVisual();
    }

    public bool CanPlaceWeaponHere()
    {
        // ทำความสะอาด list ของอาวุธที่ถูกทำลายไปแล้ว
        activeWeapons.RemoveAll(w => w == null);
        
        if (maxWeapons <= 0) return true; // ไม่มีขีดจำกัด
        return activeWeapons.Count < maxWeapons;
    }

    public bool PlaceWeapon(ItemData weaponData, PlayerCombat playerCombat, Vector3? placementPos = null)
    {
        Debug.Log($"[WeaponPlatform] PlaceWeapon() เรียก - weaponData={weaponData?.itemName ?? "null"}");
        
        if (!CanPlaceWeaponHere())
        {
            Debug.LogWarning("[WeaponPlatform] CanPlaceWeaponHere() = false");
            return false;
        }

        if (weaponData.weaponPrefab == null)
        {
            Debug.LogError("[WeaponPlatform] weaponPrefab is null");
            return false;
        }

        // ใช้ตำแหน่งที่ส่งมา (ตำแหน่งเม้าส์) หรือถ้าไม่มีให้ใช้ตำแหน่งกลาง Platform
        Vector3 spawnPosition = placementPos ?? transform.position;
        
        // ปรับความสูง (Y) ให้พอดีกับผิวของ Platform เพื่อไม่ให้จม
        // สมมติว่า Platform มีความหนา หรือใช้ offset เล็กน้อย
        spawnPosition.y = transform.position.y + 0.1f; 

        Debug.Log($"[WeaponPlatform] Instantiate weaponPrefab: {weaponData.weaponPrefab.name} ที่ {spawnPosition}");

        // สร้าง weapon
        GameObject weaponObj = Instantiate(weaponData.weaponPrefab, spawnPosition, Quaternion.identity);
        Weapon newWeapon = weaponObj.GetComponent<Weapon>();

        if (newWeapon != null)
        {
            Debug.Log($"[WeaponPlatform] Weapon ได้สำเร็จ: {newWeapon.gameObject.name}");

            // ตั้งค่า weapon
            if (newWeapon is Bean bean)
            {
                int health = (weaponData.plantData != null) ? weaponData.plantData.vegetableHealth : 3;
                bean.SetupSentry(health);
                Debug.Log($"[WeaponPlatform] Bean Setup: health={health}");
            }

            newWeapon.ActivateAutoFire();
            activeWeapons.Add(newWeapon);
            isOccupied = true;
            UpdateVisual();
            Debug.Log("[WeaponPlatform] วางอาวุธสำเร็จ!");
            
            return true;
        }

        Debug.LogError($"[WeaponPlatform] ไม่มี Weapon component บน {weaponObj.name}");
        return false;
    }

    public void RemoveWeapon()
    {
        // ทำความสะอาด list
        activeWeapons.RemoveAll(w => w == null);

        if (activeWeapons.Count > 0)
        {
            // ลบอาวุธล่าสุดที่วาง
            Weapon lastWeapon = activeWeapons[activeWeapons.Count - 1];
            Debug.Log($"[WeaponPlatform] ลบอาวุธ: {lastWeapon.gameObject.name}");
            Destroy(lastWeapon.gameObject);
            activeWeapons.RemoveAt(activeWeapons.Count - 1);
        }
        
        if (activeWeapons.Count == 0)
        {
            isOccupied = false;
        }
        
        UpdateVisual();
        Debug.Log($"[WeaponPlatform] RemoveWeapon() เสร็จ - เหลืออาวุธ {activeWeapons.Count} ชิ้น");
    }

    public void RemoveAllWeapons()
    {
        foreach (var weapon in activeWeapons)
        {
            if (weapon != null) Destroy(weapon.gameObject);
        }
        activeWeapons.Clear();
        isOccupied = false;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        // อัปเดต list ก่อนเช็ค visual
        activeWeapons.RemoveAll(w => w == null);
        isOccupied = activeWeapons.Count > 0;

        if (meshRenderer != null)
        {
            meshRenderer.material = isOccupied ? occupiedMaterial : availableMaterial;
        }
    }

    private void OnMouseEnter()
    {
        // Highlight เมื่อเมาส์ hover
    }

    private void OnMouseExit()
    {
        // ยกเลิก highlight
    }
}