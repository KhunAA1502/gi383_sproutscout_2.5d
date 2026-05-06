using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Weapon currentWeapon;
    private ItemData currentItemData;
    public Transform spawnPoint;
    public LayerMask groundLayer;

    [Header("Placement Settings")]
    public float checkRadius = 0.5f; // ������礡���ҧ��͹
    public LayerMask obstacleLayer; // Layer ����Ѻ�ͧ����ҧ�����

    void Update()
    {
        // ���͡�����ҡ Hotbar 1-8
        for (int i = 0; i < 8; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) SelectFromHotbar(i);
        }

        if (currentWeapon == null) return;

        // �����ҹ (��ԡ���¤�ҧ���ͪ���/���)
        if (Input.GetMouseButton(0)) currentWeapon.StartUse();

        // ����»������;������ҧ
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
            // --- ��鹵͹��� 1: �礻�������鹼�� (Tag Check) ---
            if (CanPlaceHere(hit.collider.gameObject))
            {
                // --- ��鹵͹��� 2: �社�鹷����ҧ (Overlap Check) ---
                Collider[] colliders = Physics.OverlapSphere(hit.point, checkRadius, obstacleLayer);

                if (colliders.Length == 0) // ��鹷����ҧ��� Tag �١��ͧ
                {
                    PerformPlacement(hit.point);
                }
                else
                {
                    Debug.Log("�ç����բͧ�ҧ��������!");
                    currentWeapon.ReleaseUse();
                }
            }
            else
            {
                Debug.Log("��鹼�ǹ���������СѺ�������������!");
                currentWeapon.ReleaseUse();
            }
        }
        else { currentWeapon.ReleaseUse(); }
    }

    private bool CanPlaceHere(GameObject groundObject)
    {
        if (currentItemData == null) return false;

        // ���紼ѡ (Seed) ��ͧ�ҧ�� Tag "Dirt" ��ҹ��
        if (currentItemData.itemType == ItemType.Seed)
        {
            return groundObject.CompareTag("Dirt");
        }

        // ���ظ������ (RangedWeapon) ��ͧ�ҧ�� Tag "Platform" ��ҹ��
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

        if (currentWeapon is Bean beanSentry) // ����繼ѡ ����駤�����ʹ
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

        // ล้างไอเท็มเก่าทั้งหมดที่อยู่ในจุดถือออกก่อน (ป้องกันการแสดงผลซ้อนกัน)
        foreach (Transform child in spawnPoint)
        {
            Destroy(child.gameObject);
        }

        currentItemData = item;
        GameObject weaponObj = Instantiate(item.weaponPrefab, spawnPoint);
        currentWeapon = weaponObj.GetComponent<Weapon>();
    }
}