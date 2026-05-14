using UnityEngine;

public class TutorialController : MonoBehaviour
{
    // ลาก TutorialPanel จาก Hierarchy มาใส่ในช่องนี้ใน Inspector
    public GameObject tutorialPanel;

    // ฟังก์ชันสำหรับเปิด Tutorial
    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
        // หยุดเวลาในเกมไว้ (ถ้าต้องการ) เพื่อให้ผู้เล่นอ่านได้สบายๆ
        Time.timeScale = 0f; 
    }

    // ฟังก์ชันสำหรับปิด Tutorial
    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        // กลับมาเดินเวลาในเกมตามปกติ
        Time.timeScale = 1f; 
    }
}