using UnityEngine;

public class PlatformGroupSpawner : MonoBehaviour
{
    [Header("발판 프리팹")]
    public GameObject platformPrefab; 

    [Header("설정")]
    public float minInterval = 3f;  // 발판 그룹 간의 간격(시간)
    public float maxInterval = 5f;
    public float platformGap = 1.5f; // 발판 사이의 간격(거리)

    [Header("높이 범위")]
    public float minY = -1f;
    public float maxY = 2f;

    private float timer;
    private float nextSpawnTime;

    void Start()
    {
        SetNextSpawnTime();
    }

    void Update()
    {
        if (!LevelManager.Instance.isGameActive) return;

        timer += Time.deltaTime;

        if (timer >= nextSpawnTime)
        {
            SpawnPlatformGroup();
            timer = 0;
            SetNextSpawnTime();
        }
    }

    void SpawnPlatformGroup()
    {
        // 1. 이번에 3개를 만들지 4개를 만들지 결정 (3 이상 5 미만 정수 -> 3, 4)
        int count = Random.Range(3, 5);
        
        // 2. 이번 그룹의 높이 결정
        float groupY = Random.Range(minY, maxY);

        // 3. 개수만큼 반복해서 생성
        for (int i = 0; i < count; i++)
        {
            // X 위치를 조금씩 뒤로 미룸 (i * gap)
            Vector3 spawnPos = new Vector3(transform.position.x + (i * platformGap), groupY, 0);
            
            Instantiate(platformPrefab, spawnPos, Quaternion.identity);
        }
    }

    void SetNextSpawnTime()
    {
        nextSpawnTime = Random.Range(minInterval, maxInterval);
    }
}