using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 3f;      // ความเร็วในการเดินไล่
    public float detectionRange = 5f; // ระยะที่ Monster จะเริ่มมองเห็นเรา

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ค้นหา Object ที่มี Tag ว่า Player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        // คำนวณระยะห่างระหว่าง Monster กับ Player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            // ถ้าอยู่ในระยะ ให้เดินไล่
            MoveTowardsPlayer();
        }
        else
        {
            // ถ้าอยู่นอกระยะ ให้หยุดนิ่ง
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void MoveTowardsPlayer()
    {
        // คำนวณทิศทาง (ซ้าย หรือ ขวา)
        float direction = (player.position.x > transform.position.x) ? 1 : -1;

        // สั่งเคลื่อนที่
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        // กลับด้านรูปภาพตามทิศทางที่เดิน
        if (direction > 0) spriteRenderer.flipX = true; // หันขวา
        else spriteRenderer.flipX = false;             // หันซ้าย
    }

    // วาดวงกลมในหน้า Scene เพื่อดูระยะตรวจจับ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}