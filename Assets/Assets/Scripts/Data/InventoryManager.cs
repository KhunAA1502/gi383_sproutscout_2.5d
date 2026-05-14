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
        if (item == null || amount <= 0) return;

        // First try to stack in hotbar
        foreach (var slot in hotbarInventory)
        {
            if (slot.item == item && slot.amount > 0 && slot.amount < 99) // Max stack 99
            {
                int canAdd = Mathf.Min(amount, 99 - slot.amount);
                slot.amount += canAdd;
                amount -= canAdd;
                if (amount <= 0)
                {
                    UpdateAllSlots();
                    return;
                }
            }
        }

        // Then try to add to empty slot in hotbar
        foreach (var slot in hotbarInventory)
        {
            if (slot.item == null)
            {
                slot.item = item;
                slot.amount = Mathf.Min(amount, 99);
                amount -= slot.amount;
                if (amount <= 0)
                {
                    UpdateAllSlots();
                    return;
                }
            }
        }

        // If hotbar is full, try to stack in main inventory
        foreach (var slot in mainInventory)
        {
            if (slot.item == item && slot.amount > 0 && slot.amount < 99)
            {
                int canAdd = Mathf.Min(amount, 99 - slot.amount);
                slot.amount += canAdd;
                amount -= canAdd;
                if (amount <= 0)
                {
                    UpdateAllSlots();
                    return;
                }
            }
        }

        // Then try to add to empty slot in main inventory
        foreach (var slot in mainInventory)
        {
            if (slot.item == null)
            {
                slot.item = item;
                slot.amount = Mathf.Min(amount, 99);
                amount -= slot.amount;
                if (amount <= 0)
                {
                    UpdateAllSlots();
                    return;
                }
            }
        }

        // If still have remaining items, try to merge stacks across inventories
        if (amount > 0)
        {
            // Try to stack remaining in hotbar again
            foreach (var slot in hotbarInventory)
            {
                if (slot.item == item && slot.amount < 99)
                {
                    int canAdd = Mathf.Min(amount, 99 - slot.amount);
                    slot.amount += canAdd;
                    amount -= canAdd;
                    if (amount <= 0) break;
                }
            }

            // Then in main inventory
            if (amount > 0)
            {
                foreach (var slot in mainInventory)
                {
                    if (slot.item == item && slot.amount < 99)
                    {
                        int canAdd = Mathf.Min(amount, 99 - slot.amount);
                        slot.amount += canAdd;
                        amount -= canAdd;
                        if (amount <= 0) break;
                    }
                }
            }
        }

        UpdateAllSlots();

        if (amount > 0)
        {
            Debug.LogWarning($"[InventoryManager] ไม่สามารถเพิ่มไอเท็ม {item.itemName} จำนวน {amount} ได้: inventory เต็ม");
        }
    }

    private void UpdateAllSlots()
    {
        foreach (var slot in FindObjectsOfType<ItemSlot>())
        {
            slot.UpdateSlotUI();
        }
    }
}