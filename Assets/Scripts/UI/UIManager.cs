using UnityEngine;
using UnityEngine.SceneManagement; // ต้องใส่เพื่อใช้เปลี่ยนซีน
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

   
}