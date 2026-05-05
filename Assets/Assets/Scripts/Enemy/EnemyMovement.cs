using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public SpriteRenderer enemySprite;

    // ฟังก์ชันสำหรับสั่งให้เดินไปที่จุดเป้าหมาย
    public void MoveTowards(Vector3 targetPosition)
    {
        // คำนวณทิศทาง
        Vector3 direction = (targetPosition - transform.position).normalized;
        
        // เคลื่อนที่ตัวละคร
        transform.position += direction * moveSpeed * Time.deltaTime;

        // หันหน้าซ้าย-ขวาตามทิศทางที่เดิน
        if (direction.x > 0) 
            enemySprite.flipX = false;
        else if (direction.x < 0) 
            enemySprite.flipX = true;
    }

    // ฟังก์ชันสำหรับสั่งหยุดเดิน (เผื่อใช้ในอนาคต)
    public void Stop()
    {
        // Rigidbody2D rb = GetComponent<Rigidbody2D>();
        // rb.linearVelocity = Vector2.zero; 
    }
}