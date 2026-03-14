using UnityEngine;

public class InvincibleItem : MonoBehaviour
{
    private InvincibleItemSpawner spawner;

    void Start()
    {
        spawner = FindObjectOfType<InvincibleItemSpawner>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
          
            PlayerInvincible inv = other.GetComponent<PlayerInvincible>();
            if (inv != null)
            {
                inv.StartInvincible();
            }

            
            if (spawner != null)
            {
                spawner.OnItemCollected();
            }

            Destroy(gameObject);
        }
    }
}