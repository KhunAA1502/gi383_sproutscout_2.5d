using UnityEngine;
using TMPro;

public class PlayerController : Character // สืบทอดจาก Character
{
    [Header("Player Specific Components")]
    [SerializeField] private TextMeshProUGUI healthText;
    private PlayerMovement movement;
    private MeleeWeapon meleeWeapon;

    public static PlayerController instance;

    protected override void Awake()
    {
        // --- ส่วนที่เพิ่มเพื่อให้อยู่ข้ามฉาก ---
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ทำให้ตัวละครไม่หายไปเมื่อข้ามฉาก
        }
        else
        {
            Destroy(gameObject); // ถ้ามีตัวละครอยู่แล้ว ให้ลบตัวที่สร้างใหม่ในฉากนี้ทิ้ง
            return;
        }
        // ----------------------------------

        base.Awake(); // เก็บสี Sprite และตั้งค่าเลือด[cite: 2]
        movement = GetComponent<PlayerMovement>();
        meleeWeapon = GetComponentInChildren<MeleeWeapon>();
    }

    private void Start()
    {
        UpdateHealthUI();
    }

    private void Update()
    {
        if (instance != this) return; // ป้องกัน Logic ทำงานซ้อนถ้ามีตัวปลอม

        bool isDashing = (meleeWeapon != null && meleeWeapon.IsDashing);
        if (isDashing)
        {
            movement.StopVelocity();
            return;
        }

        movement.HandleInput();
        movement.Animate();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        UpdateHealthUI(); // อัปเดตเลือดบนจอ[cite: 2]
    }

    public void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"HP: {currentHP} / {maxHP}";
        }
    }

    protected override void Die()
    {
        Debug.Log("Player Game Over!");
        base.Die();
    }
}