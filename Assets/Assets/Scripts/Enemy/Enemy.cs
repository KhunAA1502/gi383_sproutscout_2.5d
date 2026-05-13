using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class Enemy : Character, IDamageable
{
    [Header("Drop Settings")]
    public SeedDatabase seedDatabase; // ⚠️ ต้อง assign ใน Inspector

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
        Debug.Log($"<color=yellow>{gameObject.name} DEAD! Dropping seeds...</color>");
        DropRandomSeeds();
        Destroy(gameObject);
    }

    private void DropRandomSeeds()
    {
        if (seedDatabase == null)
        {
            Debug.LogWarning("[Enemy] seedDatabase ยังไม่ได้ assign!");
            return;
        }

        // Random เมล็ด 1-3 ชนิด
        int seedCount = Random.Range(1, 4);
        for (int i = 0; i < seedCount; i++)
        {
            ItemData randomSeed = seedDatabase.GetRandomSeed();
            if (randomSeed == null || randomSeed.itemType != ItemType.Seed) continue;

            // สร้าง drop item
            Vector3 dropPos = transform.position + Random.insideUnitSphere * 0.5f;
            GameObject dropItem = new GameObject($"SeedDrop_{randomSeed.itemName}");
            dropItem.transform.position = dropPos;

            // เพิ่ม PickupItem component
            PickupItem pickup = dropItem.AddComponent<PickupItem>();
            pickup.itemData = randomSeed;
            pickup.amount = 1;

            // เพิ่ม visual (Sphere ชั่วคราว)
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.transform.SetParent(dropItem.transform);
            visual.transform.localScale = Vector3.one * 0.3f;
            Destroy(visual.GetComponent<Collider>());

            Debug.Log($"<color=green>[Enemy] Dropped seed: {randomSeed.itemName}</color>");
        }
    }
}