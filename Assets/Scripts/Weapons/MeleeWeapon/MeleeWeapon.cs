using UnityEngine;
using System.Collections;
using System.Collections.Generic; // ต้องมีตัวนี้เพื่อใช้ List

public class MeleeWeapon : Weapon
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private int damage = 10;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 25f;
    [SerializeField] private float dashDuration = 0.15f;

    private float timer;
    public Rigidbody2D playerRb;
    public bool isDashing = false;
    public bool IsDashing => isDashing;
    private Collider2D attackCollider;
    

    // รายชื่อศัตรูที่โดนโจมตีไปแล้วในการพุ่งครั้งนี้
    private List<IDamageable> hitTargets = new List<IDamageable>();

    void Awake()
    {
        playerRb = GetComponentInParent<Rigidbody2D>();
        attackCollider = GetComponent<Collider2D>();
        
        if (attackCollider != null) attackCollider.enabled = false;
    }

    public override void Tick()
    {
        timer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && timer >= attackCooldown && !isDashing)
        {
            timer = 0f;
            StartCoroutine(DashAttackRoutine());
            Debug.Log("Click Detected! Timer: " + timer);
        }
    }

  IEnumerator DashAttackRoutine()
{
    isDashing = true;
    hitTargets.Clear();

    // 1. ปิดการชนระหว่าง Player กับ Enemy ชั่วคราว (สมมติว่าใช้ Layer 6 และ 7)
    // หรือระบุชื่อ Layer ของคุณ เช่น LayerMask.NameToLayer("Player")
    int playerLayer = playerRb.gameObject.layer;
    int enemyLayer = LayerMask.NameToLayer("Enemy"); 
    Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

    // --- ส่วนการหาทิศทางและใส่แรงพุ่ง (โค้ดเดิมของคุณ) ---
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mousePos.z = 0;
    Vector2 dashDirection = (mousePos - playerRb.transform.position).normalized;

    if (attackCollider != null) attackCollider.enabled = true;
    if (playerRb != null)
    {
        playerRb.linearVelocity = Vector2.zero; 
        playerRb.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);
    }

    yield return new WaitForSeconds(dashDuration);
    
    // 2. เปิดการชนกลับมาเหมือนเดิมเมื่อพุ่งเสร็จ
    Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

    if (playerRb != null) playerRb.linearVelocity = Vector2.zero;
    if (attackCollider != null) attackCollider.enabled = false;
    isDashing = false;
}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDashing)
        {
            IDamageable target = collision.GetComponent<IDamageable>();
            
            // ตรวจเช็คว่า:
            // 1. เป็นศัตรูที่รับดาเมจได้ไหม
            // 2. ศัตรูตัวนี้ "ยังไม่เคยโดนแทง" ในการพุ่งรอบนี้ใช่ไหม
            if (target != null && !hitTargets.Contains(target))
            {
                target.TakeDamage(damage); // ทำดาเมจ
                hitTargets.Add(target);    // เพิ่มชื่อลงใน List ว่าโดนไปแล้วนะ
                
                Debug.Log("แทงโดน: " + collision.name);
            }
        }
    }
}