using UnityEngine;

public class Collectiblediver : MonoBehaviour
{
    [Header("加分配置")]
    public int scorePerItem = 10; 
    public float waterLevel = 0f; 

    private bool isCollected = false; 

    private void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.CompareTag("Player") && !isCollected && other.transform.position.y < waterLevel)
        {
            DiverCollector collector = other.GetComponent<DiverCollector>();
            if (collector != null)
            {
                // 尝试收集物品
                bool collectSuccess = collector.CollectItem(this);
                if (collectSuccess)
                {
                    isCollected = true;
                    gameObject.SetActive(false); // 收集后隐藏物体（而非销毁，便于复用）
                }
            }
        }
    }

 
    public void AddScoreWhenAboveWater()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scorePerItem);
            Debug.Log($"收集物品加分：{scorePerItem}，当前总分：{GameManager.Instance.score}");
        }
        else
        {
            Debug.LogError("未找到GameManager，无法加分！");
        }
    }

    public void ResetItem()
    {
        isCollected = false;
        gameObject.SetActive(true);
    }
}