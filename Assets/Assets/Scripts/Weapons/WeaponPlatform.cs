using UnityEngine;

public class WeaponPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    public bool isOccupied = false;
    public Weapon currentWeapon;
    public int maxWeapons = 1; // สามารถวางได้กี่ชิ้น

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
        return !isOccupied;
    }

    public bool PlaceWeapon(ItemData weaponData, PlayerCombat playerCombat)
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

        Debug.Log($"[WeaponPlatform] Instantiate weaponPrefab: {weaponData.weaponPrefab.name} ที่ {transform.position}");

        // สร้าง weapon
        GameObject weaponObj = Instantiate(weaponData.weaponPrefab, transform.position, Quaternion.identity);
        currentWeapon = weaponObj.GetComponent<Weapon>();

        if (currentWeapon != null)
        {
            Debug.Log($"[WeaponPlatform] Weapon ได้สำเร็จ: {currentWeapon.gameObject.name}");

            // ตั้งค่า weapon
            if (currentWeapon is Bean bean)
            {
                int health = (weaponData.plantData != null) ? weaponData.plantData.vegetableHealth : 3;
                bean.SetupSentry(health);
                Debug.Log($"[WeaponPlatform] Bean Setup: health={health}");
            }

            currentWeapon.ActivateAutoFire();
            isOccupied = true;
            UpdateVisual();
            Debug.Log("[WeaponPlatform] วางอาวุธสำเร็จ!");
            
            // เซ็ตเวลา 10 วินาที ให้ weapon หายไป เพื่อให้สามารถวางอันใหม่ได้
            StartCoroutine(RemoveWeaponAfterDelay(10f));
            
            return true;
        }

        Debug.LogError($"[WeaponPlatform] ไม่มี Weapon component บน {weaponObj.name}");
        return false;
    }
    
    private System.Collections.IEnumerator RemoveWeaponAfterDelay(float delay)
    {
        Debug.Log($"[WeaponPlatform] RemoveWeaponAfterDelay() เริ่ม - รอ {delay} วินาที");
        yield return new WaitForSeconds(delay);
        Debug.Log($"[WeaponPlatform] RemoveWeaponAfterDelay() สิ้นสุด - เรียก RemoveWeapon()");
        RemoveWeapon();
    }

    public void RemoveWeapon()
    {
        Debug.Log($"[WeaponPlatform] RemoveWeapon() เรียก - currentWeapon={currentWeapon?.name ?? "null"}, isOccupied={isOccupied}");
        
        if (currentWeapon != null)
        {
            Debug.Log($"[WeaponPlatform] ลบอาวุธ: {currentWeapon.gameObject.name}");
            Destroy(currentWeapon.gameObject);
            currentWeapon = null;
        }
        
        isOccupied = false;
        UpdateVisual();
        Debug.Log("[WeaponPlatform] RemoveWeapon() เสร็จ - isOccupied=false");
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
        // Highlight เมื่อเมาส์ hover
    }

    private void OnMouseExit()
    {
        // ยกเลิก highlight
    }
}