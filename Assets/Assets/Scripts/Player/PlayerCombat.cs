using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Weapon currentWeapon;
    private ItemData currentItemData;

    [Header("Weapon Settings")]
    [SerializeField] private ItemData slot1;
    [SerializeField] private ItemData slot2;
    public Transform spawnPoint;

    [Header("Drag and Drop Settings")]
    [SerializeField] private GameObject itemWorldPrefab;
    private bool isDragging = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipFromInventory(slot1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipFromInventory(slot2);

        HandleDragAndDrop();

        if (currentWeapon == null) return;

        if (Input.GetMouseButtonDown(0) && !isDragging)
            currentWeapon.StartUse();

        if (Input.GetMouseButtonUp(0))
            currentWeapon.ReleaseUse();

        currentWeapon.Tick();
    }

    public void EquipFromInventory(ItemData item)
    {
        if (item == null || item.weaponPrefab == null)
        {
            Debug.LogWarning("สล็อตว่างเปล่า หรือไม่มี Prefab อาวุธ!");
            return;
        }

        if (currentWeapon != null)
            Destroy(currentWeapon.gameObject);

        currentItemData = item;
        GameObject weaponObj = Instantiate(item.weaponPrefab, spawnPoint);
        currentWeapon = weaponObj.GetComponent<Weapon>();
        Debug.Log("ติดตั้งอาวุธเรียบร้อย: " + item.itemName);
    }

    private void HandleDragAndDrop()
    {
        // 1. เช็คตอนคลิกขวา
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            Debug.Log("คลิกเมาส์ขวาที่ตำแหน่ง: " + mousePos);

            if (hit.collider != null)
            {
                Debug.Log("คลิกโดนวัตถุชื่อ: " + hit.collider.gameObject.name);

                if (hit.collider.gameObject == gameObject) // เช็คว่าโดนตัว Player ไหม
                {
                    if (currentWeapon != null)
                    {
                        isDragging = true;
                        Debug.Log("<color=green>เริ่มลากอาวุธจากตัวผู้เล่น!</color>");
                    }
                    else
                    {
                        Debug.Log("<color=yellow>ไม่ได้ถืออาวุธอยู่ เลยลากไม่ได้</color>");
                    }
                }
            }
            else
            {
                Debug.Log("<color=red>คลิกไม่โดน Collider อะไรเลย (เช็ค Collider ที่ตัว Player ด้วย)</color>");
            }
        }

        // 2. เช็คตอนปล่อยเมาส์
        if (Input.GetMouseButtonUp(1) && isDragging)
        {
            Debug.Log("<color=cyan>ปล่อยเมาส์ เตรียมสร้างของลงพื้น...</color>");
            isDragging = false;
            DropWeaponAtMouse();
        }
    }

    private void DropWeaponAtMouse()
    {
        if (currentItemData == null || currentWeapon == null)
        {
            Debug.LogError("ข้อมูลอาวุธหายไปก่อนจะทิ้งลงพื้น!");
            return;
        }

        if (itemWorldPrefab == null)
        {
            Debug.LogError("ลืมใส่ Prefab วงกลม C ในช่อง Item World Prefab!");
            return;
        }

        Vector3 dropPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dropPos.z = 0;

        GameObject itemFloor = Instantiate(itemWorldPrefab, dropPos, Quaternion.identity);
        ItemWorld itemWorldScript = itemFloor.GetComponent<ItemWorld>();

        if (itemWorldScript != null)
        {
            itemWorldScript.itemData = currentItemData;
            Debug.Log("<color=green>สร้างของบนพื้นสำเร็จ! ข้อมูลอาวุธ: </color>" + currentItemData.itemName);
        }
        else
        {
            Debug.LogError("Prefab บนพื้นไม่มีสคริปต์ ItemWorld ติดอยู่!");
        }

        Destroy(currentWeapon.gameObject);
        currentWeapon = null;
        currentItemData = null;
    }
}