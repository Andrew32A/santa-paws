using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // singleton-like reference (for easy calling from other scripts)
    public static ScoreManager Instance;

    [Header("Score Display (TMP)")]
    public TextMeshProUGUI scoreText;
    public int score = 0;
    public int digits = 7;

    [Header("Present Settings")]
    public Transform presentSpawnPoint;
    public GameObject[] presentPrefabs;
    public float spawnRotationRange = 15f;
    public float spawnOffsetRange = 50f;

    [Header("Combo")]
    public TextMeshProUGUI comboText;
    public float comboMultiplier = 1f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateScoreText();
        UpdateComboText(); // ensure combo text is up to date at start
    }

    /// call this whenever an enemy is destroyed or you want to add points
    public void AddScore(int amount)
    {
        // store the old score before adding
        int oldScore = score;

        // calculate how many points to add, factoring in combo
        int comboScore = Mathf.RoundToInt(amount * comboMultiplier);

        // update score
        score += comboScore;
        UpdateScoreText();

        // check every value we just "crossed" from oldScore+1 up to new score
        // if it is a multiple of 100 (i.e., s % 100 == 0), we spawn a present.
        for (int s = oldScore + 1; s <= score; s++)
        {
            if (s % 100 == 0)
            {
                // we just hit a multiple of 100, so let's spawn
                SpawnRandomPresent();
            }
        }
    }

    /// call this when the player correctly draws a shape to increase combo by +0.2
    public void IncreaseCombo()
    {
        comboMultiplier += 0.2f;
        UpdateComboText();
    }

    /// updates the combo TMP text (e.g. "x1.2") 
    private void UpdateComboText()
    {
        if (comboText != null)
        {
            comboText.text = "x" + comboMultiplier.ToString("F1");
        }
        else
        {
            Debug.LogWarning("ScoreManager: No TextMeshProUGUI assigned to comboText!");
        }
    }

    /// updates the TMP text with leading zeros (e.g. "0000100").
    private void UpdateScoreText()
    {
        // format score with leading zeros using "D + digits"
        string formattedScore = score.ToString("D" + digits);

        if (scoreText != null)
        {
            scoreText.text = formattedScore;
        }
        else
        {
            Debug.LogWarning("ScoreManager: No TextMeshProUGUI assigned to scoreText!");
        }
    }

    /// spawns a random present near presentSpawnPoint with slight random rotation.
    private void SpawnRandomPresent()
    {
        if (presentPrefabs == null || presentPrefabs.Length == 0)
        {
            Debug.LogWarning("ScoreManager: No presentPrefabs assigned!");
            return;
        }

        // pick a random present prefab
        GameObject chosenPrefab = presentPrefabs[Random.Range(0, presentPrefabs.Length)];

        // base spawn position (use spawnPoint if assigned; otherwise zero)
        Vector3 basePos = presentSpawnPoint != null
            ? presentSpawnPoint.position
            : Vector3.zero;

        // random offset in X/Y within spawnOffsetRange
        float offsetX = Random.Range(-spawnOffsetRange, spawnOffsetRange);
        float offsetY = Random.Range(-spawnOffsetRange, spawnOffsetRange);
        Vector3 offset = new Vector3(offsetX, offsetY, 0f);

        // final position
        Vector3 spawnPos = basePos + offset;

        // slight random rotation around Z
        float randomAngle = Random.Range(-spawnRotationRange, spawnRotationRange);
        Quaternion spawnRot = Quaternion.Euler(0f, 0f, randomAngle);

        // instantiate the present
        GameObject present = Instantiate(chosenPrefab, spawnPos, spawnRot);
        Debug.Log($"Spawned a random present: {present.name}");
    }
}
