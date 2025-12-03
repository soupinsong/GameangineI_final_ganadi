using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // 1. Inspector에서 드래그하여 할당할 적 프리팹 변수
    public GameObject enemyPrefab; 
    // 2. 적 생성 간격 (예: 3초에 한 번)
    public float spawnInterval = 3f; 
    // 3. 화면에 동시에 존재할 수 있는 최대 적의 수
    public int maxEnemiesOnScreen = 5;
    // 4. 게임 전체에서 생성할 총 적의 수
    public int totalEnemiesToSpawn = 20;
    // 3. 적이 처음 생성될 때까지의 시간 (선택 사항)
    public float spawnDelay = 1f;

    private int enemiesSpawned = 0;

    void Start()
    {
        // 일정 시간(spawnDelay) 후부터 일정 간격(spawnInterval)으로 SpawnEnemy 함수를 반복 실행
        InvokeRepeating("SpawnEnemy", spawnDelay, spawnInterval);
    }
    void SpawnEnemy()
    {
        if (enemiesSpawned >= totalEnemiesToSpawn)
        {
            CancelInvoke("SpawnEnemy"); // 총 생성 수를 초과하면 더 이상 스폰하지 않음
            return;
        }

        // "Enemy" 태그를 가진 모든 게임 오브젝트를 찾습니다.
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // 현재 생성된 적의 수가 최대치보다 적을 경우에만 새로운 적을 생성합니다.
        if (enemies.Length < maxEnemiesOnScreen)
        {
            // 4. enemyPrefab을 현재 스포너 오브젝트의 위치와 회전으로 생성 (Instantiate)
            // (transform.position은 스포너 오브젝트, 즉 초코우유 건물 앞 위치입니다.)
            Instantiate(enemyPrefab, transform.position, transform.rotation);
            enemiesSpawned++;
        }
    }
}