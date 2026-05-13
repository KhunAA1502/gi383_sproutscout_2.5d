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
        if (!CanPlaceWeaponHere() || weaponData.weaponPrefab == null) return false;

        // สร้าง weapon
        GameObject weaponObj = Instantiate(weaponData.weaponPrefab, transform.position, Quaternion.identity);
        currentWeapon = weaponObj.GetComponent<Weapon>();

        if (currentWeapon != null)
        {
            // ตั้งค่า weapon
            if (currentWeapon is Bean bean)
            {
                int health = (weaponData.plantData != null) ? weaponData.plantData.vegetableHealth : 3;
                bean.SetupSentry(health);
            }

            currentWeapon.ActivateAutoFire();
            isOccupied = true;
            UpdateVisual();
            return true;
        }

        return false;
    }

    public void RemoveWeapon()
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
            currentWeapon = null;
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
        // Highlight เมื่อเมาส์ hover
    }

    private void OnMouseExit()
    {
        // ยกเลิก highlight
    }
}