using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    // ตัวแปรที่เก็บข้อมูลว่าไอเทมชิ้นนี้คืออะไร
    public ItemData itemData;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // เมื่อเริ่มเกม ให้เปลี่ยนรูปภาพตาม Icon ใน ItemData
        if (itemData != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = itemData.icon;
        }
    }
}