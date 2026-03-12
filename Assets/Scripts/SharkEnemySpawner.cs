using UnityEngine;

public class SharkEnemySpawner : MonoBehaviour
{
    [Header("鲨鱼预制体")]
    public GameObject sharkEnemyPrefab;

    [Header("生成侧配置（手动指定）")]
    public bool spawnOnLeftSide = true; // 手动选择生成侧
    public float spawnDistanceX = 15f; // 生成点与玩家的X轴距离

    [Header("生成位置参数")]
    public float minSpawnY = -5f; // Y轴最小生成范围
    public float maxSpawnY = 5f; // Y轴最大生成范围
    public float spawnYInterval = 1.5f; // Y轴生成间隔

    [Header("敌人移动参数")]
    public float moveSpeed = 2f; // 鲨鱼移动速度
    public int damageOnHit = 1; // 碰到玩家扣除的生命值

    [Header("屏幕边界（用于销毁敌人）")]
    public float screenLeftX = -20f; // 屏幕左边界X
    public float screenRightX = 20f; // 屏幕右边界X

    [Header("生成频率")]
    public float spawnRate = 3f; // 生成间隔（秒）
    public float initialSpawnDelay = 2f; // 开局延迟生成时间

    private Transform player;
    private float lastSpawnY;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("未找到标签为Player的玩家物体！请检查玩家标签是否为\"Player\"");
            return;
        }

        lastSpawnY = minSpawnY;
        InvokeRepeating(nameof(SpawnSharkEnemy), initialSpawnDelay, spawnRate);
    }

    void SpawnSharkEnemy()
    {
        if (GameManager.Instance == null || GameManager.Instance.gameOver) return;
        if (player == null || sharkEnemyPrefab == null) return;

        // 计算生成位置
        float spawnX = spawnOnLeftSide ? (player.position.x - spawnDistanceX) : (player.position.x + spawnDistanceX);
        float spawnY = GetValidSpawnY();
        Vector2 spawnPosition = new Vector2(spawnX, spawnY);

        // 生成鲨鱼敌人
        GameObject shark = Instantiate(sharkEnemyPrefab, spawnPosition, Quaternion.identity);

        // 获取鲨鱼的SpriteRenderer组件（核心：翻转X轴）
        SpriteRenderer sharkRenderer = shark.GetComponent<SpriteRenderer>();
        if (sharkRenderer != null)
        {
            // 左侧生成：朝向右侧（flipX=false）；右侧生成：朝向左侧（flipX=true）
            sharkRenderer.flipX = !spawnOnLeftSide;
        }
        else
        {
            Debug.LogWarning("鲨鱼预制体缺少SpriteRenderer组件！无法翻转朝向");
        }

        // 赋值鲨鱼移动参数
        SharkEnemy sharkEnemy = shark.GetComponent<SharkEnemy>();
        if (sharkEnemy != null)
        {
            sharkEnemy.moveDirection = spawnOnLeftSide ? 1 : -1;
            sharkEnemy.moveSpeed = moveSpeed;
            sharkEnemy.screenLeftX = screenLeftX;
            sharkEnemy.screenRightX = screenRightX;
            sharkEnemy.damageOnHit = damageOnHit;
        }
        else
        {
            Debug.LogError("鲨鱼预制体未挂载SharkEnemy脚本！请给预制体添加该脚本");
            Destroy(shark);
        }
    }

    private float GetValidSpawnY()
    {
        float nextSpawnY = lastSpawnY + spawnYInterval;
        if (nextSpawnY > maxSpawnY)
        {
            nextSpawnY = minSpawnY;
        }
        lastSpawnY = nextSpawnY;

        float randomOffset = Random.Range(-0.5f, 0.5f);
        return Mathf.Clamp(nextSpawnY + randomOffset, minSpawnY, maxSpawnY);
    }

    void OnDrawGizmosSelected()
    {
        // 绘制Y轴生成范围
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(-spawnDistanceX, minSpawnY), new Vector2(spawnDistanceX, minSpawnY));
        Gizmos.DrawLine(new Vector2(-spawnDistanceX, maxSpawnY), new Vector2(spawnDistanceX, maxSpawnY));

        // 绘制屏幕边界
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(screenLeftX, minSpawnY - 1), new Vector2(screenLeftX, maxSpawnY + 1));
        Gizmos.DrawLine(new Vector2(screenRightX, minSpawnY - 1), new Vector2(screenRightX, maxSpawnY + 1));

        // 绘制当前生成器的生成侧标记
        Gizmos.color = spawnOnLeftSide ? Color.blue : Color.yellow;
        string sideText = spawnOnLeftSide ? "左侧生成器" : "右侧生成器";
        UnityEditor.Handles.Label(transform.position + Vector3.up, sideText);
    }
}