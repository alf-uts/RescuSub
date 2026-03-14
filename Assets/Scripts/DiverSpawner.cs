using System.Collections.Generic;
using System;
using UnityEngine;

public class DiverSpawner : MonoBehaviour
{
    public GameObject collectibleItemPrefab; // ���ռ���ƷԤ���壨�����CollectibleItem��
    public int maxSpawnCount = 10; // ���ͬʱ���ɵ���Ʒ����
    public float spawnRate = 2f; // ���ɼ����룩
    public float waterLevel = 0f; // ˮλ�ߣ���Ʒ������ˮ�£�

    
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -15f;
    public float maxY = -5f; // ȷ��������ˮ�£�Y < ˮλ�ߣ�
    public float minZ = 0f;
    public float maxZ = 0f; // 2D��ϷZ��̶�Ϊ0

    private int currentSpawnedCount = 0; // ��ǰ�����ɵ���Ʒ����
    private ObjectPool<Collectiblediver> itemPool; // ����أ�������Ʒ���Ż����ܣ�

    private void Start()
    {
        // ��ʼ�������
        itemPool = new ObjectPool<Collectiblediver>(
            createFunc: () => Instantiate(collectibleItemPrefab).GetComponent<Collectiblediver>(),
            actionOnGet: (item) =>
            {
                item.ResetItem();
                item.transform.position = GetRandomSpawnPosition();
                item.transform.SetParent(transform); // ������������
            },
            actionOnRelease: (item) => item.gameObject.SetActive(false),
            actionOnDestroy: (item) => Destroy(item.gameObject)
        );

        // ��δ��ֵˮλ�ߣ����Դ���ҵ�OxygenLevelͬ��
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            OxygenLevel oxygen = player.GetComponent<OxygenLevel>();
            if (oxygen != null)
            {
                waterLevel = oxygen.waterLevel;
            }
        }

        // ��ʼѭ��������Ʒ
        InvokeRepeating(nameof(SpawnItem), 1f, spawnRate);
    }

    /// <summary>
    /// ���ɵ�����Ʒ
    /// </summary>
    private void SpawnItem()
    {
        // ��Ϸ�������Ѵ������������ʱֹͣ
        if (GameManager.Instance != null && GameManager.Instance.gameOver) return;
        if (currentSpawnedCount >= maxSpawnCount) return;

        // �Ӷ���ػ�ȡ��Ʒ������
        Collectiblediver newItem = itemPool.Get();
        newItem.waterLevel = waterLevel; // ͬ��ˮλ��
        currentSpawnedCount++;
    }

    /// <summary>
    /// ��ȡ�������λ�ã�ȷ����ˮ�£�
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        float randomX = UnityEngine.Random.Range(minX, maxX);
        float randomY = UnityEngine.Random.Range(minY, maxY); // ȷ��Y < ˮλ��
        float randomZ = UnityEngine.Random.Range(minZ, maxZ);
        return new Vector3(randomX, randomY, randomZ);
    }

    /// <summary>
    /// ��Ʒ���ռ��󣬼������ɼ�������CollectibleItem���ã���ѡ��
    /// </summary>
    public void OnItemCollected()
    {
        currentSpawnedCount = Mathf.Max(0, currentSpawnedCount - 1);
    }

    // ������ͼ���ӻ����ɷ�Χ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
        Gizmos.DrawWireCube(center, size);

        // ����ˮλ��
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(minX, waterLevel, minZ), new Vector3(maxX, waterLevel, maxZ));
    }
}

// ���׶���ع����ࣨ������Ʒ������Ƶ��Instantiate/Destroy��
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