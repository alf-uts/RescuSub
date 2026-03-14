using UnityEngine;

public class SharkEnemy : MonoBehaviour
{
    [HideInInspector] public int moveDirection;
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public float screenLeftX;
    [HideInInspector] public float screenRightX;
    [HideInInspector] public int damageOnHit = 1;

    [HideInInspector] public int maxHealth = 1;
    [HideInInspector] public int currentHealth;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameOver) return;

        transform.Translate(Vector2.right * moveDirection * moveSpeed * Time.deltaTime);

        CheckOutOfScreen();
    }

    private void CheckOutOfScreen()
    {
        if (moveDirection == 1 && transform.position.x > screenRightX)
        {
            Destroy(gameObject);
        }
        else if (moveDirection == -1 && transform.position.x < screenLeftX)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (spriteRenderer != null)
        {
            StartCoroutine(FlashDamage());
        }

        if (currentHealth <= 0)
        {
            DestroyEnemy();
        }
    }

    private System.Collections.IEnumerator FlashDamage()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerInvincible inv = other.GetComponent<PlayerInvincible>();
        if (inv != null && inv.IsInvincible())
        {
            return;
        }

        if (other.CompareTag("Player") && GameManager.Instance != null && !GameManager.Instance.gameOver)
        {
            for (int i = 0; i < damageOnHit; i++)
            {
                GameManager.Instance.LoseLife();
            }
            Destroy(gameObject);
        }
    }

    public void DestroyEnemy()
    {
        GameManager.Instance.AddScore(10);
        Destroy(gameObject);
    }

    private void Awake()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        else
        {
            Debug.LogError("敌人缺少Collider2D组件，将自动添加BoxCollider2D", this);
            gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
        }
    }
}
