using UnityEngine;
using UnityEngine.SceneManagement; // สำคัญมากสำหรับการเปลี่ยนฉาก

public class SceneTeleport : MonoBehaviour
{
    [Header("ตั้งชื่อฉากที่จะไป")]
    public string nextSceneName;

    private void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าวัตถุที่มาชนมี Tag ชื่อว่า "Player" หรือไม่
        if (other.CompareTag("Player"))
        {
            Debug.Log("กำลังเปลี่ยนฉากไปที่: " + nextSceneName);

            // เปลี่ยนฉาก (ข้อมูลใน PlayerController ที่ทำ DontDestroyOnLoad ไว้จะตามไปด้วย)
            SceneManager.LoadScene(nextSceneName);
        }
    }
}