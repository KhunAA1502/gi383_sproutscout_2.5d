using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public ItemData itemData; // ใส่ข้อมูลไอเทม (ScriptableObject) ตรงนี้ใน Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ตรวจสอบว่าเป็นผู้เล่นหรือไม่ (ใช้ Tag "Player")
        if (other.CompareTag("Player"))
        {
            // หา InventoryManager ในฉาก
            InventoryManager inventory = FindObjectOfType<InventoryManager>();

            if (inventory != null)
            {
                inventory.AddItem(itemData);
                Debug.Log("เก็บ " + itemData.itemName + " แล้ว!");
                Destroy(gameObject); // ทำลายวัตถุในฉากหลังจากเก็บแล้ว
            }
        }
    }
}