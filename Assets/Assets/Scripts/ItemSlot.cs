using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public int slotIndex;
    public bool isHotbar;
    public Image iconImage; // อ้างอิง Object "Icon" ของช่อง
    public TMPro.TextMeshProUGUI countText; // อ้างอิง CountText ใน prefab
    public InventorySlot currentSlot;

    private Canvas parentCanvas;
    private Graphic slotGraphic;
    private static ItemSlot dragSourceSlot;
    private static GameObject dragIconObject;
    private static Image dragIconImage;
    private static RectTransform dragIconRect;
    private Vector2 dragOffset;

    private ItemData currentItem => currentSlot?.item;

    private void Start()
    {
        slotGraphic = GetComponent<Graphic>();
        if (slotGraphic == null)
        {
            var addedImage = gameObject.AddComponent<Image>();
            addedImage.color = new Color(1f, 1f, 1f, 0f);
            addedImage.raycastTarget = true;
            slotGraphic = addedImage;
        }
        else
        {
            slotGraphic.raycastTarget = true;
        }

        if (iconImage != null && iconImage.gameObject != gameObject)
        {
            iconImage.raycastTarget = false;
        }

        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
            parentCanvas = FindObjectOfType<Canvas>();

        Debug.Log($"[ItemSlot] Start slot={slotIndex} hotbar={isHotbar} canvas={(parentCanvas!=null)} item={(currentItem!=null ? currentItem.itemName : "null")} slotGraphic={(slotGraphic!=null)}");
        UpdateSlotUI();
        EnsureDragIcon();
    }

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

            // แสดงจำนวนถ้ามากกว่า 1 หรือเป็น 0 (แสดงจำนวน)
            if (currentSlot.amount > 1)
            {
                countText.text = currentSlot.amount.ToString();
                countText.enabled = true;
            }
            else if (currentSlot.amount == 1)
            {
                countText.text = ""; // ไม่แสดงถ้ามีแค่ 1
                countText.enabled = false;
            }
            else
            {
                countText.text = "";
                countText.enabled = false;
            }
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            countText.text = "";
            countText.enabled = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null) return;

        // --- คลิกซ้าย: เพื่อใช้งานหรือเลือกติดตั้งไอเท็ม ---
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // ถ้าเป็นเมล็ด ให้เริ่มโหมดปลูก
            if (currentItem.itemType == ItemType.Seed)
            {
                PlayerFarming playerFarming = FindFirstObjectByType<PlayerFarming>();
                if (playerFarming != null)
                {
                    playerFarming.EquipSeed(currentItem, slotIndex);
                    Debug.Log($"[ItemSlot] Started planting mode for {currentItem.itemName}");
                }
                return;
            }

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

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null)
        {
            Debug.Log($"[ItemSlot] OnBeginDrag skipped: no item in slot {slotIndex} hotbar={isHotbar}");
            return;
        }
        if (parentCanvas == null)
        {
            Debug.Log($"[ItemSlot] OnBeginDrag skipped: no canvas found for slot {slotIndex} hotbar={isHotbar}");
            return;
        }

        dragSourceSlot = this;
        EnsureDragIcon();

        Debug.Log($"[ItemSlot] OnBeginDrag slot={slotIndex} hotbar={isHotbar} item={currentItem.itemName}");
        dragIconImage.sprite = currentItem.icon;
        dragIconImage.enabled = true;
        dragIconObject.SetActive(true);

        dragIconRect.sizeDelta = iconImage.rectTransform.sizeDelta;

        Vector2 localPointer;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : eventData.pressEventCamera,
            out localPointer);

        dragOffset = Vector2.zero;
        dragIconRect.anchoredPosition = localPointer;

        iconImage.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragSourceSlot != this || dragIconObject == null)
        {
            Debug.Log($"[ItemSlot] OnDrag ignored: source={(dragSourceSlot!=null ? dragSourceSlot.slotIndex.ToString() : "null")}, target={slotIndex}");
            return;
        }
        UpdateDragIconPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"[ItemSlot] OnEndDrag slot={slotIndex} hotbar={isHotbar}");
        if (dragIconObject != null)
        {
            dragIconObject.SetActive(false);
            dragIconImage.enabled = false;
        }

        if (dragSourceSlot != null)
        {
            dragSourceSlot.UpdateSlotUI();
            dragSourceSlot = null;
        }

        UpdateSlotUI();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (dragSourceSlot == null)
        {
            Debug.Log($"[ItemSlot] OnDrop ignored: no drag source for slot {slotIndex}");
            return;
        }
        if (dragSourceSlot == this)
        {
            Debug.Log($"[ItemSlot] OnDrop ignored: dropped on same slot {slotIndex}");
            return;
        }
        if (currentSlot == null)
        {
            Debug.Log($"[ItemSlot] OnDrop ignored: target slot {slotIndex} has no InventorySlot");
            return;
        }

        Debug.Log($"[ItemSlot] OnDrop from slot {dragSourceSlot.slotIndex} hotbar={dragSourceSlot.isHotbar} to slot {slotIndex} hotbar={isHotbar}");
        SwapItems(dragSourceSlot, this);
        foreach (var slot in FindObjectsByType<ItemSlot>(FindObjectsSortMode.None))
        {
            slot.UpdateSlotUI();
        }
    }

    private void SwapItems(ItemSlot source, ItemSlot target)
    {
        if (source.currentSlot == null || target.currentSlot == null) return;

        ItemData sourceItem = source.currentSlot.item;
        int sourceAmount = source.currentSlot.amount;

        source.currentSlot.item = target.currentSlot.item;
        source.currentSlot.amount = target.currentSlot.amount;

        target.currentSlot.item = sourceItem;
        target.currentSlot.amount = sourceAmount;
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

    private void EnsureDragIcon()
    {
        if (dragIconObject != null || parentCanvas == null) return;

        dragIconObject = new GameObject("DragIcon", typeof(RectTransform), typeof(CanvasGroup));
        dragIconObject.transform.SetParent(parentCanvas.transform, false);
        dragIconObject.transform.SetAsLastSibling();
        dragIconRect = dragIconObject.GetComponent<RectTransform>();
        dragIconRect.pivot = new Vector2(0.5f, 0.5f);
        dragIconRect.sizeDelta = iconImage != null ? iconImage.rectTransform.sizeDelta : new Vector2(32, 32);

        dragIconImage = dragIconObject.AddComponent<Image>();
        dragIconImage.raycastTarget = false;
        dragIconImage.enabled = false;

        CanvasGroup cg = dragIconObject.GetComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.interactable = false;

        dragIconObject.SetActive(false);
    }

    private void UpdateDragIconPosition(PointerEventData eventData)
    {
        if (dragIconRect == null || parentCanvas == null) return;

        Vector2 localPointer;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : eventData.pressEventCamera,
            out localPointer);

        dragIconRect.anchoredPosition = localPointer - dragOffset;
    }
}
