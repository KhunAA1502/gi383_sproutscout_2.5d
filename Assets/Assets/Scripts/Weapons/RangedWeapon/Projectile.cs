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
        float upForce = distance * 0.5f; // คำนวณแรงยกให้กระสุนย้อย[cite: 2]
        Vector3 force = (direction * distance) + (Vector3.up * upForce);

        rb.AddForce(force, ForceMode.Impulse);
        transform.forward = force.normalized;
    }

    private void Update()
    {
        if (rb.velocity != Vector3.zero)
        {
            transform.forward = rb.velocity.normalized;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // เช็คการชนกับ Interface หรือ Tag ของศัตรู[cite: 2]
        if (other.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject); // ชนพื้นแล้วหายไป[cite: 2]
        }
    }
}