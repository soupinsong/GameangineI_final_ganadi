using UnityEngine;
using System.Collections;

// 다른 스크립트와의 이름 충돌을 피하기 위해 보스 타워 전용 난이도 열거형을 정의합니다.
public enum BossTowerDifficulty { VeryEasy, Easy, Normal }

[RequireComponent(typeof(SpriteRenderer))]
public class BossTowerController : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("인스펙터에서 이 보스의 난이도를 직접 설정합니다.")]
    public BossTowerDifficulty bossDifficulty = BossTowerDifficulty.Normal;

    [Tooltip("난이도별 체력 (매우 쉬움, 쉬움, 보통)")]
    public int[] healthByDifficulty = { 5, 7, 8 };
    private int maxHealth;
    private int currentHealth;

    [Header("Special Attack Settings")]
    public GameObject specialAttackProjectile; // 특수 공격 총알 프리팹
    public Transform specialAttackSpawnPoint;  // 특수 공격 발사 위치
    public float specialAttackCooldown = 5f;   // 특수 공격 쿨타임
    public int numberOfProjectiles = 5;        // 한 번에 발사할 총알 개수
    public float spreadAngle = 45f;            // 총알이 퍼지는 각도
    public float projectileSpeed = 8f;         // 특수 공격 총알 속도

    [Header("Effects")]
    public float invincibilityDuration = 0.5f; // 피격 후 무적 시간
    private bool isInvincible = false;

    // 컴포넌트 참조
    private SpriteRenderer spriteRenderer;
    private Transform playerTransform;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 인스펙터에서 설정된 난이도에 따라 체력 설정
        int difficultyIndex = (int)bossDifficulty;
        if (difficultyIndex < healthByDifficulty.Length)
        {
            maxHealth = healthByDifficulty[difficultyIndex];
        }
        else
        {
            // 배열의 범위를 벗어나는 경우, 가장 높은 난이도의 체력으로 안전하게 설정합니다.
            maxHealth = healthByDifficulty[healthByDifficulty.Length - 1];
        }

        currentHealth = maxHealth;

        // UIManager에 보스 체력 UI 초기화 요청
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateBossHealthUI(currentHealth, maxHealth);
        }

        // 플레이어 찾아오기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        StartCoroutine(SpecialAttackRoutine());
    }

    // 총알 등으로부터 데미지를 받는 함수
    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        // UIManager에 체력 UI 업데이트 요청
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateBossHealthUI(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            // 보스 처치
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowClearPanel();
                Time.timeScale = 0f; // 보스 처치 시 게임 시간 정지
            }
            Destroy(gameObject); // 보스 오브젝트 파괴
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    // 피격 시 붉은색으로 깜빡이는 효과를 위한 코루틴
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        Color originalColor = spriteRenderer.color; // 원래 색상 저장

        float endTime = Time.time + invincibilityDuration;
        while (Time.time < endTime)
        {
            spriteRenderer.color = Color.red; // 붉은색으로 변경
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor; // 원래 색상으로 복원
            yield return new WaitForSeconds(0.1f);
        }
        spriteRenderer.color = originalColor; // 코루틴 종료 시 원래 색상으로 복원
        isInvincible = false;
    }

    // 특수 공격을 주기적으로 실행하는 코루틴
    private IEnumerator SpecialAttackRoutine()
    {
        // 첫 공격 전 약간의 대기 시간
        yield return new WaitForSeconds(specialAttackCooldown / 2);

        while (currentHealth > 0)
        {
            yield return new WaitForSeconds(specialAttackCooldown);

            if (this != null && gameObject.activeInHierarchy)
            {
                PerformSpecialAttack();
            }
        }
    }

    // 특수 공격 실행
    private void PerformSpecialAttack()
    {
        if (specialAttackProjectile == null || specialAttackSpawnPoint == null || playerTransform == null) return;

        Vector2 targetDirection = (playerTransform.position - specialAttackSpawnPoint.position).normalized;
        float startAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - spreadAngle / 2;
        float angleStep = spreadAngle / (numberOfProjectiles - 1);

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);
            GameObject projectile = Instantiate(specialAttackProjectile, specialAttackSpawnPoint.position, rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 velocity = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));
                rb.velocity = velocity * projectileSpeed;
            }
        }
    }
}