using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("소환할 대상 (Prefabs)")]
    public GameObject[] prefabs; // 랜덤으로 소환할 프리팹들

    [Header("소환 주기 설정")]
    public float minInterval = 1.5f; // 최소 대기 시간
    public float maxInterval = 3.0f; // 최대 대기 시간

    [Header("위치 설정 (Y축 범위)")]
    public float minY = -2f; // 최소 높이
    public float maxY = 0f;  // 최대 높이

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
            Spawn();
            timer = 0;
            SetNextSpawnTime();
        }
    }

    public void Spawn()
    {
        if (prefabs.Length == 0) return;

        // 1. 랜덤 프리팹 선택
        int randomIndex = Random.Range(0, prefabs.Length);
        GameObject selectedPrefab = prefabs[randomIndex];

        // 2. 랜덤 Y 위치 결정
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(transform.position.x, randomY, 0);

        // 만약 생성될 프리팹의 태그가 "Tree"라면, z 위치를 0.1로 설정합니다.
        if (selectedPrefab.CompareTag("Tree"))
        {
            spawnPos.z = 0.1f;
        }

        // EndPoint 태그를 가진 오브젝트가 생성되면 관련 로직 처리
        if (selectedPrefab.CompareTag("EndPoint"))
        {
            HandleEndPointSpawn(selectedPrefab, spawnPos);
            return; // EndPoint는 한 번만 생성되므로 여기서 함수 종료
        }

        // 3. 생성
        Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
    }

    private void HandleEndPointSpawn(GameObject selectedPrefab, Vector3 spawnPos)
    {
        // EndPoint 태그를 가진 오브젝트가 생성되면 LevelManager에 알림
        if (selectedPrefab.CompareTag("EndPoint"))
        {
            // 모든 적과 아이템 오브젝트를 찾아서 제거
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                Destroy(enemy);
            }

            GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
            foreach (GameObject item in items)
            {
                Destroy(item);
            }

            LevelManager.Instance.InitiateEndSequence(); // 엔딩 시퀀스 시작 (스폰 및 스크롤 중지)
            GameObject endPointInstance = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
            FindFirstObjectByType<PlayerController>().StartAutoWalk(endPointInstance.transform);
        }
    }

    void SetNextSpawnTime()
    {
        // 다음 소환까지 걸릴 시간을 랜덤으로 결정
        nextSpawnTime = Random.Range(minInterval, maxInterval);
    }
}