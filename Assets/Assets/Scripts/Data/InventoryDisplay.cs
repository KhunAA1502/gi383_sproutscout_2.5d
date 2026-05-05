using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    public GameObject inventoryPanel; // ลาก Panel ของกระเป๋ามาใส่ช่องนี้
    private bool isOpen = false;

    void Update()
    {
        // ตรวจสอบการกดปุ่ม I (หรือเปลี่ยนเป็น KeyCode อื่นตามชอบ)
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen; // สลับค่า true/false
        inventoryPanel.SetActive(isOpen); // เปิดหรือปิดหน้าจอตามค่า isOpen

        if (isOpen)
        {
            // ถ้าเปิดกระเป๋า อาจจะสั่งหยุดเวลาหรือเปิด Cursor เมาส์
            // Time.timeScale = 0f; 
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // ถ้าปิดกระเป๋า ให้เกมเดินต่อและซ่อนเมาส์ (ถ้าจำเป็น)
            // Time.timeScale = 1f;
            // Cursor.visible = false;
        }
    }
}