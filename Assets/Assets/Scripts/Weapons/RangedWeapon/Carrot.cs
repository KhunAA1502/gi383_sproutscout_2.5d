using UnityEngine;

public class Carrot : RangedWeapon
{
    private int ammo = 5; // Carrot has more ammo than Bean
    private float nextFireTime;

    public void SetupSentry(int healthFromData)
    {
        // Add any specific setup for Carrot here if needed
        Debug.Log($"<color=orange>Carrot Sentry Setup!</color> HP: {healthFromData}");
    }

    public override void Tick()
    {
        base.Tick();

        if (isPlaced && Time.time >= nextFireTime)
        {
            Transform target = FindClosestEnemy();
            if (target != null)
            {
                ShootAtTarget(target);
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    private Transform FindClosestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRange);
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
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

        Vector3 direction = (target.position - throwPoint.position).normalized;
        GameObject projObj = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity);
        
        if (projObj.TryGetComponent(out Projectile proj))
        {
            float distance = Vector3.Distance(throwPoint.position, target.position);
            proj.Launch(direction, Mathf.Clamp(distance, 5f, detectRange));
        }

        ammo--;
        Debug.Log($"<color=orange>Carrot Fire!</color> Target: {target.name} | Ammo left: {ammo}");

        if (ammo <= 0)
        {
            Debug.Log("<color=red>Carrot: ammo empty!</color>");
            Destroy(gameObject, 0.5f);
        }
    }
}
