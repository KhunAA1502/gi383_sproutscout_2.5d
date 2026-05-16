using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public int damage = 10;
    private Rigidbody rb;

    void Awake() => rb = GetComponent<Rigidbody>();

    public void Launch(Vector3 direction, float distance)
    {
        rb.useGravity = true;
        float upForce = distance * 0.8f; // �ӹǳ�ç¡������ع����[cite: 2]
        Vector3 force = (direction * distance) + (Vector3.up * upForce);

        rb.AddForce(force, ForceMode.Impulse);
        transform.forward = force.normalized;
    }

    private void Update()
    {
        if (rb.linearVelocity != Vector3.zero)
        {
            transform.forward = rb.linearVelocity.normalized;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. เช็คว่าเป็น Enemy หรือไม่ (ใช้ Tag เพื่อแยกแยะจาก Player)
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        // 2. ถ้าชนพื้น ให้ทำลายทิ้ง
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }

    }
