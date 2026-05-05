using UnityEngine;

public class RangedWeapon : Weapon
{
    [Header("Prefabs & Visuals")]
    public GameObject projectilePrefab;
    public GameObject previewPrefab;
    public LineRenderer line;
    public Transform throwPoint;

    [Header("Sentry Settings")]
    public float detectRange = 10f;
    public float fireRate = 1.0f; // ปรับให้ช้าลงนิดนึงเพื่อให้อ่าน Log ทัน
    public LayerMask groundLayer;

    protected bool isPlaced = false;
    protected bool isCharging = false;
    protected GameObject currentPreview;
    private float nextFireTime;

    public override void StartUse()
    {
        if (isPlaced) return;
        isCharging = true;
        if (previewPrefab != null && currentPreview == null)
        {
            currentPreview = Instantiate(previewPrefab, transform.position, Quaternion.identity);
        }
    }

    public override void ReleaseUse()
    {
        isCharging = false;
        if (currentPreview != null) Destroy(currentPreview);
        if (line != null) line.positionCount = 0;
    }

    public override void ActivateAutoFire()
    {
        isPlaced = true;
        Debug.Log("<color=green>Ranged Weapon:</color> เริ่มระบบยิงสุ่ม!"); 
    }

    public override void Tick()
    {
        if (isPlaced)
        {
            HandleRandomFire();
            return;
        }

        if (isCharging)
        {
            UpdateAimPreview();
        }
    }

    private void HandleRandomFire()
    {
        if (Time.time >= nextFireTime)
        {
            // สุ่มทิศทางมั่วๆ 360 องศา[cite: 1]
            float randomAngle = Random.Range(0f, 360f);
            Vector3 randomDirection = Quaternion.Euler(0, randomAngle, 0) * Vector3.forward;
            float randomDistance = Random.Range(2f, detectRange);

            FireProjectile(randomDirection, randomDistance);
            nextFireTime = Time.time + fireRate;
        }
    }

    protected virtual void FireProjectile(Vector3 direction, float distance)
    {
        if (projectilePrefab == null || throwPoint == null) return;

        GameObject projObj = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity);
        if (projObj.TryGetComponent(out Projectile proj))
        {
            proj.Launch(direction, distance); 
        }
    }

    private void UpdateAimPreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            if (currentPreview != null) currentPreview.transform.position = hit.point;
        }
    }
}