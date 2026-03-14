using System.Collections.Generic;
using System;
using UnityEngine;

public class OxygenSpawner : MonoBehaviour
{
    [Header("氧气瓶预制体")]
    public GameObject oxygenPrefab;

    [Header("生成设置")]
    public int maxSpawnCount = 5;
    public float spawnRate = 5f;
    public float waterLevel = 0f;

    [Header("生成范围")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -15f;
    public float maxY = -5f;
    public float minZ = 0f;
    public float maxZ = 0f;

    private int currentSpawnedCount = 0;
    private ObjectPool<CollectibleOxygen> itemPool;

    private void Start()
    {
        itemPool = new ObjectPool<CollectibleOxygen>(
            createFunc: () => Instantiate(oxygenPrefab).GetComponent<CollectibleOxygen>(),
            actionOnGet: (item) =>
            {
                item.gameObject.SetActive(true);
                item.transform.position = GetRandomSpawnPosition();
                item.transform.SetParent(transform);
            },
            actionOnRelease: (item) => item.gameObject.SetActive(false),
            actionOnDestroy: (item) => Destroy(item.gameObject)
        );

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            OxygenLevel oxygen = player.GetComponent<OxygenLevel>();
            if (oxygen != null)
            {
                waterLevel = oxygen.waterLevel;
            }
        }

        InvokeRepeating(nameof(SpawnOxygen), 2f, spawnRate);
    }

    private void SpawnOxygen()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameOver) return;
        if (currentSpawnedCount >= maxSpawnCount) return;

        CollectibleOxygen newItem = itemPool.Get();
        currentSpawnedCount++;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float randomX = UnityEngine.Random.Range(minX, maxX);
        float randomY = UnityEngine.Random.Range(minY, maxY);
        float randomZ = UnityEngine.Random.Range(minZ, maxZ);
        return new Vector3(randomX, randomY, randomZ);
    }

    public void OnOxygenCollected()
    {
        currentSpawnedCount = Mathf.Max(0, currentSpawnedCount - 1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
        Gizmos.DrawWireCube(center, size);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(minX, waterLevel, minZ), new Vector3(maxX, waterLevel, maxZ));
    }
}
