using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public List<WaveData> waves; 
    public Transform[] spawnPoints;

    [Header("UI Elements")]
    public TextMeshProUGUI enemyCountText;
    public GameObject winUI;

    private int currentWaveIndex = 0;
    public bool isSpawning = false;
    private int enemiesCount = 0;

    private void Start()
    {
        // ค้นหา EnemyCountText อัตโนมัติด้วย Tag แบบปลอดภัย (ไม่ทำให้เครื่องค้างถ้าลืมสร้าง Tag)
        try 
        {
            GameObject textObj = GameObject.FindWithTag("EnemyCountText");
            if (textObj != null)
            {
                enemyCountText = textObj.GetComponent<TextMeshProUGUI>();
            }
        }
        catch (System.Exception)
        {
            Debug.LogWarning("[WaveManager] ยังไม่ได้สร้าง Tag ชื่อ 'EnemyCountText' ใน Tag Manager");
        }

        // WinUI ให้ใช้ตามที่ลากใส่ใน Inspector (ตามที่คุณต้องการ)
        if (winUI != null) winUI.SetActive(false);
        
        UpdateEnemyCountUI();

        if (waves != null && waves.Count > 0)
        {
            StartCoroutine(SpawnWave(waves[currentWaveIndex]));
        }
        else
        {
            Debug.LogWarning("[WaveManager] ไม่มีข้อมูล Wave ใน List!");
        }
    }

    private void UpdateEnemyCountUI()
    {
        if (enemyCountText != null)
        {
            enemyCountText.text = $"Enemies: {enemiesCount}";
        }
    }

    IEnumerator SpawnWave(WaveData wave)
    {
        isSpawning = true;
        // หมายเหตุ: enemiesCount จะเพิ่มขึ้นทีละตัวเมื่อ Instantiate เพื่อความแม่นยำ
        
        for (int i = 0; i < wave.enemyCount; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomIndex];

            GameObject enemyPrefab = wave.enemyPrefab[Random.Range(0, wave.enemyPrefab.Length)];

            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            enemiesCount++;
            UpdateEnemyCountUI();

            if (enemy.TryGetComponent(out Character character))
            {
                character.OnDeath += HandleEnemyDeath;
            }
            else
            {
                Debug.LogWarning($"[WaveManager] Enemy {enemy.name} ไม่มี Character component!");
            }

            yield return new WaitForSeconds(wave.spawnRate);
        }

        isSpawning = false;
    }

    void HandleEnemyDeath()
    {
        enemiesCount--;
        UpdateEnemyCountUI();

        if (enemiesCount <= 0 && !isSpawning)
        {
            currentWaveIndex++;
            if (currentWaveIndex < waves.Count)
            {
                Debug.Log($"Wave {currentWaveIndex} Cleared! Starting next wave...");
                StartCoroutine(SpawnWave(waves[currentWaveIndex]));
            }
            else
            {
                Debug.Log("All Waves Cleared! YOU WIN!");
                if (winUI != null)
                {
                    winUI.SetActive(true);
                }
            }
        }
    }
}
