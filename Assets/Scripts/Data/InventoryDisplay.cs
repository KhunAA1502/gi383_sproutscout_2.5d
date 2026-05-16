using UnityEngine;
using UnityEngine.EventSystems; // ต้องใช้เพื่อจัดการระบบคลิก

public class InventoryDisplay : MonoBehaviour
{
    public static InventoryDisplay instance;

    [Header("UI Settings")]
    public GameObject inventoryPanel;
    public GameObject eventSystem; // ลาก EventSystem ใน Canvas มาใส่ช่องนี้ด้วย
    private bool isOpen = false;

    void Awake()
    {
        // ทำให้ทั้ง Canvas (รวมถึงปุ่มและพื้นหลัง) ไม่ถูกทำลายเมื่อเปลี่ยนซีน
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // ถ้ามีของเก่าอยู่แล้ว ให้ทำลายอันใหม่ทิ้งทันทีเพื่อไม่ให้ซ้ำซ้อน
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            isOpen = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (inventoryPanel == null) return;

        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // ตรวจสอบความถูกต้องของ EventSystem ทุกครั้งที่เปิด
            CheckEventSystem();

            // อัปเดตรูปไอคอนในกระเป๋า (ป้องกันปัญหาสีขาวค้าง)
            UpdateAllSlots();
        }
        else
        {
            // ซ่อนเมาส์เมื่อปิดกระเป๋า (เปิดใช้งานได้หากเกมเป็นแนว FPS/Action)
            // Cursor.visible = false;
            // Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void UpdateAllSlots()
    {
        ItemSlot[] slots = FindObjectsOfType<ItemSlot>();
        foreach (var slot in slots)
        {
            slot.UpdateSlotUI(); // เรียกใช้งานเพื่อวาดรูปไอคอนใหม่
        }
    }

    private void CheckEventSystem()
    {
        // ป้องกันปัญหาข้ามซีนแล้วกดปุ่มไม่ได้เนื่องจาก EventSystem ซ้ำซ้อน
        EventSystem[] allEventSystems = FindObjectsOfType<EventSystem>();
        if (allEventSystems.Length > 1)
        {
            foreach (EventSystem es in allEventSystems)
            {
                // ถ้าไม่ใช่ EventSystem ที่ติดมากับ Canvas นี้ให้ทำลายทิ้ง
                if (es.gameObject != eventSystem)
                {
                    Destroy(es.gameObject);
                }
            }
        }
    }
}