using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private int damage = 10;     // เพิ่ม: พลังโจมตี
    [SerializeField] private float lifeTime = 3f; // เพิ่ม: เวลาก่อนทำลายตัวเองถ้าไม่โดนอะไรเลย

    private Rigidbody2D rb;
    private Vector2 startPos;
    private float targetDistance;
    private bool isMoving = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // เริ่มนับถอยหลังทำลายตัวเองทันทีที่เกิดมา (กรณีไม่ชนอะไรเลย)
        Invoke("DestroyObject", lifeTime);
    }

    public void Launch(Vector2 direction, float distance)
    {
        startPos = transform.position;
        targetDistance = distance;
        isMoving = true;

        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    void Update()
    {
        if (isMoving)
        {
            float traveled = Vector2.Distance(startPos, transform.position);
            if (traveled >= targetDistance)
            {
                StopObject();
            }
        }
    }

    // ฟังก์ชันตรวจจับการชน
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // เช็คว่าสิ่งที่ชนคือศัตรูหรือไม่ (ตรวจสอบผ่าน Component IDamageable หรือ Tag)
        IDamageable enemy = collision.GetComponent<IDamageable>();

        if (enemy != null)
        {
            // ทำดาเมจใส่ศัตรู
            enemy.TakeDamage(damage);
            
            // เมื่อโดนศัตรูแล้ว ให้ทำลายตัวเองทันที
            CancelInvoke("DestroyObject"); // ยกเลิกการลบตามเวลาเดิม
            DestroyObject();
        }
    }

    private void StopObject()
    {
        isMoving = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        
        // ถ้าถึงระยะเป้าหมายแล้วยังไม่ชนใคร ให้รออีกนิดค่อยหายไป (หรือจะให้หายทันทีก็ได้)
        Invoke("DestroyObject", 1f); 
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}