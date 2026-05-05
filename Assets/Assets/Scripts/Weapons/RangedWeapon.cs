using UnityEngine;
using UnityEngine.InputSystem;

public class RangedWeapon : Weapon
{
    [Header("Prefabs")]
    public GameObject tomatoPrefab;       // ตัวจริงที่จะยิง
    public GameObject tomatoPreviewPrefab; // ตัวเงาโปร่งแสงที่จะให้คนเล่นเห็น

    [Header("Settings")]
    public Transform throwPoint;           // จุดที่มะเขือเทศจะโผล่ออกมา 
    public float maxChargeTime = 2f;       // เวลากดค้างนานสุด 
    public float minDistance = 1.5f;       // ระยะใกล้สุด 
    public float maxDistance = 8f;         // ระยะไกลสุด 

    [Header("Visuals")]
    public LineRenderer line;              // ตัววาดเส้นเล็ง

    private float chargeTime;              // เก็บว่ากดค้างมานานกี่วินาทีแล้ว
    private bool isCharging;               // เช็กว่าตอนนี้กำลังกดค้างอยู่ไหม
    private GameObject currentPreview;     // เก็บตัวเงาที่สร้างออกมา

    // ทำงานเมื่อเริ่มกดเมาส์
    public override void StartUse()
    {
        isCharging = true;
        chargeTime = 0f; // รีเซ็ตเวลาชาร์จใหม่ทุกครั้งที่กด

        // สร้างเงาขึ้นมาที่ตำแหน่งตัวเรา
        if (tomatoPreviewPrefab != null)
            currentPreview = Instantiate(tomatoPreviewPrefab, throwPoint.position, Quaternion.identity);
    }

    // ทำงานตลอดเวลา (เหมือน Update)
    public override void Tick()
    {
        if (isCharging)
        {
            // เพิ่มเวลาชาร์จไปเรื่อยๆ แต่ไม่ให้เกินเวลาสูงสุดที่ตั้งไว้
            chargeTime = Mathf.Min(chargeTime + Time.deltaTime, maxChargeTime);
            
            // เรียกฟังก์ชันคำนวณหา "จุดเป้าหมายบนพื้น"
            Vector3 targetPos = CalculateTargetPosition();

            // ขยับเงาไปวางไว้ที่จุดเป้าหมายนั้น
            if (currentPreview != null) currentPreview.transform.position = targetPos;
            
            // วาดเส้นจากตัวเรา (Index 0) ไปหาเป้าหมาย (Index 1)
            if (line != null)
            {
                line.positionCount = 2;
                line.SetPosition(0, throwPoint.position);
                line.SetPosition(1, targetPos);
            }
        }
    }

    // ทำงานเมื่อปล่อยเมาส์
    public override void ReleaseUse()
    {
        if (!isCharging) return;
        isCharging = false;

        // คำนวณระยะทางสุดท้ายตามเวลาที่ชาร์จมาได้
        float percent = chargeTime / maxChargeTime;
        float finalDistance = Mathf.Lerp(minDistance, maxDistance, percent);

        // หา "ทิศทาง" จากตัวเราไปหาเมาส์
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldMouse = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 direction = (worldMouse - (Vector2)throwPoint.position).normalized;

        // สร้างมะเขือเทศตัวจริงออกมา
        GameObject tomato = Instantiate(tomatoPrefab, throwPoint.position, Quaternion.identity);
        
        // ส่งทิศทางและระยะทางไปให้สคริปต์ Projectile ทำงานต่อ
        if (tomato.TryGetComponent(out Projectile proj))
        {
            proj.Launch(direction, finalDistance);
        }

        // ลบเงาทิ้ง และซ่อนเส้นเล็ง
        if (currentPreview != null) Destroy(currentPreview);
        if (line != null) line.positionCount = 0;
    }

    // ฟังก์ชันคำนวณตำแหน่ง (ใช้ซ้ำทั้งตอนวาดเงาและตอนยิงจริง)
    private Vector3 CalculateTargetPosition()
    {
        float percent = chargeTime / maxChargeTime;
        // สูตร Lerp: ถ้าชาร์จ 0% จะได้ minDistance, ถ้าชาร์จ 100% จะได้ maxDistance
        float dist = Mathf.Lerp(minDistance, maxDistance, percent);

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldMouse = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 dir = (worldMouse - (Vector2)throwPoint.position).normalized;

        // ส่งค่ากลับเป็น: จุดเริ่ม + (ทิศทางเมาส์ * ระยะทาง)
        return (Vector2)throwPoint.position + (dir * dist);
    }
}