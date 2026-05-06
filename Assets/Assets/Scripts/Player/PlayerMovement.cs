using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private float currentSpeed;
    private Rigidbody rb;
    private Vector3 input;
    private Vector3 lastMoveDirection;
    public Animator animator;
    public SpriteRenderer sr;

    void Awake() => rb = GetComponent<Rigidbody>();

    public void HandleInput()
    {
        // รับค่าแกนนอน (A/D) และแกนตั้ง (W/S)
        input.x = Input.GetAxisRaw("Horizontal");
        input.z = Input.GetAxisRaw("Vertical"); // เก็บค่า W/S ไว้ในแกน Z ของ 3D
        
        if (input.sqrMagnitude > 1) input.Normalize();

        if (input.x > 0) sr.flipX = false;
        else if (input.x < 0) sr.flipX = true;

        if (input != Vector3.zero) lastMoveDirection = input;
    }

    void FixedUpdate()
    {
        // คำนวณความเร็วรวมเพื่อส่งให้ Animator
        currentSpeed = new Vector2(input.x, input.z).magnitude * moveSpeed;

        // ใช้ Rigidbody ควบคุมการเคลื่อนที่ทั้งแกน X และ Z
        // ส่วนแกน Y ให้ใช้ความเร็วเดิมของ Rigidbody (เผื่อกรณีมีแรงโน้มถ่วงหรือการตกจากที่สูง)
        rb.linearVelocity = new Vector3(input.x * moveSpeed, rb.linearVelocity.y, input.z * moveSpeed);
    }
    
    public void Animate()
    {
        if (animator == null) return;
        animator.SetFloat("MoveX", lastMoveDirection.x);
        animator.SetFloat("MoveZ", lastMoveDirection.z);
        animator.SetFloat("Speed", currentSpeed);
    }

    public void StopVelocity()
    {
        input = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
    }
}