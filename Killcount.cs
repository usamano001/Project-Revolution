using UnityEngine;
using UnityEngine.UI;

public class KillCount : MonoBehaviour
{
    public Text killCountText;  // UI Text to display kill count
    private int totalKills = 0; // Total kills

    private void Start()
    {
        UpdateKillCountUI(); // Update the UI at the start
    }

    // Call this method to increment the kill count when an enemy dies
    public void IncrementKillCount()
    {
        totalKills++;  // Increase the kill count
        UpdateKillCountUI(); // Update the UI text
    }

    // Update the kill count UI text
    private void UpdateKillCountUI()
    {
        if (killCountText != null)
        {
            killCountText.text = "Kills: " + totalKills;
        }
    }
}
