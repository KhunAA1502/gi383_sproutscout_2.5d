using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class Enemy : Character, IDamageable
{
    
    [Header("Drop Settings")]
    public GameObject seedDropPrefab; 
    public SeedDatabase seedDatabase;

    public enum State { Chasing, Attacking, Cooldown }
    public State currentState = State.Chasing;

    [Header("Attack Settings")]
    public int damageAmount = 10;
    public float attackRange = 1.5f;     // ระยะที่สามารถโจมตีได้
    public float attackRate = 1.0f;      // โจมตีทุกๆกี่วินาที
    private float nextAttackTime = 0f;   // ตัวจับเวลา Cooldown

    [Header("Hit Audio")]
    public AudioClip hitSfx;
    [Range(0f, 1f)] public float hitVolume = 1f;
    private AudioSource audioSource;

    private EnemyMovement movement;
    private Transform target;

    protected override void Awake()
    {
        base.Awake();
        movement = GetComponent<EnemyMovement>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }

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

        // ใน Update() ของ Enemy.cs
        if (distanceToTarget <= attackRange)
        {
            // อยู่ในระยะโจมตี -> สั่งหยุดเดิน
            movement.Stop();

            if (Time.time >= nextAttackTime)
            {
                if (Time.time >= nextAttackTime)
                {
                    Attack();
                    nextAttackTime = Time.time + attackRate;
                }
            }
        }
        else
        {
            // นอกระยะโจมตี -> ให้เดินต่อ
            movement.Resume();
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

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (audioSource != null && hitSfx != null)
        {
            audioSource.PlayOneShot(hitSfx, hitVolume);
        }
        Debug.Log($"<color=red><b>HIT!</b></color> {gameObject.name} HP: {currentHP}");
    }

    protected override void Die()
    {
        Debug.Log($"<color=yellow>{gameObject.name} DEAD! Dropping seeds...</color>");
        DropItems();
        
        // เรียก base.Die() เพื่อส่งสัญญาณ OnDeath ให้ WaveManager รู้
        base.Die();
        
        // ทำลาย Object หลังจากแจ้งเตือนความตายแล้ว
        Destroy(gameObject, 0.1f);
    }

    public void DropItems() // เรียกฟังก์ชันนี้ตอนตาย
    {
        if (seedDatabase == null || seedDropPrefab == null) return;

        int seedCount = Random.Range(1, 2);
        for (int i = 0; i < seedCount; i++)
        {
            ItemData randomSeed = seedDatabase.GetRandomSeed();
            if (randomSeed == null) continue;

            // สุ่มตำแหน่งรอบๆ ตัวศัตรู
            Vector3 dropPos = transform.position + (Random.insideUnitSphere * 0.7f);
            dropPos.y = transform.position.y; // ให้ตกลงมาอยู่ที่ระดับพื้นเท่าศัตรู

            // --- ใช้การ Instantiate Prefab แทนการสร้างใหม่จากศูนย์ ---
            GameObject dropItem = Instantiate(seedDropPrefab, dropPos, Quaternion.identity);

            // ส่งข้อมูล ItemData เข้าไปในสคริปต์ PickupItem ที่ติดมากับ Prefab
            if (dropItem.TryGetComponent(out PickupItem pickup))
            {
                pickup.itemData = randomSeed;
                pickup.amount = 1;
            }

            // ถ้าอยากให้ชื่อมันเปลี่ยนตามเมล็ด (ไว้ดูใน Hierarchy)
            dropItem.name = "SeedDrop_" + randomSeed.itemName;
        }
    }
}