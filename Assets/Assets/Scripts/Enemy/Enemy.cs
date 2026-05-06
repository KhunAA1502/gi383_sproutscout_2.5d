using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class Enemy : Character, IDamageable
{
    public enum State { Chasing, Attacking, Cooldown }
    public State currentState = State.Chasing;

    [Header("Attack Settings")]
    public int damageAmount = 10;
    public float attackRange = 1.5f;     // ระยะที่สามารถโจมตีได้
    public float attackRate = 1.0f;      // โจมตีทุกๆกี่วินาที
    private float nextAttackTime = 0f;   // ตัวจับเวลา Cooldown

    private EnemyMovement movement;
    private Transform target;

    protected override void Awake()
    {
        base.Awake();
        movement = GetComponent<EnemyMovement>();

        // หาเป้าหมาย (Player)
        if (PlayerController.instance != null)
        {
            target = PlayerController.instance.transform;
        }
    }

    void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // ระบบ State Machine อย่างง่าย
        if (distanceToTarget <= attackRange)
        {
            // อยู่ในระยะโจมตี
            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackRate;
            }
        }
        else
        {
            // อยู่นอกระยะ ให้เดินตาม
            movement.MoveTowards(target.position);
        }
    }

    private void Attack()
    {
        Debug.Log($"<color=orange>Enemy Attacking Player for {damageAmount} damage!</color>");

        // ส่ง Damage ไปที่ Player โดยใช้ Interface IDamageable
        IDamageable playerDamageable = target.GetComponent<IDamageable>();
        if (playerDamageable != null)
        {
            playerDamageable.TakeDamage(damageAmount);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"<color=red><b>HIT!</b></color> {gameObject.name} HP: {currentHP}");

        if (currentHP <= 0) Die();
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} DEAD!");
        Destroy(gameObject);
    }
}