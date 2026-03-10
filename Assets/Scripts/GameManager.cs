using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score = 0;
    public int playerLives = 3;

    public bool gameOver = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score: " + score);
    }

    public void LoseLife()
    {
        if (gameOver) return;

        playerLives--;

        Debug.Log("Lives Remaining: " + playerLives);

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
}
