using UnityEngine;
using UnityEngine.EventSystems; // ต้องเพิ่มเพื่อเช็กว่าไม่ได้คลิกโดนปุ่ม UI

public class PlantSpawnerUI : MonoBehaviour
{
    public GameObject plantPrefab;
    private bool isPlacing = false; // สถานะว่ากำลังจะวางผักไหม

    // ฟังก์ชันนี้เรียกจากปุ่ม UI
    public void SelectPlant()
    {
        isPlacing = true;
        Debug.Log("เลือกผักแล้ว! คลิกที่พื้นเพื่อปลูก");
    }

    void Update()
    {
        // ถ้าอยู่ในโหมดวางผัก และมีการคลิกเมาส์ซ้าย
        if (isPlacing && Input.GetMouseButtonDown(0))
        {
            // ตรวจสอบว่าเมาส์ไม่ได้คลิกโดน UI อื่นๆ (เช่น ปุ่ม)
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                PlacePlant();
            }
        }
    }

    void PlacePlant()
    {
        // แปลงตำแหน่งเมาส์จากหน้าจอ (Screen Space) เป็นโลกในเกม (World Space)
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // ระยะห่างจากกล้อง (ปรับตามความเหมาะสม)
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // สำหรับเกม 2D ให้เซตค่า Z เป็น 0 เพื่อให้ผักอยู่ระนาบเดียวกับฉาก
        worldPos.z = 0;

        // สร้างผักในตำแหน่งที่เมาส์ชี้
        Instantiate(plantPrefab, worldPos, Quaternion.identity);

        // หากต้องการปลูกแค่ต้นเดียวแล้วจบโหมดวาง ให้เซตเป็น false
        isPlacing = false;
    }
}