using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Queue Parent")]
    [Tooltip("Parent containing all the child enemies (inactive).")]
    public Transform enemyQueueParent;

    [Header("Spawning Settings")]
    public Transform optionalSpawnPoint;
    public float spawnInterval = 3f; // Time between spawns

    private List<GameObject> inactiveEnemies = new List<GameObject>();
    private int currentIndex = 0; // Tracks which enemy to activate next

    void Start()
    {
        // Gather all child objects from 'enemyQueueParent' into 'inactiveEnemies'.
        GatherEnemiesFromQueue();

        // Start spawning loop.
        StartCoroutine(SpawnLoop());
    }

    /// <summary>
    /// Collects all children of 'enemyQueueParent' and adds them to 'inactiveEnemies'.
    /// </summary>
    private void GatherEnemiesFromQueue()
    {
        // Just in case: clear the list first
        inactiveEnemies.Clear();

        // For each child under 'enemyQueueParent', set inactive and add to the list
        foreach (Transform child in enemyQueueParent)
        {
            child.gameObject.SetActive(false);
            inactiveEnemies.Add(child.gameObject);
        }
    }

    private IEnumerator SpawnLoop()
    {
        // Keep spawning until we run out of enemies
        while (currentIndex < inactiveEnemies.Count)
        {
            // Wait for 'spawnInterval' seconds before activating the next one
            yield return new WaitForSeconds(spawnInterval);

            // Get the next enemy
            GameObject enemy = inactiveEnemies[currentIndex];

            // (Optional) set position/rotation if you want them all spawned at the same point
            if (optionalSpawnPoint != null)
            {
                enemy.transform.position = optionalSpawnPoint.position;
                enemy.transform.rotation = optionalSpawnPoint.rotation;
            }

            // Activate the enemy
            enemy.SetActive(true);

            // Move on to the next enemy
            currentIndex++;
        }

        Debug.Log("No more inactive enemies to spawn!");
    }
}
