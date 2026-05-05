using System; // เพิ่มตัวนี้
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int maxSlots = 20;
    public List<InventorySlot> slots = new List<InventorySlot>();

    // Event สำหรับแจ้งเตือน UI เมื่อมีการเปลี่ยนแปลงไอเทม
    public Action onInventoryChanged;

    [SerializeField] private ItemData meleeItem;
    // ... (โค้ดเดิมของคุณใน Awake และ Start) ...

    public bool AddItem(ItemData item, int amount = 1)
    {
        foreach (var slot in slots)
        {
            if (slot.item == item)
            {
                slot.amount += amount;
                onInventoryChanged?.Invoke(); // แจ้ง UI
                return true;
            }
        }

        foreach (var slot in slots)
        {
            if (slot.item == null)
            {
                slot.item = item;
                slot.amount = amount;
                onInventoryChanged?.Invoke(); // แจ้ง UI
                return true;
            }
        }
        return false;
    }

    public void RemoveItem(ItemData item, int amount = 1)
    {
        foreach (var slot in slots)
        {
            if (slot.item == item)
            {
                slot.amount -= amount;
                if (slot.amount <= 0)
                {
                    slot.item = null;
                    slot.amount = 0;
                }
                onInventoryChanged?.Invoke();
                return;
            }
        }
    }

    public void UseItem(ItemData item)
    {
        if (item == null) return;

        Debug.Log("Using item: " + item.itemName);

        switch (item.itemType)
        {
            case ItemType.Seed:
                // เรียกใช้ระบบปลูกพืช (ส่งข้อมูลเมล็ดไปที่มือตัวละคร)
                FindObjectOfType<PlayerCombat>().EquipItem(item);
                Debug.Log("Equipped " + item.itemName + " for planting!");
                break;

            case ItemType.RangedWeapon:
                // เรียกใช้เพื่อติดตั้งอาวุธระยะไกล
                FindObjectOfType<PlayerCombat>().EquipItem(item);
                Debug.Log("Equipped " + item.itemName + " for combat!");
                break;
        }
    }
}