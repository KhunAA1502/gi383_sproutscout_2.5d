using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    void Awake()
    {
        // ตรวจสอบว่ามีตัวจัดการ UI อยู่แล้วหรือไม่
        if (instance == null)
        {
            instance = this;
            // สั่งให้ Object นี้ (และลูกๆ เช่น Canvas) ไม่ถูกทำลายเมื่อเปลี่ยนซีน
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // ถ้ามีอันเดิมอยู่แล้ว ให้ทำลายอันใหม่ทิ้งเพื่อป้องกัน UI ซ้ำซ้อน
            Destroy(gameObject);
        }
    }
}