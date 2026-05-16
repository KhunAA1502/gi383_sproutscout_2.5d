using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic; // ต้องใช้เพื่อเก็บข้อมูลพิกัด

public class PlantSpawnerUI : MonoBehaviour
{
    public GameObject plantPrefab;
    private PlantData selectedPlantData;
    private bool isPlacing = false;
    
    [Header("Grid Settings")]
    public float gridSize = 1f; // ขนาดของแต่ละช่องในกริด

    // Dictionary สำหรับเก็บว่าพิกัดไหนมีผักอยู่แล้ว (Vector2 คือพิกัด X, Y บนพื้น)
    public static Dictionary<Vector2, bool> occupiedTiles = new Dictionary<Vector2, bool>();

    public void SelectPlant(PlantData data)
    {
        selectedPlantData = data;
        isPlacing = true;
    }

    void Update()
    {
        if (isPlacing && Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                PlacePlant();
            }
        }
    }

    void PlacePlant()
    {
        // ยิง Ray จากเมาส์ไปหาจุดที่คลิก
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Snap ตำแหน่งให้ตรงกับ Grid
            Vector3 snappedPos = SnapToGrid(hit.point);
            Vector2 gridKey = new Vector2(snappedPos.x, snappedPos.z);

            // ตรวจสอบว่าช่องนี้ว่างหรือไม่
            if (occupiedTiles.ContainsKey(gridKey) && occupiedTiles[gridKey])
            {
                Debug.Log($"<color=red>[PlantSpawner] ช่องนี้มีผักแล้ว!</color> ตำแหน่ง: {gridKey}");
                isPlacing = false;
                return;
            }

            Vector3 spawnPos = new Vector3(snappedPos.x, 0.5f, snappedPos.z);
            Debug.Log($"[PlantSpawner] กำลังปลูก Prefab: {plantPrefab.name} ที่ตำแหน่ง: {spawnPos}");
            
            GameObject newPlant = Instantiate(plantPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"[PlantSpawner] สร้างผักสำเร็จ: {newPlant.name}");
        
            PlantController controller = newPlant.GetComponent<PlantController>();
            if (controller != null)
            {
                controller.spawner = this;
                controller.gridPosition = gridKey;
                Debug.Log($"[PlantSpawner] เรียก Init() กับ: {selectedPlantData?.plantName ?? "ไม่มี"}");
                controller.Init(selectedPlantData);
                
                // Mark ช่องนี้เป็น occupied
                occupiedTiles[gridKey] = true;
            }
            else
            {
                Debug.LogWarning("[PlantSpawner] ไม่พบ PlantController component!");
            }

            isPlacing = false;
        }
        else
        {
            Debug.LogWarning("[PlantSpawner] Raycast ไม่ชน!");
        }
    }

    // ฟังก์ชัน Snap to Grid
    private Vector3 SnapToGrid(Vector3 position)
    {
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float z = Mathf.Round(position.z / gridSize) * gridSize;
        return new Vector3(x, position.y, z);
    }

    // ฟังก์ชันสำหรับให้ PlantController เรียกคืนพื้นที่ว่าง
    public void FreeTile(Vector2 pos)
    {
        if (occupiedTiles.ContainsKey(pos))
        {
            occupiedTiles[pos] = false;
            Debug.Log($"[PlantSpawner] Free tile: {pos}");
        }
    }
}