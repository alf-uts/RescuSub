using UnityEngine;
using UnityEngine.UI; // ����UI�����ռ䣨���룩
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score = 0;
    public int playerLives = 3;
    public int maxLives = 3; // 生命上限
    public bool gameOver = false;

    [Header("UI Text ����")]
    public Text scoreText; 
    public Text livesText; 
public GameObject submarinePrefab;
public Transform spawnPoint;

    [Header("额外生命设置")]
    public int extraLifeScoreThreshold = 1000;
    private int lastExtraLifeScore = 0;

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
        CheckExtraLife();
        Debug.Log("Score: " + score);
        UpdateScoreUI();
    }

    private void CheckExtraLife()
    {
        int extraLivesEarned = score / extraLifeScoreThreshold;
        if (extraLivesEarned > lastExtraLifeScore)
        {
            int newLives = extraLivesEarned - lastExtraLifeScore;
            playerLives += newLives;
            lastExtraLifeScore = extraLivesEarned;
            UpdateLivesUI();
            Debug.Log($"额外生命！ +{newLives} 生命，当前生命: {playerLives}");

            // 同时恢复满氧气
            OxygenLevel oxygen = FindFirstObjectByType<OxygenLevel>();
            if (oxygen != null)
            {
                oxygen.ResetOxygen();
                Debug.Log("氧气已恢复满");
            }
        }
    }

    public void LoseLife(bool respawn = false)
    {
        if (gameOver) return;

        playerLives--;
        Debug.Log("Lives Remaining: " + playerLives);
        UpdateLivesUI();

        if (playerLives <= 0)
        {
            GameOver();
        }
        else if (respawn)
        {
            RespawnSub();
        }
    }

    // 恢复生命（不超过生命上限）
    public void RestoreLife(int amount)
    {
        if (gameOver) return;

        int oldLives = playerLives;
        playerLives = Mathf.Min(playerLives + amount, maxLives);
        int restored = playerLives - oldLives;

        if (restored > 0)
        {
            Debug.Log($"恢复生命 +{restored}，当前生命: {playerLives}/{maxLives}");
            UpdateLivesUI();
        }
        else
        {
            Debug.Log($"生命已满，无法恢复，当前生命: {playerLives}/{maxLives}");
        }
    }

    // 增加生命（返回是否成功增加）
    public bool AddLife()
    {
        if (gameOver) return false;

        if (playerLives < maxLives)
        {
            playerLives++;
            Debug.Log($"增加生命 +1，当前生命: {playerLives}/{maxLives}");
            UpdateLivesUI();
            return true;
        }
        else
        {
            Debug.Log($"生命已满，无法增加，当前生命: {playerLives}/{maxLives}");
            return false;
        }
    }

    // 增加生命上限
    public void IncreaseMaxLives(int amount)
    {
        maxLives += amount;
        Debug.Log($"生命上限提升！当前生命上限: {maxLives}");
    }

    public void RespawnSub()
    {
        if(submarinePrefab !=null && spawnPoint !=null)
        {
            StartCoroutine(RespawnCoroutine());
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSecondsRealtime(1f);
        GameObject newSub=Instantiate(submarinePrefab, spawnPoint.position, Quaternion.identity);
        OxygenLevel oxy=newSub.GetComponent<OxygenLevel>();
        if(oxy != null)
        {
            oxy.ResetOxygen();
        }

        CameraController cam = Object.FindFirstObjectByType<CameraController>();
        if(cam != null)
        cam.SetPlayer(newSub.transform);
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
            scoreText.text = $"Score: {score}"; // �Զ�����ʾ��ʽ
        }
        else
        {
            Debug.LogWarning("����Text���δ��ֵ������Inspector�������", this);
        }
    }

    // ����������ֵUI���·��������ڸ��ã�
    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = $"Lives: {playerLives}"; // �Զ�����ʾ��ʽ
        }
        else
        {
            Debug.LogWarning("����ֵText���δ��ֵ������Inspector�������", this);
        }
    }
}