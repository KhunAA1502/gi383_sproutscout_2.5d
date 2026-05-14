using UnityEngine;

public class RangedWeapon : Weapon
{
    [Header("Prefabs & Visuals")]
    public GameObject projectilePrefab;
    public GameObject previewPrefab;
    public Transform throwPoint;

    [Header("Sentry Settings")]
    public float detectRange = 10f;
    public float fireRate = 1.0f; 
    public LayerMask groundLayer;

    protected bool isPlaced = false;
    protected bool isCharging = false;
    protected GameObject currentPreview;
    protected float fixedY; 
    protected float nextFireTime;

    public override void StartUse()
    {
        if (isPlaced) return;
        isCharging = true;
        
        if (previewPrefab != null && currentPreview == null)
        {
            currentPreview = Instantiate(previewPrefab, transform.position, Quaternion.identity);
            Debug.Log("<color=white>PREVIEW:</color> สร้างตัวเล็งแล้ว");
        }
    }

    public override void ReleaseUse()
    {
        isCharging = false;
        
        // --- ส่วนที่เพิ่ม: ล้าง Preview แบบถอนรากถอนโคน ---
        if (currentPreview != null) Destroy(currentPreview);
        
        // ค้นหาและทำลาย Preview ที่อาจจะค้างอยู่ในฉาก (เผื่อกรณี Instantiate ซ้อน)
        GameObject[] orphans = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in orphans)
        {
            if (obj.name.Contains("Preview Prefab") || obj.name.Contains("Preview(Clone)"))
            {
                Destroy(obj);
            }
        }
        
        Debug.Log("<color=white>PREVIEW:</color> ล้างตัวเล็งทั้งหมดในฉากแล้ว");
    }

    public override void ActivateAutoFire()
    {
        isPlaced = true;
        isCharging = false;
        
        ReleaseUse(); // เรียกใช้เพื่อล้าง Preview ทั้งหมด

        fixedY = transform.position.y; 
        Debug.Log("<color=green>SUCCESS:</color> วางตัวจริงลงบนพื้นเรียบร้อยแล้ว!"); 
    }

    private void Update()
    {
        if (isPlaced) Tick();
    }

    public override void Tick()
    {
        if (isPlaced)
        {
            HandleRandomFire();
        }
        else if (isCharging)
        {
            UpdateAimPreview();
        }
    }

    private void HandleRandomFire() { }

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
        if (currentPreview == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            currentPreview.transform.position = hit.point;
        }
    }
}