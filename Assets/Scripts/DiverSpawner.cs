using System.Collections.Generic;
using System;
using UnityEngine;

public class DiverSpawner : MonoBehaviour
{
    [Header("生成配置")]
    public GameObject collectibleItemPrefab; // 可收集物品预制体（需挂载CollectibleItem）
    public int maxSpawnCount = 10; // 最大同时生成的物品数量
    public float spawnRate = 2f; // 生成间隔（秒）
    public float waterLevel = 0f; // 水位线（物品生成在水下）

    [Header("生成范围（相对于生成器）")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -15f;
    public float maxY = -5f; // 确保生成在水下（Y < 水位线）
    public float minZ = 0f;
    public float maxZ = 0f; // 2D游戏Z轴固定为0

    private int currentSpawnedCount = 0; // 当前已生成的物品数量
    private ObjectPool<Collectiblediver> itemPool; // 对象池（复用物品，优化性能）

    private void Start()
    {
        // 初始化对象池
        itemPool = new ObjectPool<Collectiblediver>(
            createFunc: () => Instantiate(collectibleItemPrefab).GetComponent<Collectiblediver>(),
            actionOnGet: (item) =>
            {
                item.ResetItem();
                item.transform.position = GetRandomSpawnPosition();
                item.transform.SetParent(transform); // 归属于生成器
            },
            actionOnRelease: (item) => item.gameObject.SetActive(false),
            actionOnDestroy: (item) => Destroy(item.gameObject)
        );

        // 若未赋值水位线，尝试从玩家的OxygenLevel同步
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            OxygenLevel oxygen = player.GetComponent<OxygenLevel>();
            if (oxygen != null)
            {
                waterLevel = oxygen.waterLevel;
            }
        }

        // 开始循环生成物品
        InvokeRepeating(nameof(SpawnItem), 1f, spawnRate);
    }

    /// <summary>
    /// 生成单个物品
    /// </summary>
    private void SpawnItem()
    {
        // 游戏结束或已达最大生成数量时停止
        if (GameManager.Instance != null && GameManager.Instance.gameOver) return;
        if (currentSpawnedCount >= maxSpawnCount) return;

        // 从对象池获取物品并生成
        Collectiblediver newItem = itemPool.Get();
        newItem.waterLevel = waterLevel; // 同步水位线
        currentSpawnedCount++;
    }

    /// <summary>
    /// 获取随机生成位置（确保在水下）
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        float randomX = UnityEngine.Random.Range(minX, maxX);
        float randomY = UnityEngine.Random.Range(minY, maxY); // 确保Y < 水位线
        float randomZ = UnityEngine.Random.Range(minZ, maxZ);
        return new Vector3(randomX, randomY, randomZ);
    }

    /// <summary>
    /// 物品被收集后，减少生成计数（供CollectibleItem调用，可选）
    /// </summary>
    public void OnItemCollected()
    {
        currentSpawnedCount = Mathf.Max(0, currentSpawnedCount - 1);
    }

    // 场景视图可视化生成范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
        Gizmos.DrawWireCube(center, size);

        // 绘制水位线
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(minX, waterLevel, minZ), new Vector3(maxX, waterLevel, maxZ));
    }
}

// 简易对象池工具类（复用物品，避免频繁Instantiate/Destroy）
public class ObjectPool<T> where T : Component
{
    private readonly Func<T> createFunc;
    private readonly Action<T> actionOnGet;
    private readonly Action<T> actionOnRelease;
    private readonly Action<T> actionOnDestroy;
    private readonly Queue<T> pool = new Queue<T>();

    public ObjectPool(Func<T> createFunc, Action<T> actionOnGet = null, Action<T> actionOnRelease = null, Action<T> actionOnDestroy = null)
    {
        this.createFunc = createFunc;
        this.actionOnGet = actionOnGet;
        this.actionOnRelease = actionOnRelease;
        this.actionOnDestroy = actionOnDestroy;
    }

    public T Get()
    {
        T item;
        if (pool.Count == 0)
        {
            item = createFunc();
        }
        else
        {
            item = pool.Dequeue();
        }
        actionOnGet?.Invoke(item);
        return item;
    }

    public void Release(T item)
    {
        actionOnRelease?.Invoke(item);
        pool.Enqueue(item);
    }

    public void Clear()
    {
        if (actionOnDestroy != null)
        {
            foreach (var item in pool)
            {
                actionOnDestroy(item);
            }
        }
        pool.Clear();
    }
}