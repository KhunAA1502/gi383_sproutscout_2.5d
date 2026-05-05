using UnityEngine;

public class Bean : RangedWeapon
{
    private int ammo = 3;
    private float nextFireTime;

    // ฟังก์ชันนี้ PlayerCombat.cs จะเป็นคนเรียกใช้ (ตามรูป image_cdacf0.png บรรทัดที่ 85)
    public void SetupSentry(int healthFromData)
    {
        // ถ้าค่าที่ส่งมาเป็น 0 ให้ตั้งเป็น 3 นัดอัตโนมัติ
        ammo = (healthFromData > 0) ? healthFromData : 3;
        Debug.Log("<color=cyan>Bean Setup:</color> พร้อมยิง " + ammo + " นัด");
    }

    // แก้ไข Tick จากไฟล์แม่ ให้ทำงานแบบยิงสุ่มง่ายๆ
    public override void Tick()
    {
        if (!isPlaced) return; // ถ้ายังไม่วาง ไม่ต้องยิง

        if (Time.time >= nextFireTime && ammo > 0)
        {
            ShootSimple();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void ShootSimple()
    {
        if (projectilePrefab == null || throwPoint == null) return;

        // สุ่มทิศทางมั่วๆ
        float randomAngle = Random.Range(0f, 360f);
        Vector3 direction = Quaternion.Euler(0, randomAngle, 0) * Vector3.forward;

        // สร้างกระสุน
        GameObject projObj = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity);
        if (projObj.TryGetComponent(out Projectile proj))
        {
            proj.Launch(direction, 5f); // ยิงออกไปแรง 5
        }

        ammo--;
        Debug.Log($"<color=yellow>BANG!</color> เหลือกระสุน: {ammo}");

        if (ammo <= 0)
        {
            Debug.Log("<color=red>กระสุนหมด หายตัว!</color>");
            Destroy(gameObject, 0.1f);
        }
    }
}