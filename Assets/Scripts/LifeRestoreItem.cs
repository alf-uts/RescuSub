using UnityEngine;

public class LifeRestoreItem : MonoBehaviour
{
    [Header("恢复生命数量")]
    public int restoreAmount = 1;

    private LifeRestoreItemSpawner spawner;

    void Start()
    {
        spawner = FindFirstObjectByType<LifeRestoreItemSpawner>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 恢复生命（不会超过上限）
            GameManager.Instance.RestoreLife(restoreAmount);

            // 通知生成器道具被收集
            if (spawner != null)
            {
                spawner.OnItemCollected();
            }

            Destroy(gameObject);
        }
    }
}
