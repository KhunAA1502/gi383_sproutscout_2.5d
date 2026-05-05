using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();
    public Image[] uiSlots; // ลาก Image ของแต่ละช่องมาใส่ที่นี่ใน Inspector
    public InventoryDisplay inventoryDisplay; 

    private ItemData pendingMonster;
    private int pendingSlotIndex;
    private bool isPlacing = false;

    void Start()
    {
        // พยายามหา InventoryDisplay หากไม่ได้ตั้งค่าไว้
        if (inventoryDisplay == null) inventoryDisplay = FindObjectOfType<InventoryDisplay>();

        // สร้างระบบคลิกให้กับ Image แต่ละช่องโดยอัตโนมัติ
        for (int i = 0; i < uiSlots.Length; i++)
        {
            if (uiSlots[i] != null)
            {
                // ตรวจสอบว่ามี Button หรือยัง ถ้าไม่มีให้เพิ่มเข้าไป
                Button btn = uiSlots[i].GetComponent<Button>();
                if (btn == null)
                {
                    btn = uiSlots[i].gameObject.AddComponent<Button>();
                    btn.transition = Selectable.Transition.ColorTint;
                }
                
                int index = i;
                btn.onClick.AddListener(() => UseItem(index));

                // ตรวจสอบให้แน่ใจว่า Image สามารถรับการคลิกได้
                uiSlots[i].raycastTarget = true;
            }
        }
        RefreshUI();
    }

    void Update()
    {
        if (isPlacing)
        {
            // คลิกซ้ายเพื่อวางมอนสเตอร์ (ต้องไม่เป็นการคลิกบน UI)
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                PlaceMonsterAtMouse();
            }
            
            // คลิกขวาเพื่อยกเลิกการวาง
            if (Input.GetMouseButtonDown(1))
            {
                CancelPlacing();
            }
        }
    }

    public void AddItem(ItemData data)
    {
        if (data == null)
        {
            Debug.LogWarning("พยายามเพิ่มไอเทมที่เป็น null!");
            return;
        }

        if (items.Count < uiSlots.Length)
        {
            items.Add(data);
            Debug.Log("เก็บไอเทม: " + data.itemName);
            RefreshUI();
        }
        else
        {
            Debug.Log("กระเป๋าเต็ม!");
        }
    }

    public void UseItem(int slotIndex)
    {
        if (slotIndex >= items.Count) return;

        ItemData data = items[slotIndex];
        if (data == null) return;

        Debug.Log("เลือกใช้: " + data.itemName);

        if (data.itemType == ItemType.Monster)
        {
            if (data.monsterPrefab != null)
            {
                pendingMonster = data;
                pendingSlotIndex = slotIndex;
                
                // ปิดหน้าต่างกระเป๋า
                if (inventoryDisplay != null)
                {
                    inventoryDisplay.ToggleInventory();
                }

                // เริ่มโหมดการวางหลังจากดีเลย์เล็กน้อย
                Invoke("StartPlacing", 0.1f);
            }
            else
            {
                Debug.LogWarning("มอนสเตอร์นี้ไม่มี Prefab: " + data.itemName);
            }
        }
        else if (data.itemType == ItemType.Crop)
        {
            items.RemoveAt(slotIndex);
            RefreshUI();
        }
    }

    private void StartPlacing()
    {
        isPlacing = true;
        Debug.Log("อยู่ในโหมดวาง: คลิกที่พื้นเพื่อเสกมอนสเตอร์");
    }

    private void PlaceMonsterAtMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // ตรวจสอบให้แน่ใจว่ามอนสเตอร์อยู่ในระนาบ 2D

        // เสกมอนสเตอร์ที่จุดที่เมาส์ชี้
        if (pendingMonster != null && pendingMonster.monsterPrefab != null)
        {
            Instantiate(pendingMonster.monsterPrefab, mousePos, Quaternion.identity);
            Debug.Log("เสก " + pendingMonster.itemName + " สำเร็จ!");

            // ลบออกจากกระเป๋า
            items.RemoveAt(pendingSlotIndex);
            RefreshUI();
        }

        // ออกจากโหมดวาง
        isPlacing = false;
        pendingMonster = null;
    }

    private void CancelPlacing()
    {
        isPlacing = false;
        pendingMonster = null;
        Debug.Log("ยกเลิกการวาง");
        if (inventoryDisplay != null) inventoryDisplay.ToggleInventory();
    }

    public void RefreshUI()
    {
        for (int i = 0; i < uiSlots.Length; i++)
        {
            if (uiSlots[i] == null) continue;

            if (i < items.Count && items[i] != null)
            {
                uiSlots[i].sprite = items[i].icon;
                uiSlots[i].enabled = true;
            }
            else
            {
                uiSlots[i].sprite = null;
                uiSlots[i].enabled = false;
            }
        }
    }
}