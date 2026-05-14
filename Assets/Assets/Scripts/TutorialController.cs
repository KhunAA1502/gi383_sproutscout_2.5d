using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    public static TutorialController Instance;

    // ลาก TutorialPanel จาก Hierarchy มาใส่ในช่องนี้ใน Inspector
    public GameObject tutorialPanel;
    private Button closeButton;
    private bool closeButtonRegistered;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        EnsureTutorialPanel();

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        // รีเซ็ตเวลาให้เป็นปกติเมื่อเข้าซีนใหม่
        Time.timeScale = 1f;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureTutorialPanel();

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    void EnsureTutorialPanel()
    {
        if (tutorialPanel == null)
        {
            tutorialPanel = FindGameObjectInScene("Tutorials");
            closeButtonRegistered = false;
        }

        if (tutorialPanel != null)
        {
            if (closeButton == null)
            {
                closeButtonRegistered = false;
            }
            RegisterCloseButton();
        }
    }

    void RegisterCloseButton()
    {
        if (closeButtonRegistered || tutorialPanel == null)
        {
            return;
        }

        closeButton = FindButtonByName(tutorialPanel.transform, "close");

        if (closeButton == null)
        {
            closeButton = tutorialPanel.GetComponentInChildren<Button>(true);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseTutorial);
            closeButtonRegistered = true;
        }
    }

    Button FindButtonByName(Transform parent, string partialName)
    {
        var button = parent.GetComponent<Button>();
        if (button != null && parent.name.ToLower().Contains(partialName.ToLower()))
        {
            return button;
        }

        foreach (Transform child in parent)
        {
            var found = FindButtonByName(child, partialName);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    GameObject FindGameObjectInScene(string targetName)
    {
        foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            var found = FindInChildren(root.transform, targetName);
            if (found != null)
            {
                return found.gameObject;
            }
        }

        return null;
    }

    Transform FindInChildren(Transform parent, string targetName)
    {
        if (parent.name == targetName)
        {
            return parent;
        }

        foreach (Transform child in parent)
        {
            var found = FindInChildren(child, targetName);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    // ฟังก์ชันสำหรับเปิด Tutorial
    public void OpenTutorial()
    {
        EnsureTutorialPanel();

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
            // หยุดเวลาในเกมไว้ (ถ้าต้องการ) เพื่อให้ผู้เล่นอ่านได้สบายๆ
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("TutorialController: tutorialPanel ยังไม่ได้กำหนดค่าใน Inspector และไม่พบ GameObject ชื่อ Tutorials");
        }
    }

    // ฟังก์ชันสำหรับปิด Tutorial
    public void CloseTutorial()
    {
        EnsureTutorialPanel();

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("TutorialController.CloseTutorial: ไม่พบ tutorialPanel เพื่อปิด");
        }

        // กลับมาเดินเวลาในเกมตามปกติ
        Time.timeScale = 1f;
    }
}