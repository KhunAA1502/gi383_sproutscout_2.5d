using UnityEngine;

public class PlantController : MonoBehaviour
{
    [Header("Settings")]
    public ItemData cabbageItemData; // ไฟล์ ItemData (ผัก) ที่ลากใส่
    public float timePerStage = 3f;
    public float interactDistance = 10f;

    [Header("Visual Models")]
    public GameObject seedModel;
    public GameObject seedlingModel;
    public GameObject matureModel;

    private int currentStage = 0; // 0=Seed, 1=Seedling, 2=Mature
    private float timer;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        UpdateVisuals();
    }

    void Update()
    {
        // 1. ระบบเติบโต
        if (currentStage < 2)
        {
            timer += Time.deltaTime;
            if (timer >= timePerStage)
            {
                currentStage++;
                timer = 0;
                UpdateVisuals();
            }
        }

        // 2. ระบบกดเก็บผัก (กด E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckInteraction();
        }
    }

    void UpdateVisuals()
    {
        seedModel.SetActive(currentStage == 0);
        seedlingModel.SetActive(currentStage == 1);
        matureModel.SetActive(currentStage == 2);
    }

    void CheckInteraction()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactDistance)
        {
            if (currentStage == 2) // ต้องโตเต็มที่ (Mature) ถึงจะเก็บได้
            {
                CollectPlant();
            }
            else
            {
                Debug.Log("ผักยังไม่โตเต็มที่! รออีกสักพัก");
            }
        }
    }

    void CollectPlant()
    {
        if (cabbageItemData != null)
        {
            // 1. หาตัว Inventory ในฉาก (สมมติชื่อสคริปต์ว่า InventoryManager)
            InventoryManager inventory = FindFirstObjectByType<InventoryManager>();

            if (inventory != null)
            {
                // 2. ส่งไฟล์ ItemData เข้าไปในฟังก์ชันเพิ่มของ (เช่น AddItem)
                inventory.AddItem(cabbageItemData);

                Debug.Log("ส่ง " + cabbageItemData.itemName + " เข้า Inventory แล้ว!");
                Destroy(gameObject); // ทำลายผักทิ้งหลังจากส่งข้อมูลเสร็จ
            }
            else
            {
                Debug.LogError("หา InventoryManager ไม่เจอในฉาก!");
            }
        }
    }
}