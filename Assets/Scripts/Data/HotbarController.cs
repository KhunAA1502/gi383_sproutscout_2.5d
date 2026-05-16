using UnityEngine;
using UnityEngine.EventSystems;

public class HotbarController : MonoBehaviour
{
    public static HotbarController instance;

    private ItemSlot[] hotbarSlots;
    private const int HOTBAR_SIZE = 8;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // หา ItemSlot ทั้งหมดใน hotbar
        FindHotbarSlots();
    }

    private void FindHotbarSlots()
    {
        ItemSlot[] allSlots = FindObjectsOfType<ItemSlot>();
        int hotbarCount = 0;

        // สร้าง array เก็บ hotbar slots เรียงตามลำดับ
        hotbarSlots = new ItemSlot[HOTBAR_SIZE];

        foreach (var slot in allSlots)
        {
            if (slot.isHotbar && slot.slotIndex < HOTBAR_SIZE)
            {
                hotbarSlots[slot.slotIndex] = slot;
                hotbarCount++;
                Debug.Log($"[HotbarController] Found hotbar slot {slot.slotIndex}");
            }
        }

        Debug.Log($"[HotbarController] Total hotbar slots found: {hotbarCount}");
    }

    public delegate void HotbarSlotSelectedHandler(int slotIndex, ItemSlot slot);
    public static event HotbarSlotSelectedHandler OnHotbarSlotSelected;

    private void Update()
    {
        // ตรวจสอบการกดปุ่ม 1-8 เพื่อเลือก hotbar slot
        HandleHotbarKeyInput();

        // ถ้าคลิกที่อื่น (ไม่ใช่ UI) และไม่ใช่โหมดปลูก → ปิด highlight
        //if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        //{
        //    PlayerFarming playerFarming = FindFirstObjectByType<PlayerFarming>();
        //    if (playerFarming == null || !playerFarming.IsPlantingSeed)
        //    {
        //        DeselectHotbar();
        //    }
        //}
    }

    private void HandleHotbarKeyInput()
    {
        // เลข 1 = hotbar slot 0
        // เลข 2 = hotbar slot 1
        // เป็นต้น...
        for (int i = 0; i < HOTBAR_SIZE; i++)
        {
            KeyCode keyToPress = (KeyCode)(KeyCode.Alpha1 + i);

            if (Input.GetKeyDown(keyToPress))
            {
                SelectHotbarSlot(i);
            }
        }
    }

    public void SelectHotbarSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= HOTBAR_SIZE)
        {
            Debug.LogWarning($"[HotbarController] Invalid hotbar slot index: {slotIndex}");
            return;
        }

        if (hotbarSlots == null || hotbarSlots[slotIndex] == null)
        {
            Debug.LogWarning($"[HotbarController] Hotbar slot {slotIndex} not found");
            return;
        }

        // เลือก slot
        ItemSlot selectedSlot = hotbarSlots[slotIndex];
        selectedSlot.SelectSlot();
        Debug.Log($"[HotbarController] Selected hotbar slot {slotIndex}");

        OnHotbarSlotSelected?.Invoke(slotIndex, selectedSlot);
    }

    public ItemSlot GetSelectedSlot()
    {
        return ItemSlot.GetSelectedHotbarSlot();
    }

    public void DeselectHotbar()
    {
        foreach (var slot in hotbarSlots)
        {
            if (slot != null) slot.DeselectSlot();
        }

        // เพิ่มส่วนนี้: สั่งให้ลบ Preview อาวุธและเมล็ดพันธุ์ในมือ
        PlayerCombat combat = FindFirstObjectByType<PlayerCombat>();
        if (combat != null) combat.UnequipItem();

        PlayerFarming farming = FindFirstObjectByType<PlayerFarming>();
        if (farming != null) farming.CancelPlanting();

        Debug.Log("[HotbarController] All slots deselected and hand cleared.");
    }
}
