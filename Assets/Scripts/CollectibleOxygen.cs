using UnityEngine;

public class CollectibleOxygen : MonoBehaviour
{
    [Header("氧气增加量")]
    public float oxygenAmount = 30f;

    [Header("是否被收集")]
    private bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            OxygenLevel oxygen = other.GetComponent<OxygenLevel>();
            if (oxygen != null)
            {
                oxygen.AddOxygen(oxygenAmount);
                isCollected = true;

                OxygenSpawner spawner = FindFirstObjectByType<OxygenSpawner>();
                if (spawner != null)
                {
                    spawner.OnOxygenCollected();
                }

                Destroy(gameObject);
                Debug.Log($"收集氧气瓶，增加氧气: {oxygenAmount}");
            }
        }
    }
}
