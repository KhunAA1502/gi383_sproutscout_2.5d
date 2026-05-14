using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public ItemData itemData; // ใส่ข้อมูลไอเทม (ScriptableObject) ตรงนี้ใน Inspector
    public int amount = 1; // จำนวนไอเท็มที่จะเก็บ

    private void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าเป็นผู้เล่นหรือไม่ (ใช้ Tag "Player")
        if (other.CompareTag("Player"))
        {
            PickupItemLogic();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // สำหรับเกม 2D
        if (other.CompareTag("Player"))
        {
            PickupItemLogic();
        }
    }

    private void PickupItemLogic()
    {
        // หา InventoryManager ในฉาก
        InventoryManager inventory = InventoryManager.instance;

        if (inventory != null && itemData != null)
        {
            inventory.AddItem(itemData, amount);
            Debug.Log($"<color=green>เก็บ {itemData.itemName} x{amount} แล้ว!</color>");
            Destroy(gameObject); // ทำลายวัตถุในฉากหลังจากเก็บแล้ว
        }
    }
}