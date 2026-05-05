using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Data Storage")]
    public List<ItemData> mainInventory = new List<ItemData>(new ItemData[20]); // 20 ช่องบน
    public List<ItemData> hotbarInventory = new List<ItemData>(new ItemData[8]);  // 8 ช่องล่าง

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
    }

    public void AddItem(ItemData item)
    {
        // เพิ่มไอเทมลงในช่องว่างแรกที่เจอ
        for (int i = 0; i < hotbarInventory.Count; i++)
        {
            if (hotbarInventory[i] == null) { hotbarInventory[i] = item; return; }
        }
        for (int i = 0; i < mainInventory.Count; i++)
        {
            if (mainInventory[i] == null) { mainInventory[i] = item; return; }
        }
    }
}