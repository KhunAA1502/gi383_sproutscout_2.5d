using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float explodeDelay = 2f;

    private Rigidbody2D rb;
    private bool hasLanded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!hasLanded && rb.linearVelocity.magnitude < 0.1f)
        {
            hasLanded = true;

            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;

            Invoke(nameof(Explode), explodeDelay);
        }
    }

    void Explode()
    {
        Debug.Log("BOOM 💥");
        Destroy(gameObject);
    }
}