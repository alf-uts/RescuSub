using UnityEngine;

public class PlayerInvincible : MonoBehaviour
{
    [Header("无敌配置")]
    public float invincibleTime = 3f;       
    public float blinkInterval = 0.15f;  

    [Header("吃什么物品触发无敌")]
    public string collectibleTag = "InvincibleItem"; 

    // 内部变量
    private bool isInvincible = false;
    private float invincibleTimer;
    private SpriteRenderer sr;
    private OxygenLevel oxygen;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        oxygen = GetComponent<OxygenLevel>();
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;

          
            float blink = Mathf.PingPong(Time.time / blinkInterval, 1);
            sr.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0.5f), blink);

       
            if (invincibleTimer <= 0)
            {
                EndInvincible();
            }
        }
    }

 
    public void StartInvincible()
    {
        isInvincible = true;
        invincibleTimer = invincibleTime;
    }

    
    void EndInvincible()
    {
        isInvincible = false;
        sr.color = Color.white; 
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(collectibleTag) && !isInvincible)
        {
            StartInvincible();
            Destroy(other.gameObject); 
        }
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }
}