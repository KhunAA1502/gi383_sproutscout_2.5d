using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int slotIndex;
    public bool isHotbar;
    public Image iconImage; // �ҡ Object "Icon" �����㹪�ͧ���
    public Text amountText; // เพิ่ม Text สำหรับแสดงจำนวน
    public InventorySlot currentSlot;

    private Canvas parentCanvas;
    private GameObject dragIconObject;
    private RectTransform dragIconRect;
    private static ItemSlot draggingSlot;

    private void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        UpdateSlotUI();
    }

    public void UpdateSlotUI()
    {
        if (InventoryManager.instance == null) return;

        // �֧������������� Index
        currentSlot = isHotbar ?
            InventoryManager.instance.hotbarInventory[slotIndex] :
            InventoryManager.instance.mainInventory[slotIndex];

        if (currentSlot != null && currentSlot.item != null && currentSlot.item.icon != null)
        {
            iconImage.sprite = currentSlot.item.icon;
            iconImage.enabled = true;

            // แสดงจำนวนถ้ามากกว่า 1
            if (currentSlot.amount > 1 && amountText != null)
            {
                amountText.text = currentSlot.amount.ToString();
                amountText.enabled = true;
            }
            else if (amountText != null)
            {
                amountText.text = "";
                amountText.enabled = false;
            }
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            if (amountText != null)
            {
                amountText.text = "";
                amountText.enabled = false;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentSlot == null || currentSlot.item == null) return;

        if (!isHotbar) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            PlayerCombat playerCombat = FindFirstObjectByType<PlayerCombat>();
            if (playerCombat != null) playerCombat.EquipItem(currentSlot.item, slotIndex);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // อาจเพิ่มคำสั่ง quick move หรือ reorder hotbar ได้ในอนาคต
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isHotbar) return;
        if (currentSlot == null || currentSlot.item == null || iconImage.sprite == null) return;

        draggingSlot = this;
        CreateDragIcon();
        SetDragIconPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIconRect == null) return;
        SetDragIconPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIconObject != null)
        {
            Destroy(dragIconObject);
            dragIconObject = null;
            dragIconRect = null;
        }

        if (draggingSlot == null) return;

        GameObject target = eventData.pointerCurrentRaycast.gameObject;
        if (target != null)
        {
            ItemSlot targetSlot = target.GetComponentInParent<ItemSlot>();
            if (targetSlot != null && targetSlot != this && targetSlot.isHotbar && isHotbar)
            {
                SwapWith(targetSlot);
                draggingSlot = null;
                return;
            }
        }

        if (isHotbar && currentSlot != null && currentSlot.item != null && currentSlot.item.itemType == ItemType.Seed)
        {
            PlayerFarming playerFarming = FindFirstObjectByType<PlayerFarming>();
            if (playerFarming != null)
            {
                playerFarming.TryPlaceSeedFromScreenPosition(currentSlot.item, slotIndex, eventData.position);
            }
        }

        draggingSlot = null;
    }

    private void CreateDragIcon()
    {
        if (parentCanvas == null) return;

        dragIconObject = new GameObject("DragIcon");
        dragIconObject.transform.SetParent(parentCanvas.transform, false);
        dragIconObject.transform.SetAsLastSibling();

        dragIconRect = dragIconObject.AddComponent<RectTransform>();
        dragIconRect.sizeDelta = iconImage.rectTransform.sizeDelta;

        Image dragImage = dragIconObject.AddComponent<Image>();
        dragImage.sprite = currentSlot.item.icon;
        dragImage.raycastTarget = false;
        dragImage.preserveAspect = true;
    }

    private void SetDragIconPosition(PointerEventData eventData)
    {
        if (dragIconRect == null || parentCanvas == null) return;

        Vector2 localPoint;
        RectTransform canvasRect = parentCanvas.transform as RectTransform;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            dragIconRect.anchoredPosition = localPoint;
        }
    }

    private void SwapWith(ItemSlot targetSlot)
    {
        if (InventoryManager.instance == null) return;

        InventorySlot sourceSlot = currentSlot;
        InventorySlot targetSlotData = targetSlot.currentSlot;

        List<InventorySlot> sourceList = isHotbar ? InventoryManager.instance.hotbarInventory : InventoryManager.instance.mainInventory;
        List<InventorySlot> targetList = targetSlot.isHotbar ? InventoryManager.instance.hotbarInventory : InventoryManager.instance.mainInventory;

        sourceList[slotIndex] = targetSlotData;
        targetList[targetSlot.slotIndex] = sourceSlot;

        UpdateSlotUI();
        targetSlot.UpdateSlotUI();

        Debug.Log($"[ItemSlot] Swapped item from slot {slotIndex} ({(isHotbar ? "Hotbar" : "Inventory")}) to slot {targetSlot.slotIndex} ({(targetSlot.isHotbar ? "Hotbar" : "Inventory")})");
    }

    private void MoveItem(List<InventorySlot> fromList, List<InventorySlot> toList)
    {
        InventorySlot movingSlot = fromList[slotIndex];
        fromList[slotIndex] = new InventorySlot(); // Clear slot

        if (!isHotbar)
        {
            // Shift items in main inventory
            for (int i = 0; i < fromList.Count - 1; i++)
            {
                if (fromList[i].item == null && fromList[i + 1].item != null)
                {
                    fromList[i] = fromList[i + 1];
                    fromList[i + 1] = new InventorySlot();
                }
            }
        }

        // Find empty slot in target list
        for (int i = 0; i < toList.Count; i++)
        {
            if (toList[i].item == null)
            {
                toList[i] = movingSlot;
                break;
            }
        }

        foreach (var slot in FindObjectsOfType<ItemSlot>())
        {
            slot.UpdateSlotUI();
        }
    }
}