using UnityEngine;

public class LevelInitializer : MonoBehaviour
{
    [Header("ลากจุด Spawn มาใส่ตรงนี้")]
    public Transform spawnPoint;

    void Start()
    {
        // ค้นหา PlayerController.instance ที่ติดมาจากฉากที่แล้ว
        if (PlayerController.instance != null && spawnPoint != null)
        {
            // ย้ายตำแหน่งตัวละครไปที่จุดเกิด
            PlayerController.instance.transform.position = spawnPoint.position;
            PlayerController.instance.transform.rotation = spawnPoint.rotation;

            Debug.Log("ย้ายตัวละครไปที่จุดเกิดเรียบร้อยแล้ว");
        }
        else
        {
            Debug.LogWarning("ไม่พบ Player หรือจุด Spawn ในฉากนี้");
        }
    }
}