using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public EnemyData data;
    public Transform attackPoint;
    public SpriteRenderer indicator;

    private bool isAttacking = false;
    public bool IsAttacking => isAttacking; // ให้ตัวอื่นอ่านค่าได้แต่แก้ไม่ได้

    public bool IsInRange(Transform target)
    {
        float distance = Vector3.Distance(attackPoint.position, target.position);
        return distance <= data.attackRange;
    }

    public void StartAttack(Transform target)
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackRoutine(target));
        }
    }

    IEnumerator AttackRoutine(Transform target)
    {
        isAttacking = true;

        // 1. แสดง Indicator (กะพริบแจ้งเตือน)
        if (indicator != null)
        {
            indicator.enabled = true;
            yield return new WaitForSeconds(0.5f); // รอให้ผู้เล่นหลบ
            indicator.enabled = false;
        }

        // 2. ตรวจสอบดาเมจตอนจังหวะสุดท้าย
        if (target != null && IsInRange(target))
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            damageable?.TakeDamage(data.damage);
        }

        yield return new WaitForSeconds(data.attackCooldown);
        isAttacking = false;
    }
}