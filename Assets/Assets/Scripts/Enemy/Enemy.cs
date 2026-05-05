using UnityEngine;

// ต้องสืบทอด IDamageable เพื่อให้ระบบดาเมจทำงาน[cite: 4]
[RequireComponent(typeof(EnemyMovement))]
public class Enemy : Character, IDamageable
{
    public enum State { Idle, Chasing, Attacking }
    [Header("Target Settings")]
    public Transform target;

    private EnemyMovement movement;

    protected override void Awake()
    {
        base.Awake();
        movement = GetComponent<EnemyMovement>(); 

        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform; 
            }
        }
    }

    // ฟังก์ชันรับดาเมจและแสดงผลเลือด[cite: 4]
    public void TakeDamage(int damage)
    {
        currentHP -= damage; // currentHealth มาจาก Character

        // แสดง Log สีแดงเพื่อให้เห็นชัดเจนใน Console
        Debug.Log($"<color=red><b>HIT!</b></color> {gameObject.name} โดนดาเมจ: {damage} | เลือดปัจจุบัน: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"<color=black><b>{gameObject.name} DEAD!</b></color>");
        Destroy(gameObject); 
    }

    void Update()
    {
        // ตรวจสอบว่า Player ยังอยู่ไหมก่อนเดิน
        if (PlayerController.instance != null && PlayerController.instance.gameObject.activeInHierarchy)
        {
            movement.MoveTowards(PlayerController.instance.transform.position); 
        }
    }
}