using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private bool waitForClickToScene = false;
    [SerializeField]
    private string targetSceneName;
    [SerializeField]
    private GameObject _panel;

    void Update()
    {
        // ตรวจสอบการคลิก 1 ครั้งเพื่อเปลี่ยนซีน
        if (waitForClickToScene && Input.GetMouseButtonDown(0))
        {
            waitForClickToScene = false; // ปิดสถานะ
            SceneManager.LoadScene(targetSceneName);
        }
    }

    // --- 1. ฟังก์ชันเปลี่ยนซีน (ใช้กับปุ่มทั่วไป) ---
    // วิธีใช้: ใน Unity ลากปุ่มมาใส่ OnClick -> เลือก UIManager -> ChangeScene -> พิมพ์ชื่อซีน
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // --- 2. ฟังก์ชันเปิด/ปิด Panel ---
    // วิธีใช้: ลากปุ่มมาใส่ OnClick -> ลาก GameObject (Panel) ไปใส่ในช่อง
    public void TogglePanel(GameObject panel)
    {
        if (panel != null)
        {
            bool isActive = panel.activeSelf;
            _panel.SetActive(false);
            panel.SetActive(!isActive);
            

            PrepareClickToChangeScene("SafeZone");
        }
    }

    // --- 3. ฟังก์ชัน "คลิกที่ไหนก็ได้ 1 ทีแล้วเปลี่ยนซีน" ---
    // วิธีใช้: เรียกฟังก์ชันนี้ (เช่น หลังจบ Cutscene หรือหน้า Loading)
    public void PrepareClickToChangeScene(string sceneName)
    {
        targetSceneName = sceneName;
        waitForClickToScene = true;
        Debug.Log("UIManager: รอการคลิกเพื่อเปลี่ยนซีนเป็น " + sceneName);
    }
}
