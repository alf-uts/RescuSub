using UnityEngine;

public class CollectibleLife : MonoBehaviour
{
    private LifeItemSpawner spawner;

    void Start()
    {
        spawner = FindObjectOfType<LifeItemSpawner>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 尝试增加生命，如果成功则销毁道具
            bool lifeAdded = GameManager.Instance.AddLife();
            
            if (lifeAdded)
            {
                if (spawner != null)
                {
                    spawner.OnItemCollected();
                }
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("生命已满，无法增加");
            }
        }
    }
}
