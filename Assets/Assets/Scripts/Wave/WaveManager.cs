using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WaveManager : MonoBehaviour
{
   public List<WaveData> waves; 
    public Transform[] spawnPoints;

    private int currentWaveIndex = 0;
    public bool isSpawning = false;
    private int enemiesCount = 0;

    private void Start()
    {
        StartCoroutine(SpawnWave(waves[currentWaveIndex]));
        currentWaveIndex++;
    }



    IEnumerator SpawnWave(WaveData wave)
    {
        isSpawning = true;
        enemiesCount = wave.enemyCount;

        for (int i = 0; i < wave.enemyCount; i++)
        {
           int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomIndex];

            GameObject enemyPrefab = wave.enemyPrefab[Random.Range(0, wave.enemyPrefab.Length)];

            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            enemy.GetComponent<Character>().OnDeath += HandleEnemyDeath;

            yield return new WaitForSeconds(wave.spawnRate);
        }

        isSpawning = false;
    }

    void HandleEnemyDeath()
    {
        enemiesCount--;
        if (enemiesCount <= 0 && !isSpawning)
        {
            Debug.Log("Wave Cleared!");
           
        }
    }
}
