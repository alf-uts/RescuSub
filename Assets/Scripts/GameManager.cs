using UnityEngine;
using UnityEngine.UI; // ����UI�����ռ䣨���룩
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score = 0;
    public int playerLives = 3;
    public bool gameOver = false;

    [Header("UI Text ����")]
    public Text scoreText; 
    public Text livesText; 
public GameObject submarinePrefab;
public Transform spawnPoint;

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
        if(cam = null)
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