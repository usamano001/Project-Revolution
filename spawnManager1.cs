using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager1 : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Array to hold different enemy prefabs
    public Transform[] spawnPoints;  // List of spawn points for enemies
    public float spawnInterval = 5f; // Interval between enemy spawns
    public int maxEnemies = 10;      // Maximum allowed enemies in the scene
    private int waveNumber = 1;      // Current wave number
    private bool spawningInProgress = false;

    private List<GameObject> currentEnemies = new List<GameObject>();

    void Start()
    {
        // Start the spawn process
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        while (true) // Continuously spawn waves
        {
            // Ensure all current enemies are defeated before proceeding with the next wave
            yield return new WaitUntil(() => AreAllEnemiesDefeated());

            // Start spawning the new enemies for the current wave
            spawningInProgress = true;
            for (int i = 0; i < waveNumber * 3; i++) // Increase enemies per wave
            {
                if (GetEnemyCount() < maxEnemies) // Check if the enemy count is within the limit
                {
                    SpawnEnemy();
                }
                else
                {
                    Debug.Log("Max enemy limit reached. Pausing spawn.");
                    break; // Stop spawning if maxEnemies is reached
                }

                yield return new WaitForSeconds(spawnInterval); // Delay between spawns
            }
            spawningInProgress = false;

            // Increase the wave number for the next round
            waveNumber++;

            // Optional: Delay before starting the next wave
            yield return new WaitForSeconds(3f); // Wait for 3 seconds before the next wave
        }
    }

    void SpawnEnemy()
    {
        // Select a random spawn point and a random enemy prefab
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        currentEnemies.Add(newEnemy); // Add to the list of current enemies

        // Subscribe to the OnDeath event to remove the enemy when it dies
        EnemyHealthWithUI enemyHealth = newEnemy.GetComponent<EnemyHealthWithUI>();
        if (enemyHealth != null)
        {
            enemyHealth.OnDeath += () => currentEnemies.Remove(newEnemy); // Remove enemy when it dies
        }
    }

    int GetEnemyCount()
    {
        // Count the enemies currently in the scene using the currentEnemies list
        return currentEnemies.Count;
    }

    bool AreAllEnemiesDefeated()
    {
        // Check if all enemies in the currentEnemies list are defeated
        foreach (GameObject enemy in currentEnemies)
        {
            if (enemy != null)
            {
                EnemyHealthWithUI enemyHealth = enemy.GetComponent<EnemyHealthWithUI>();
                if (enemyHealth != null && !enemyHealth.IsDead) // Check if the enemy is still alive
                {
                    return false; // If at least one enemy is still alive, return false
                }
            }
        }
        return true; // If all enemies are defeated, return true
    }
}
