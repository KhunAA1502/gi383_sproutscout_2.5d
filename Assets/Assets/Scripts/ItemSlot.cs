using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public int slotIndex;
    public bool isHotbar;
    public Image iconImage; // �ҡ Object "Icon" �����㹪�ͧ���
    public ItemData currentItem;

    private void Start() => UpdateSlotUI();

    public void UpdateSlotUI()
    {
        if (InventoryManager.instance == null) return;

        // �֧������������� Index
        currentItem = isHotbar ?
            InventoryManager.instance.hotbarInventory[slotIndex] :
            InventoryManager.instance.mainInventory[slotIndex];

        if (currentItem != null && currentItem.icon != null)
        {
            iconImage.sprite = currentItem.icon;
            iconImage.enabled = true; // �ʴ��ٻ (��ѭ���բ�Ǥ�ҧ)
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false; // ��͹�ٻ���������բͧ (��ѭ���բ�Ǥ�ҧ)
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

    private void MoveItem(List<ItemData> fromList, List<ItemData> toList)
    {
        // 1. �������͡�ҡ��ͧ����ԡ
        fromList[slotIndex] = null;

        // 2. �Ѵ���§����: ����͹�����������͢����᷹����ͧ��ҧ (੾��㹡����Һ�)
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

        // 3. ��������ŧ���ʵ���·ҧ (��ͧ��ҧ�á)
        for (int i = 0; i < toList.Count; i++)
        {
            if (toList[i] == null)
            {
                toList[i] = currentItem;
                break;
            }
        }

        // 4. ������ء��ͧ�ѻവ�ٻ˹�Ҩ�����
        foreach (var slot in FindObjectsOfType<ItemSlot>())
        {
            slot.UpdateSlotUI();
        }
    }
}