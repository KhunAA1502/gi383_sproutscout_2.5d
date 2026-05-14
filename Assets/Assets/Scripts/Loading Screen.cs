using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public Slider progressBar;
    public string sceneToLoad = "SafeZone 1"; // ชื่อฉากที่คุณต้องการจะไป

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        float startTime = Time.time;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float elapsedTime = Time.time - startTime;
            // คำนวณ Progress (ค่าจริงจากเครื่อง + เวลาที่เราอยากดึงเช็งไว้ 8 วินาที)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            float timeProgress = Mathf.Clamp01(elapsedTime / 8.0f);

            float finalProgress = Mathf.Min(progress, timeProgress);

            if (progressBar != null)
                progressBar.value = finalProgress;

            // ถ้าโหลดข้อมูลเสร็จ และเวลาผ่านไปครบ 8 วินาทีแล้ว
            if (operation.progress >= 0.9f && elapsedTime >= 8.0f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}