using UnityEngine;
using TMPro;

public class PlayerController : Character // สืบทอดจาก Character
{
    [Header("Player Specific Components")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Transform safeZoneSpawnPoint; // จุดเกิดที่ SafeZone

    private PlayerMovement movement;
    private MeleeWeapon meleeWeapon;

    public static PlayerController instance;

    protected override void Awake()
    {
        // --- ส่วนที่ทำให้ตัวละครอยู่ข้ามฉาก ---[cite: 1]
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        // ----------------------------------

        base.Awake(); // เรียกใช้การตั้งค่าจากคลาส Character[cite: 1]
        movement = GetComponent<PlayerMovement>();
        meleeWeapon = GetComponentInChildren<MeleeWeapon>();
    }

    private void Start()
    {
        UpdateHealthUI(); 
    }

    private void Update()
    {
        if (instance != this) return;

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
        base.TakeDamage(damage); // หักเลือดพื้นฐาน[cite: 1]
        UpdateHealthUI(); // อัปเดตเลือดบนจอ[cite: 1]
    }

    public void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"HP: {currentHP} / {maxHP}"; 
        }
    }

    // ฟังก์ชันสำหรับตั้งค่าจุดเกิดใหม่ (กรณีเปลี่ยนฉากแล้ว SafeZone เปลี่ยนที่)
    public void SetRespawnPoint(Transform newPoint)
    {
        safeZoneSpawnPoint = newPoint; 
    }

    protected override void Die()
    {
        Debug.Log("<color=red>Player Game Over! Respawning...</color>");

        // 1. คืนค่าเลือดให้เต็มเพื่อให้เล่นต่อได้[cite: 1]
        currentHP = maxHP;
        UpdateHealthUI();

        // 2. ย้ายตำแหน่งตัวละครไปที่จุด SafeZone
        if (safeZoneSpawnPoint != null)
        {
            transform.position = safeZoneSpawnPoint.position;
            transform.rotation = safeZoneSpawnPoint.rotation;
        }
        else
        {
            Debug.LogWarning("ไม่ได้ตั้งค่า SafeZoneSpawnPoint! ตัวละครจะกลับไปที่จุด 0,0,0");
            transform.position = Vector3.zero;
        }

        // 3. หยุดแรงส่งจากการเคลื่อนที่ค้างไว้
        if (movement != null)
        {
            movement.StopVelocity();
        }
    }
}