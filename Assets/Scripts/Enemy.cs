using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public int scoreValue = 10;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null)
        {
            Debug.Log("player==null");
            return;
        }
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        Debug.Log("追踪玩家");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerInvincible inv = other.GetComponent<PlayerInvincible>();
        if (inv != null && inv.IsInvincible())
        {
            return; // 直接跳过，不造成伤害
        }
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.LoseLife();
            Destroy(gameObject);
        }
    }

    public void DestroyEnemy()
    {
        GameManager.Instance.AddScore(scoreValue);
        Destroy(gameObject);
    }
}
