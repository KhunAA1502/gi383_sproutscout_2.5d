using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour, IDamageable
{
    public System.Action OnDeath;

    [Header("Stat Settings")]
    public int maxHP = 100;
    protected int currentHP;

    [Header("Visual Flash Settings")]
    public SpriteRenderer characterSprite; // ลาก Sprite ของตัวละครมาใส่ตรงนี้
    public Color flashColor = Color.red;   // สีตอนโดนตี
    public float flashDuration = 0.15f;    // ระยะเวลากะพริบ (วินาที)

    private Color originalColor;
    private Coroutine flashCoroutine;

    protected virtual void Awake()
    {
        currentHP = maxHP;
        
        // ถ้าไม่ได้ลากใส่ใน Inspector ให้พยายามดึงจากเครื่องตัวเอง
        if (characterSprite == null) 
            characterSprite = GetComponent<SpriteRenderer>();

        // เก็บสีดั้งเดิมเอาไว้ (เพื่อเปลี่ยนกลับ)
        if (characterSprite != null)
            originalColor = characterSprite.color;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"{gameObject.name} รับดาเมจ: {damage} เลือดเหลือ: {currentHP}");

        // เริ่มการกะพริบสี
        if (characterSprite != null)
        {
            // ถ้ากำลังกะพริบอันเก่าอยู่ ให้หยุดก่อนแล้วเริ่มใหม่
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashRoutine());
        }

        if (currentHP <= 0) Die();
    }

    IEnumerator FlashRoutine()
    {
        // 1. เปลี่ยนเป็นสีแดง (หรือสีที่ตั้งไว้)
        characterSprite.color = flashColor;
        
        // 2. รอตามเวลาที่กำหนด
        yield return new WaitForSeconds(flashDuration);
        
        // 3. เปลี่ยนกลับเป็นสีเดิม
        characterSprite.color = originalColor;
        
        flashCoroutine = null;
    }

    public int GetCurrentHP() => currentHP;

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} ตายแล้ว");
       
        gameObject.SetActive(false);

        OnDeath?.Invoke();

    }
}