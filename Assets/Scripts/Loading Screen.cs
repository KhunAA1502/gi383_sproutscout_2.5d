using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public Slider progressBar;
    public string sceneToLoad = "SafeZone"; // ���ͩҡ���س��ͧ��è��

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
            // �ӹǳ Progress (��Ҩ�ԧ�ҡ����ͧ + ���ҷ�������ҡ�֧����� 8 �Թҷ�)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            float timeProgress = Mathf.Clamp01(elapsedTime / 8.0f);

            float finalProgress = Mathf.Min(progress, timeProgress);

            if (progressBar != null)
                progressBar.value = finalProgress;

            // �����Ŵ���������� ������Ҽ�ҹ令ú 8 �Թҷ�����
            if (operation.progress >= 0.9f && elapsedTime >= 8.0f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}