using UnityEngine;
using UnityEngine.UI; // 引入UI命名空间（必须）

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score = 0;
    public int playerLives = 3;
    public bool gameOver = false;

    [Header("UI Text 引用")]
    public Text scoreText; 
    public Text livesText; 

    void Awake()
    {
    
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        UpdateScoreUI();
        UpdateLivesUI();
    }

    public void AddScore(int amount)
    {
        if (gameOver) return; 

        score += amount;
        Debug.Log("Score: " + score);
        UpdateScoreUI(); 
    }

    public void LoseLife()
    {
        if (gameOver) return;

        playerLives--;
        Debug.Log("Lives Remaining: " + playerLives);
        UpdateLivesUI(); 

        if (playerLives <= 0)
        {
            GameOver();
        }
    }


    void GameOver()
    {
        gameOver = true;
        Debug.Log("GAME OVER");
        Time.timeScale = 0f;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}"; // 自定义显示格式
        }
        else
        {
            Debug.LogWarning("分数Text组件未赋值！请在Inspector面板拖入", this);
        }
    }

    // 单独的生命值UI更新方法（便于复用）
    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = $"Lives: {playerLives}"; // 自定义显示格式
        }
        else
        {
            Debug.LogWarning("生命值Text组件未赋值！请在Inspector面板拖入", this);
        }
    }
}