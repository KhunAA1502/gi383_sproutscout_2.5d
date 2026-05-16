using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float currentEnergy = 100f;
    public float maxEnergy = 100f;
    private int totalCrops = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetTotalCrops()
    {
        return totalCrops;
    }

    public float GetEnergyPercent()
    {
        return currentEnergy / maxEnergy;
    }

    public void AddCrops(int amount)
    {
        totalCrops += amount;
    }

    public void UseEnergy(float amount)
    {
        currentEnergy -= amount;
        if (currentEnergy < 0) currentEnergy = 0;
    }

    public void RestoreEnergy(float amount)
    {
        currentEnergy += amount;
        if (currentEnergy > maxEnergy) currentEnergy = maxEnergy;
    }
}
