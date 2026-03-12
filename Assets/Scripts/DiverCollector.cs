using UnityEngine;
using UnityEngine.UI;

public class DiverCollector : MonoBehaviour
{
    [Header("收集配置")]
    public int maxCollectCount = 3; // 最大收集数量（每次最多3个）
    public float waterLevel = 0f; // 与OxygenLevel一致的水位线

    [Header("UI显示")]
    public Text collectCountText; // 显示收集数量的Text组件

    private int currentCollectCount = 0; // 当前收集数量
    private Collectiblediver[] collectedItems = new Collectiblediver[3]; // 存储已收集的物品

    private void Start()
    {
        // 初始化UI显示
        UpdateCollectUI();

        // 若未赋值水位线，尝试从OxygenLevel同步
        OxygenLevel oxygen = GetComponent<OxygenLevel>();
        if (oxygen != null)
        {
            waterLevel = oxygen.waterLevel;
        }
    }

    private void Update()
    {
        // 检测玩家是否浮出水面（Y ≥ 水位线），且有已收集的物品
        if (transform.position.y >= waterLevel && currentCollectCount > 0)
        {
            GiveScoreForCollectedItems(); // 浮出水面加分
            ClearCollectedItems(); // 清空收集的物品
        }
    }

 
  
    public bool CollectItem(Collectiblediver item)
    {
      
        if (currentCollectCount >= maxCollectCount)
        {
            Debug.LogWarning("已收集3个物品，无法继续收集！");
            return false;
        }

       
        collectedItems[currentCollectCount] = item;
        currentCollectCount++;
        UpdateCollectUI();
        Debug.Log($"收集物品成功，当前数量：{currentCollectCount}/{maxCollectCount}");
        return true;
    }

    /// <summary>
    /// 浮出水面时为所有收集的物品加分
    /// </summary>
    private void GiveScoreForCollectedItems()
    {
        for (int i = 0; i < currentCollectCount; i++)
        {
            if (collectedItems[i] != null)
            {
                collectedItems[i].AddScoreWhenAboveWater();
            }
        }
    }

    /// <summary>
    /// 清空已收集的物品（加分后重置）
    /// </summary>
    private void ClearCollectedItems()
    {
        currentCollectCount = 0;
        collectedItems = new Collectiblediver[maxCollectCount];
        UpdateCollectUI();
        Debug.Log("浮出水面，已清空收集的物品");
    }

    /// <summary>
    /// 更新收集数量的UI显示
    /// </summary>
    private void UpdateCollectUI()
    {
        if (collectCountText != null)
        {
            collectCountText.text = $"Carrying divers：{currentCollectCount}/{maxCollectCount}";
        }
        else
        {
            Debug.LogWarning("收集数量Text未赋值！");
        }
    }
}