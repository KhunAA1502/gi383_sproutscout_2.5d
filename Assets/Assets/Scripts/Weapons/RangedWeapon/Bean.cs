using UnityEngine;

public class Bean : RangedWeapon
{
    private int ammo = 3;

    // ฟังก์ชันที่ PlayerCombat.cs จะเป็นคนเรียกใช้
    public void SetupSentry(int healthFromData)
    {
        // กำหนดจำนวนกระสุนตามเลือดของพืช
        ammo = (healthFromData > 0) ? healthFromData : 3;
        Debug.Log("<color=cyan>Bean Setup:</color> จำนวนกระสุน " + ammo + " นัด");
    }

    public override void Tick()
    {
        if (!isPlaced) return; // ถ้ายังไม่วาง ไม่ต้องยิง

        // --- ส่วนที่เพิ่ม: บังคับล็อคตำแหน่ง Y ให้คงที่เสมอ ป้องกันการลอยจาก Animation ---
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);

        if (Time.time >= nextFireTime && ammo > 0)
        {
            // ค้นหาศัตรูในระยะ
            Transform target = FindNearestEnemy();

            if (target != null)
            {
                ShootAtTarget(target);
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    private Transform FindNearestEnemy()
    {
        // ค้นหา Collider ทั้งหมดในระยะ detectRange
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectRange);
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var hit in hitColliders)
        {
            // ตรวจสอบว่ามี Tag "Enemy" หรือไม่
            if (hit.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEnemy = hit.transform;
                }
            }
        }
        return closestEnemy;
    }

    private void ShootAtTarget(Transform target)
    {
        if (projectilePrefab == null || throwPoint == null) return;

        // คำนวณทิศทางไปยังเป้าหมาย
        Vector3 direction = (target.position - throwPoint.position).normalized;
        // ให้กระสุนลอยขึ้นนิดนึง
        direction.y = 0.2f; 

        // สร้างกระสุน
        GameObject projObj = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity);
        if (projObj.TryGetComponent(out Projectile proj))
        {
            // ยิงไปยังทิศทางศัตรู
            float distance = Vector3.Distance(throwPoint.position, target.position);
            proj.Launch(direction, Mathf.Clamp(distance, 5f, detectRange));
            PlayShootSfx();
        }

        ammo--;
        Debug.Log($"<color=green>Sentry Fire!</color> เป้าหมาย: {target.name} | กระสุนเหลือ: {ammo}");

        if (ammo <= 0)
        {
            Debug.Log("<color=red>Bean: กระสุนหมดแล้ว!</color>");
            Destroy(gameObject, 0.5f);
        }
    }
}