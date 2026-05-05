using UnityEngine;

public class BombArc : MonoBehaviour
{
    [Header("Arc Settings")]
    public float launchForce = 5f;
    public float arcHeight = 3f;
    public float gravity = 9f;

    private Vector2 moveDirection;
    private float height;
    private float verticalVelocity;
    private bool hasLanded = false;

    [SerializeField] public Transform visual;

    void Start()
    {
        // ความสูงเริ่มต้น
        height = 0;
        verticalVelocity = arcHeight;

        // ยิงไปข้างหน้า
        moveDirection = transform.right;
    }

    void Update()
    {
        if (hasLanded) return;

        // เคลื่อนที่บนพื้น (X,Y)
        transform.position += (Vector3)(moveDirection * launchForce * Time.deltaTime);

        // คำนวณความสูงปลอม
        verticalVelocity -= gravity * Time.deltaTime;
        height += verticalVelocity * Time.deltaTime;

        // ขยับ sprite ให้ดูเหมือนลอย
        visual.localPosition = new Vector3(0, height, 0);

        if (height <= 0)
        {
            height = 0;
            hasLanded = true;
            OnLand();
        }
    }

    void OnLand()
    {
        Debug.Log("ตกพื้นแล้ว 💣");

        Invoke(nameof(Explode), 2f);
    }

    void Explode()
    {
        Debug.Log("BOOM 💥");
        Destroy(gameObject);
    }
}