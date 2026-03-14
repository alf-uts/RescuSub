using UnityEngine;
using UnityEngine.UI;

public class DiverCollector : MonoBehaviour
{
    [Header("�ռ�����")]
    public int maxCollectCount = 3; // ����ռ�������ÿ�����3����
    public float waterLevel = 0f; // ��OxygenLevelһ�µ�ˮλ��

    [Header("UI��ʾ")]
    public Text collectCountText; // ��ʾ�ռ�������Text���

    private int currentCollectCount = 0; // ��ǰ�ռ�����
    private Collectiblediver[] collectedItems = new Collectiblediver[3]; // �洢���ռ�����Ʒ

    private void Start()
    {
        // ��ʼ��UI��ʾ
        UpdateCollectUI();

        // ��δ��ֵˮλ�ߣ����Դ�OxygenLevelͬ��
        OxygenLevel oxygen = GetComponent<OxygenLevel>();
        if (oxygen != null)
        {
            waterLevel = oxygen.waterLevel;
        }
    }

    private void Update()
    {
        // �������Ƿ񸡳�ˮ�棨Y �� ˮλ�ߣ����������ռ�����Ʒ
        if (transform.position.y >= waterLevel && currentCollectCount > 0)
        {
            GiveScoreForCollectedItems(); // ����ˮ��ӷ�
            ClearCollectedItems(); // ����ռ�����Ʒ
        }
    }

 
  
    public bool CollectItem(Collectiblediver item)
    {
      
        if (currentCollectCount >= maxCollectCount)
        {
            Debug.LogWarning("���ռ�3����Ʒ���޷������ռ���");
            return false;
        }

       
        collectedItems[currentCollectCount] = item;
        currentCollectCount++;
        UpdateCollectUI();
        Debug.Log($"�ռ���Ʒ�ɹ�����ǰ������{currentCollectCount}/{maxCollectCount}");
        return true;
    }

    /// <summary>
    /// ����ˮ��ʱΪ�����ռ�����Ʒ�ӷ�
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
    /// ������ռ�����Ʒ���ӷֺ����ã�
    /// </summary>
    private void ClearCollectedItems()
    {
        currentCollectCount = 0;
        collectedItems = new Collectiblediver[maxCollectCount];
        UpdateCollectUI();
        Debug.Log("����ˮ�棬������ռ�����Ʒ");
    }

    /// <summary>
    /// �����ռ�������UI��ʾ
    /// </summary>
    private void UpdateCollectUI()
    {
        if (collectCountText != null)
        {
            collectCountText.text = $"Carrying divers：{currentCollectCount}/{maxCollectCount}";
        }
        else
        {
            Debug.LogWarning("�ռ�����Textδ��ֵ��");
        }
    }
}