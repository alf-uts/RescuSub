using UnityEngine;

public class LifeItemSpawner : MonoBehaviour
{
    [Header("生命恢复道具预制体")]
    public GameObject lifeItemPrefab;

    [Header("生成范围")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -15f;
    public float maxY = -2f;

    [Header("生成频率")]
    public float spawnRate = 15f;
    public float initialDelay = 10f;

    [Header("最大数量")]
    public int maxItems = 1;

    private int currentSpawned = 0;

    void Start()
    {
        InvokeRepeating(nameof(SpawnItem), initialDelay, spawnRate);
    }

    void SpawnItem()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameOver) return;
        if (currentSpawned >= maxItems) return;
        if (lifeItemPrefab == null) return;

        float randX = UnityEngine.Random.Range(minX, maxX);
        float randY = UnityEngine.Random.Range(minY, maxY);
        Vector2 spawnPos = new Vector2(randX, randY);

        Instantiate(lifeItemPrefab, spawnPos, Quaternion.identity);
        currentSpawned++;
    }

    public void OnItemCollected()
    {
        currentSpawned = Mathf.Max(0, currentSpawned - 1);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 1);
        Gizmos.DrawWireCube(center, size);
    }
}
