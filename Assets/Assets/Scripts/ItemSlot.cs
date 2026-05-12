using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public int slotIndex;
    public bool isHotbar;
    public Image iconImage; // อ้างอิง Object "Icon" ของช่อง
    public InventorySlot currentSlot;

    private ItemData currentItem => currentSlot?.item;

    private void Start() => UpdateSlotUI();

    public void UpdateSlotUI()
    {
        if (InventoryManager.instance == null) return;

        currentSlot = isHotbar ?
            InventoryManager.instance.hotbarInventory[slotIndex] :
            InventoryManager.instance.mainInventory[slotIndex];

        if (currentItem != null && currentItem.icon != null)
        {
            iconImage.sprite = currentItem.icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null) return;

        // --- คลิกซ้าย: เพื่อใช้งานหรือเลือกติดตั้งไอเท็ม ---
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isHotbar)
            {
                // ถ้าเป็นช่อง Hotbar ให้สั่งติดตั้งไอเท็มมาที่ตัวละคร
                PlayerCombat playerCombat = FindFirstObjectByType<PlayerCombat>();
                if (playerCombat != null) playerCombat.EquipItem(currentItem, slotIndex);
            }
            else
            {
                // ถ้าอยู่ในกระเป๋าหลัก ให้ย้ายลง Hotbar
                MoveItem(InventoryManager.instance.mainInventory, InventoryManager.instance.hotbarInventory);
            }
        }
        // --- คลิกขวา: เพื่อย้ายของสลับที่ (Hotbar <-> กระเป๋าหลัก) ---
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isHotbar)
                MoveItem(InventoryManager.instance.hotbarInventory, InventoryManager.instance.mainInventory);
            else
                MoveItem(InventoryManager.instance.mainInventory, InventoryManager.instance.hotbarInventory);
        }
    }

    private void MoveItem(List<InventorySlot> fromList, List<InventorySlot> toList)
    {
        if (currentSlot == null || currentItem == null) return;

        ItemData itemToMove = currentItem;
        int amountToMove = currentSlot.amount;

        fromList[slotIndex].item = null;
        fromList[slotIndex].amount = 0;

        if (!isHotbar)
        {
            for (int i = 0; i < fromList.Count - 1; i++)
            {
                if (fromList[i].item == null && fromList[i + 1].item != null)
                {
                    fromList[i].item = fromList[i + 1].item;
                    fromList[i].amount = fromList[i + 1].amount;
                    fromList[i + 1].item = null;
                    fromList[i + 1].amount = 0;
                }
            }
        }

        for (int i = 0; i < toList.Count; i++)
        {
            if (toList[i].item == null)
            {
                toList[i].item = itemToMove;
                toList[i].amount = amountToMove;
                break;
            }
        }

        foreach (var slot in FindObjectsByType<ItemSlot>(FindObjectsSortMode.None))
        {
            slot.UpdateSlotUI();
        }
    }
}