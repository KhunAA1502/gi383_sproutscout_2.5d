using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic; // ต้องใช้เพื่อเก็บข้อมูลพิกัด

public class PlantSpawnerUI : MonoBehaviour
{
    public GameObject plantPrefab;
    private PlantData selectedPlantData;
    private bool isPlacing = false;

    // Dictionary สำหรับเก็บว่าพิกัดไหนมีผักอยู่แล้ว (Vector2 คือพิกัด X, Y บนพื้น)
    private static Dictionary<Vector2, bool> occupiedTiles = new Dictionary<Vector2, bool>();

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
            // ปลูกที่จุดที่คลิกทันที โดยล็อกค่า Z ให้ลอยออกมาข้างหน้า และล็อกค่า Y ที่ 0.5
            Vector3 spawnPos = new Vector3(hit.point.x, 0.5f, 0f); 

            Debug.Log($"[PlantSpawner] กำลังปลูก Prefab: {plantPrefab.name} ที่ตำแหน่ง: {spawnPos}");
            
            GameObject newPlant = Instantiate(plantPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"[PlantSpawner] สร้างผักสำเร็จ: {newPlant.name}");
        
            PlantController controller = newPlant.GetComponent<PlantController>();
            if (controller != null)
            {
                controller.spawner = this;
                controller.gridPosition = new Vector2(hit.point.x, hit.point.z);
                Debug.Log($"[PlantSpawner] เรียก Init() กับ: {selectedPlantData?.plantName ?? "ไม่มี"}");
                controller.Init(selectedPlantData);
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

    // ฟังก์ชันสำหรับให้ PlantController เรียกคืนพื้นที่ว่าง
    public void FreeTile(Vector2 pos)
    {
        if (occupiedTiles.ContainsKey(pos))
        {
            occupiedTiles.Remove(pos);
        }
    }
}