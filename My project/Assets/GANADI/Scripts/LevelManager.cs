using UnityEngine;

public enum Difficulty
{
    VeryEasy,
    Easy,
    Normal
}
public class LevelManager : MonoBehaviour
{
    // 어디서든 접근할 수 있게 싱글톤으로 만듭니다.
    public static LevelManager Instance;

    [Header("상태 정보")]
    public int currentLevel = 1;
    public float gameTime = 0f;
    public bool isGameActive = true;  // 게임이 진행중인지 여부
    public float currentSpeed = 5f;

    [Header("난이도 설정")]
    public Difficulty sceneDifficulty = Difficulty.Normal; // 이 씬의 난이도

    [Header("적 생성 주기 (초)")]
    [Tooltip("매우 쉬움, 쉬움, 보통 순서")]
    public float[] minSpawnRatesByDifficulty = { 4.0f, 3.0f, 2.0f };
    [Tooltip("매우 쉬움, 쉬움, 보통 순서")]
    public float[] maxSpawnRatesByDifficulty = { 6.0f, 5.0f, 3.5f };

    [Header("아이템 생성 주기 (초)")]
    [Tooltip("매우 쉬움, 쉬움, 보통 순서")]
    public float[] minItemSpawnRatesByDifficulty = { 5.0f, 4.0f, 3.0f };
    [Tooltip("매우 쉬움, 쉬움, 보통 순서")]
    public float[] maxItemSpawnRatesByDifficulty = { 8.0f, 7.0f, 6.0f };

    [Tooltip("매우 쉬움, 쉬움, 보통 순서")]
    public float[] gameSpeedsByDifficulty = { 5f, 7f, 9f };
    [Tooltip("매우 쉬움, 쉬움, 보통 순서 (초 단위)")]
    public float[] endPointSpawnTimes = { 80f, 120f, 150f };
    private bool endPointSpawned = false;

    [Header("스포너 참조")]
    public ObjectSpawner enemySpawner;
    public ObjectSpawner itemSpawner;
    public ObjectSpawner endPointSpawner;
    public PlatformGroupSpawner platformSpawner;

    void Awake()
    {
        if (Instance == null) Instance = this;

        // 씬의 난이도에 따라 레벨 설정
        SetLevel((int)sceneDifficulty);
    }

    void Update()
    {
        if (!isGameActive) return;

        gameTime += Time.deltaTime;

        // EndPoint 생성 시간이 되었고, 아직 생성되지 않았다면 생성
        if (!endPointSpawned && gameTime >= endPointSpawnTimes[currentLevel])
        {
            endPointSpawner.Spawn();
            endPointSpawned = true;
        }
    }

    void SetLevel(int level)
    {
        currentLevel = level;
        currentSpeed = gameSpeedsByDifficulty[level];
        
        // 스포너가 할당되었는지 확인하고 생성 주기 업데이트
        if (enemySpawner != null)
        {
            enemySpawner.minInterval = GetMinSpawnRate();
            enemySpawner.maxInterval = GetMaxSpawnRate();
        }

        // 아이템 스포너가 할당되었는지 확인하고 생성 주기 업데이트
        if (itemSpawner != null)
        {
            itemSpawner.minInterval = GetMinItemSpawnRate();
            itemSpawner.maxInterval = GetMaxItemSpawnRate();
        }

        Debug.Log($"난이도 설정됨: {(Difficulty)currentLevel}, 속도: {currentSpeed}");
    }

    // 다른 스크립트에서 현재 적 생성 주기를 가져갈 때 쓰는 함수
    public float GetMinSpawnRate() => minSpawnRatesByDifficulty[currentLevel];
    public float GetMaxSpawnRate() => maxSpawnRatesByDifficulty[currentLevel];
    public float GetMinItemSpawnRate() => minItemSpawnRatesByDifficulty[currentLevel];
    public float GetMaxItemSpawnRate() => maxItemSpawnRatesByDifficulty[currentLevel];

    public void EndGame()
    {
        // 이 함수는 플레이어가 EndPoint에 도달했을 때 등, 게임이 완전히 멈춰야 할 때 호출됩니다.
        isGameActive = false;
        Debug.Log("게임이 종료되었습니다.");
    }

    public void InitiateEndSequence()
    {
        isGameActive = false; // 스포너 등 모든 활성 로직 정지
        currentSpeed = 0f; // 배경 스크롤 등 모든 움직임 정지
    }
}