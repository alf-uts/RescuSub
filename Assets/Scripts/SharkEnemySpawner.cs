using UnityEngine;
using System.Collections;

public class SharkEnemySpawner : MonoBehaviour
{
    [Header("敌人预制体")]
    public GameObject sharkEnemyPrefab;

    [Header("生成位置设置（手动指定）")]
    public bool spawnOnLeftSide = true;
    public float spawnDistanceX = 15f;

    [Header("生成位置范围")]
    public float minSpawnY = -5f;
    public float maxSpawnY = 5f;
    public float spawnYInterval = 1.5f;

    [Header("敌人基础属性")]
    public float moveSpeed = 2f;
    public int damageOnHit = 1;

    [Header("屏幕边界")]
    public float screenLeftX = -20f;
    public float screenRightX = 20f;

    [Header("生成频率")]
    public float initialSpawnDelay = 2f;

    [Header("===== 难度递增设置 =====")]
    [Header("生成间隔递减")]
    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 5f;
    public float difficultySpawnRate = 30f;

    [Header("敌人属性增强")]
    public float maxSpeedMultiplier = 1.5f;
    public int baseHealth = 1;
    public int maxHealthBonus = 2;

    private Transform player;
    private float lastSpawnY;
    private float currentSpawnInterval;
    private float currentSpeedMultiplier;
    private int currentHealthBonus;
    private float gameTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("未找到玩家！请检查玩家标签是否为Player");
            return;
        }

        lastSpawnY = minSpawnY;
        currentSpawnInterval = maxSpawnInterval;
        currentSpeedMultiplier = 1f;
        currentHealthBonus = 0;

        StartCoroutine(SpawnRoutine());
        StartCoroutine(DifficultyIncreaseRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(initialSpawnDelay);

        while (GameManager.Instance != null && !GameManager.Instance.gameOver)
        {
            SpawnSharkEnemy();
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
            currentSpeedMultiplier = Mathf.Lerp(1f, maxSpeedMultiplier, progress);
            currentHealthBonus = Mathf.FloorToInt(Mathf.Lerp(0, maxHealthBonus, progress));
        }
    }

    void SpawnSharkEnemy()
    {
        if (GameManager.Instance == null || GameManager.Instance.gameOver) return;
        if (player == null || sharkEnemyPrefab == null) return;

        float spawnX = spawnOnLeftSide ? (player.position.x - spawnDistanceX) : (player.position.x + spawnDistanceX);
        float spawnY = GetValidSpawnY();
        Vector2 spawnPosition = new Vector2(spawnX, spawnY);

        GameObject shark = Instantiate(sharkEnemyPrefab, spawnPosition, Quaternion.identity);

        SpriteRenderer sharkRenderer = shark.GetComponent<SpriteRenderer>();
        if (sharkRenderer != null)
        {
            sharkRenderer.flipX = !spawnOnLeftSide;
        }

        SharkEnemy sharkEnemy = shark.GetComponent<SharkEnemy>();
        if (sharkEnemy != null)
        {
            sharkEnemy.moveDirection = spawnOnLeftSide ? 1 : -1;
            sharkEnemy.moveSpeed = moveSpeed * currentSpeedMultiplier;
            sharkEnemy.screenLeftX = screenLeftX;
            sharkEnemy.screenRightX = screenRightX;
            sharkEnemy.damageOnHit = damageOnHit;
            sharkEnemy.maxHealth = baseHealth + currentHealthBonus;
            sharkEnemy.currentHealth = sharkEnemy.maxHealth;
        }
        else
        {
            Debug.LogError("敌人预制体未添加SharkEnemy脚本");
            Destroy(shark);
        }
    }

    private float GetValidSpawnY()
    {
        float nextSpawnY = lastSpawnY + spawnYInterval;
        if (nextSpawnY > maxSpawnY)
        {
            nextSpawnY = minSpawnY;
        }
        lastSpawnY = nextSpawnY;

        float randomOffset = Random.Range(-0.5f, 0.5f);
        return Mathf.Clamp(nextSpawnY + randomOffset, minSpawnY, maxSpawnY);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(-spawnDistanceX, minSpawnY), new Vector2(spawnDistanceX, minSpawnY));
        Gizmos.DrawLine(new Vector2(-spawnDistanceX, maxSpawnY), new Vector2(spawnDistanceX, maxSpawnY));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(screenLeftX, minSpawnY - 1), new Vector2(screenLeftX, maxSpawnY + 1));
        Gizmos.DrawLine(new Vector2(screenRightX, minSpawnY - 1), new Vector2(screenRightX, maxSpawnY + 1));

        Gizmos.color = spawnOnLeftSide ? Color.blue : Color.yellow;
        string sideText = spawnOnLeftSide ? "左侧生成" : "右侧生成";
        UnityEditor.Handles.Label(transform.position + Vector3.up, sideText);
    }
}
