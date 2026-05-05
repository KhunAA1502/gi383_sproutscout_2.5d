using UnityEngine;
using TMPro;

public class PlayerController : Character // สืบทอดจาก Character ที่คุณเขียนมา
{
    [Header("Player Specific Components")]
    [SerializeField] private TextMeshProUGUI healthText;
    private PlayerMovement movement;
    private MeleeWeapon meleeWeapon;

    public static PlayerController instance;

    protected override void Awake()
    {
        instance = this;

        // เรียก Awake ของ Character เพื่อเซตเลือดและเก็บสี Sprite ดั้งเดิม
        base.Awake();

        movement = GetComponent<PlayerMovement>();
        meleeWeapon = GetComponentInChildren<MeleeWeapon>();
    }

    private void Start()
    {
        UpdateHealthUI();
        Debug.Log(currentHP);
    }

    private void Update()
    {
        // ตรวจสอบสถานะจากอาวุธ (ถ้ามี)
        bool isDashing = (meleeWeapon != null && meleeWeapon.IsDashing);

        if (isDashing)
        {
            movement.StopVelocity();
            return;
        }

        movement.HandleInput();
        movement.Animate();
    }

    // Override ฟังก์ชัน TakeDamage เพื่อเพิ่มการอัปเดต UI ของผู้เล่น
    public override void TakeDamage(int damage)
    {
        // เรียก Logic เดิมของ Character (ลดเลือด + กะพริบสี)
        base.TakeDamage(damage);

        // เพิ่ม Logic เฉพาะของ Player คือการโชว์เลือดบนจอ
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"HP: {currentHP} / {maxHP}";
        }
    }

    protected override void Die()
    {
        // ถ้าอยากให้ Player ตายแล้วมี Logic พิเศษ (เช่นเด้งหน้า Game Over) ให้ใส่ที่นี่
        Debug.Log("Player Game Over!");

        // แต่ยังคงเรียกใช้การปิด Object จาก Base Class อยู่
        base.Die();
    }
}