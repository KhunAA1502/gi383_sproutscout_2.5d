using UnityEngine;
using UnityEngine.UI;

public class InventoryUIUpdater : MonoBehaviour
{
    public Inventory inventory; // ลาก Object Player ที่มีสคริปต์ Inventory มาใส่
    public Transform slotParent; // ลาก Panel ที่เก็บช่อง Slot UI มาใส่

    void Start()
    {
        // สมัครรับแจ้งเตือน: เมื่อของเปลี่ยน ให้รันฟังก์ชัน DrawInventory
        inventory.onInventoryChanged += DrawInventory;
    }

    void DrawInventory()
    {
        // วนลูป Slot UI ทั้งหมด แล้วดึงข้อมูลจาก inventory.slots มาแสดงผล
        // ตัวอย่างเช่น การเปลี่ยนรูป Icon และจำนวน Text ในแต่ละช่อง
        Debug.Log("UI กำลังวาดไอเทมใหม่ตามข้อมูลในกระเป๋า...");

        // (ส่วนนี้ขึ้นอยู่กับว่าคุณสร้าง UI ไว้ยังไง แต่หัวใจหลักคือเรียกใช้ onInventoryChanged)
    }
}