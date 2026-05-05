using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cropCountText;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image energyBar;
    
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (gameManager == null) return;

        // Update crop count
        if (cropCountText != null)
        {
            cropCountText.text = $"🌾 เก็บเกี่ยว: {gameManager.GetTotalCrops()}";
        }

        // Update energy
        if (energyText != null)
        {
            int energy = Mathf.RoundToInt(gameManager.currentEnergy);
            int maxEnergy = Mathf.RoundToInt(gameManager.maxEnergy);
            energyText.text = $"⚡ พลัง: {energy}/{maxEnergy}";
        }

        // Update energy bar
        if (energyBar != null)
        {
            energyBar.fillAmount = gameManager.GetEnergyPercent();
        }

        // Update status
        if (statusText != null)
        {
            if (gameManager.GetEnergyPercent() < 0.2f)
                statusText.text = "😴 พลังต่ำ! ไปพักผ่อน...";
            else if (gameManager.GetEnergyPercent() > 0.9f)
                statusText.text = "💪 พลังเต็มแล้ว!";
            else
                statusText.text = "🌾 มาปลูกพืชเถอะ!";
        }
    }
}
