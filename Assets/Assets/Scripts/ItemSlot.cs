using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public int slotIndex;
    public bool isHotbar;
    public Image iconImage; // ลาก Object "Icon" มาใส่ในช่องนี้
    public ItemData currentItem;

    private void Start() => UpdateSlotUI();

    public void UpdateSlotUI()
    {
        if (InventoryManager.instance == null) return;

        // ดึงข้อมูลไอเทมตาม Index
        currentItem = isHotbar ?
            InventoryManager.instance.hotbarInventory[slotIndex] :
            InventoryManager.instance.mainInventory[slotIndex];

        if (currentItem != null && currentItem.icon != null)
        {
            iconImage.sprite = currentItem.icon;
            iconImage.enabled = true; // แสดงรูป (แก้ปัญหาสีขาวค้าง)
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false; // ซ่อนรูปเมื่อไม่มีของ (แก้ปัญหาสีขาวค้าง)
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null) return;

        if (isHotbar)
            MoveItem(InventoryManager.instance.hotbarInventory, InventoryManager.instance.mainInventory);
        else
            MoveItem(InventoryManager.instance.mainInventory, InventoryManager.instance.hotbarInventory);
    }

    private void MoveItem(List<ItemData> fromList, List<ItemData> toList)
    {
        // 1. นำไอเทมออกจากช่องที่คลิก
        fromList[slotIndex] = null;

        // 2. จัดเรียงใหม่: เลื่อนไอเทมที่เหลือขึ้นมาแทนที่ช่องว่าง (เฉพาะในกระเป๋าบน)
        if (!isHotbar)
        {
            for (int i = 0; i < fromList.Count - 1; i++)
            {
                if (fromList[i] == null && fromList[i + 1] != null)
                {
                    fromList[i] = fromList[i + 1];
                    fromList[i + 1] = null;
                }
            }
        }

        // 3. เพิ่มไอเทมลงในลิสต์ปลายทาง (ช่องว่างแรก)
        for (int i = 0; i < toList.Count; i++)
        {
            if (toList[i] == null)
            {
                toList[i] = currentItem;
                break;
            }
        }

        // 4. สั่งให้ทุกช่องอัปเดตรูปหน้าจอใหม่
        foreach (var slot in FindObjectsOfType<ItemSlot>())
        {
            slot.UpdateSlotUI();
        }
    }
}