using UnityEngine;

public class PlantController : MonoBehaviour
{
    [Header("Settings")]
    public PlantData data; // เปลี่ยนมาใช้ ScriptableObject เพื่อให้ปลูกผักได้หลากหลายชนิด
    public float timePerStage = 5f; // ปรับเป็น 5 วินาทีตามที่ต้องการ
    public float interactDistance = 3f; // ระยะที่เหมาะสมสำหรับเกม 2.5D

    public Vector2 gridPosition; // เพิ่มตัวแปรเก็บพิกัดตัวเอง
    public PlantSpawnerUI spawner; // เพิ่มตัวแปรอ้างอิงถึงตัวคุมการวาง
    public Farmland farmland; // เพิ่มตัวแปรอ้างอิงถึงพื้นที่ปลูก

    private GameObject currentVisual;
    private int currentStage = 0; // 0=Seed, 1=Seedling, 2=Mature
    private float timer;
    private Transform player;
    private bool isGrown = false;

    // ฟังก์ชัน Init สำหรับรับข้อมูลจาก Spawner เมื่อปลูก
    public void Init(PlantData plantData)
    {
        Debug.Log($"[PlantController] Init() เรียกใช้งาน plantData={plantData?.plantName ?? "null"}");
        data = plantData;
        currentStage = 0;
        timer = 0f;
        isGrown = false;
        UpdateVisuals();
    }

    void Start()
    {
        // ค้นหา Player ในฉาก
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        
        if (data != null)
        {
            UpdateVisuals();
        }
    }

    void Update()
    {
        // 1. ระบบเติบโต (ทำงานตามเวลาที่กำหนดใน PlantData)
        if (currentStage < 2)
        {
            timer += Time.deltaTime;
            float currentTimePerStage = (currentStage == 0) ? data.timeToSprout : data.timeToMature;
            if (timer >= currentTimePerStage)
            {
                currentStage++;
                timer = 0;
                UpdateVisuals();
                
                if (currentStage == 2) isGrown = true;
            }
        }

        // 2. ระบบกดเก็บผัก (กด E)
        if (Input.GetKeyDown(KeyCode.E) && player != null)
        {
            CheckInteraction();
        }
    }

    void UpdateVisuals()
    {
        if (currentVisual != null)
        {
            Destroy(currentVisual);
        }

        if (data == null)
        {
            Debug.LogWarning("[PlantController] ไม่มีข้อมูล PlantData ให้แสดง visual");
            return;
        }

        Debug.Log($"[PlantController] UpdateVisuals() stage={currentStage} plant={data.plantName}");

        GameObject stagePrefab = null;
        if (currentStage == 0) stagePrefab = data.seedModel;
        else if (currentStage == 1) stagePrefab = data.seedlingModel;
        else if (currentStage == 2) stagePrefab = data.matureModel;

        if (stagePrefab == null)
        {
            Debug.LogWarning($"[PlantController] ไม่มี prefab สำหรับ stage {currentStage} ของผัก {data.plantName}");
            return;
        }

        currentVisual = Instantiate(stagePrefab, transform);
        currentVisual.transform.localPosition = Vector3.zero;
        currentVisual.transform.localRotation = Quaternion.identity;
        currentVisual.transform.localScale = Vector3.one;

        Debug.Log($"Current Visual Stage: {currentStage}");
        Debug.Log($"[UpdateVisuals] สร้าง model stage {currentStage} สำเร็จ: {stagePrefab.name}");
    }

    void CheckInteraction()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactDistance)
        {
            if (isGrown) 
            {
                CollectPlant();
            }
            else
            {
                Debug.Log($"{data?.plantName ?? "ผัก"} ยังไม่โตเต็มที่!");
            }
        }
    }

    void CollectPlant()
    {
        // ถ้ามี farmland ให้ใช้ farmland.HarvestPlant()
        if (farmland != null)
        {
            farmland.HarvestPlant(this);
        }
        else
        {
            // Fallback สำหรับกรณีไม่มี farmland (backward compatibility)
            InventoryManager inventory = FindFirstObjectByType<InventoryManager>();
            if (inventory != null && data != null && data.harvestItem != null)
            {
                inventory.AddItem(data.harvestItem, 1);
            }

            // คืนพื้นที่ว่างให้ Grid
            if (spawner != null) spawner.FreeTile(gridPosition);

            Destroy(gameObject);
        }
    }
}