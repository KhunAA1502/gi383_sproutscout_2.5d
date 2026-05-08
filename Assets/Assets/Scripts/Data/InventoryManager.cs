using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Data Storage")]
    public List<InventorySlot> mainInventory = new List<InventorySlot>(); // 20 ช่องบน
    public List<InventorySlot> hotbarInventory = new List<InventorySlot>();  // 8 ช่องล่าง

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize slots
        for (int i = 0; i < 20; i++)
        {
            mainInventory.Add(new InventorySlot());
        }
        for (int i = 0; i < 8; i++)
        {
            hotbarInventory.Add(new InventorySlot());
        }
    }

    public void AddItem(ItemData item, int amount = 1)
    {
        // First try to stack in hotbar
        foreach (var slot in hotbarInventory)
        {
            if (slot.item == item && slot.amount > 0)
            {
                slot.amount += amount;
                UpdateAllSlots();
                return;
            }
        }

        // Then try to add to empty slot in hotbar
        foreach (var slot in hotbarInventory)
        {
            if (slot.item == null)
            {
                slot.item = item;
                slot.amount = amount;
                UpdateAllSlots();
                return;
            }
        }

        Debug.LogWarning("[InventoryManager] hotbar เต็ม: ไม่มีที่ว่างสำหรับเพิ่มไอเท็ม " + item.itemName);
    }

    private void UpdateAllSlots()
    {
        foreach (var slot in FindObjectsOfType<ItemSlot>())
        {
            slot.UpdateSlotUI();
        }
    }
}