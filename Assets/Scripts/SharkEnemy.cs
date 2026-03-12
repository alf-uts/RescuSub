using UnityEngine;

public class SharkEnemy : MonoBehaviour
{
  
    [HideInInspector] public int moveDirection; // 移动方向：1=右，-1=左
    [HideInInspector] public float moveSpeed; // 移动速度
    [HideInInspector] public float screenLeftX; // 屏幕左边界
    [HideInInspector] public float screenRightX; // 屏幕右边界
    [HideInInspector] public int damageOnHit = 1; // 碰到玩家扣血数

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

  
    private void OnTriggerEnter2D(Collider2D other)
    {
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
            Debug.LogError("鲨鱼敌人缺少Collider2D组件！已自动添加BoxCollider2D", this);
            gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
        }
    }
}