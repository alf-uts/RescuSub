using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;

    [Header("基础设置")]
    public float spawnDistance = 15f;

    [Header("===== 难度递增设置 =====")]
    [Header("生成间隔递减")]
    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 5f;
    public float difficultySpawnRate = 30f;

    private Transform player;
    private float currentSpawnInterval;
    private float gameTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("未找到玩家！请检查玩家标签是否为Player");
            return;
        }

        currentSpawnInterval = maxSpawnInterval;
        gameTime = 0f;

        StartCoroutine(SpawnRoutine());
        StartCoroutine(DifficultyIncreaseRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(2f);

        while (GameManager.Instance != null && !GameManager.Instance.gameOver)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

    private IEnumerator DifficultyIncreaseRoutine()
    {
        while (GameManager.Instance != null && !GameManager.Instance.gameOver)
        {
            yield return new WaitForSeconds(1f);
            gameTime += 1f;

            float progress = Mathf.Clamp01(gameTime / difficultySpawnRate);
            currentSpawnInterval = Mathf.Lerp(maxSpawnInterval, minSpawnInterval, progress);
        }
    }

    void SpawnEnemy()
    {
        if (GameManager.Instance == null || GameManager.Instance.gameOver) return;
        if (player == null || enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        Vector2 spawnDirection = Random.insideUnitCircle.normalized;
        Vector2 spawnPosition = (Vector2)player.position + spawnDirection * spawnDistance;

        int randomEnemy = Random.Range(0, enemyPrefabs.Length);

        Instantiate(enemyPrefabs[randomEnemy], spawnPosition, Quaternion.identity);
    }
}
