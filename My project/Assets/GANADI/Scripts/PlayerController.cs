using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("설정 (Settings)")]
    public float jumpForce = 10f; // 점프 높이 (Inspector에서 수정 가능)
    public int maxHealth = 3;     // 최대 체력
    public float invincibilityDuration = 1.5f; // 무적 시간

    [Header("상태 확인용 (Read Only)")]
    public int currentHealth;     // 현재 체력
    public bool isGrounded;       // 바닥에 닿아있는지 여부
    public bool isInvincible = false; // 현재 무적 상태인지 여부

    [Header("아이템 (Item)")]
    public static int nextStageItemCount = 0; // 다음 스테이지에서 사용할 아이템 개수

    private bool isAutoWalking = false;
    private Transform endPointTarget;
    private float autoWalkSpeed = 5f; // 자동 걷기 속도

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth; // 게임 시작 시 체력 초기화

        // UI 매니저에게 알려 체력 UI 업데이트
        UIManager.Instance.UpdateHealthUI(currentHealth);

        // 아이템 UI 초기화
        UIManager.Instance.UpdateItemCountUI(nextStageItemCount);

        // 난이도에 따른 플레이어 설정 적용
        if (LevelManager.Instance != null && LevelManager.Instance.sceneDifficulty == Difficulty.Normal)
        {
            rb.gravityScale = 2.1f;
            jumpForce = 13f;
        }
    }

    void Update()
    {
        if (isAutoWalking)
        {
            AutoWalkToEndPoint();
            return;
        }
        // 점프 입력 (스페이스바) & 바닥에 있을 때만 점프 가능 (2단 점프 방지)
        if (LevelManager.Instance.isGameActive && Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        // 위쪽으로 힘을 가함
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // 점프 효과음 재생
        if (AudioManager.Instance != null && AudioManager.Instance.jumpSfx != null)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Instance.jumpSfx);
        }
        
        // 점프 즉시 바닥에서 떨어졌다고 처리 (연타 방지)
        isGrounded = false;
    }

    // 충돌 감지 함수 (바닥 착지 및 적 충돌)
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. 바닥(Ground) 태그와 충돌했는지 확인
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // 점프 가능 상태로 변경
            Debug.Log("땅에 착지했습니다.");
        }
    }

    // 바닥에서 떨어질 때 처리 (선택 사항: 더 정확한 체크를 위해)
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void TakeDamage(int damage, GameObject enemy)
    {
        if (isInvincible) return; // 무적 상태이면 데미지를 입지 않음

        currentHealth -= damage; // 체력 감소
        Debug.Log("아야! 체력이 깎였습니다. 현재 체력: " + currentHealth);

        // UI 매니저에게 알려 체력 UI 업데이트
        UIManager.Instance.UpdateHealthUI(currentHealth);

        // 충돌 효과음 재생
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.collisionSfx);

        // 적 오브젝트 파괴
        Destroy(enemy);

        // 무적 코루틴 시작
        StartCoroutine(InvincibilityCoroutine());
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // GameManager에 플레이어 사망을 알림
        GameManager.Instance.PlayerDied();

        // 여기에 게임 오버 화면을 띄우거나 씬을 재시작하는 코드를 넣으세요.
        // 예: Time.timeScale = 0; (게임 일시정지)
    }

    // 트리거 충돌 감지 함수 (아이템 획득)
    void OnTriggerEnter2D(Collider2D other)
    {
        // 적(Enemy) 태그와 충돌했는지 확인
        if (other.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1, other.gameObject); // 데미지와 함께 적 오브젝트 전달
        }

        // 아이템(Item) 태그와 접촉했는지 확인
        if (other.gameObject.CompareTag("Item"))
        {
            CollectItem(other.gameObject);
        }

        // EndPoint와 접촉했는지 확인
        if (other.gameObject.CompareTag("EndPoint"))
        {
            if (isAutoWalking)
            {
                isAutoWalking = false; // 자동 걷기 중지
                rb.linearVelocity = Vector2.zero; // 움직임 정지
                // 자동 걷기 종료 후 다시 X축 위치 고정
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            }
            // GameManager에 EndPoint 도달 및 아이템 개수 전달
            GameManager.Instance.ReachEndPoint(nextStageItemCount);
        }
    }

    void CollectItem(GameObject item)
    {
        nextStageItemCount++; // 아이템 개수 증가
        Debug.Log("아이템을 획득했습니다! 다음 스테이지 아이템: " + nextStageItemCount);

        // UI 매니저에게 알려 아이템 개수 UI 업데이트
        UIManager.Instance.UpdateItemCountUI(nextStageItemCount);

        // 아이템 획득 효과음 재생
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.itemGetSfx);

        Destroy(item); // 아이템 오브젝트 파괴
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true; // 무적 상태 시작

        // 무적 시간 동안 깜빡이는 효과
        float endTime = Time.time + invincibilityDuration;
        while (Time.time < endTime)
        {
            // 스프라이트를 껐다 켬
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
        }

        // 무적 시간이 끝나면 다시 원래 상태로 복구
        spriteRenderer.enabled = true;
        isInvincible = false; // 무적 상태 종료
    }

    public void StartAutoWalk(Transform target)
    {
        isAutoWalking = true;
        endPointTarget = target;
        // 자동 걷기를 위해 X축 위치 고정 해제
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void AutoWalkToEndPoint()
    {
        if (endPointTarget == null) return;

        // 오른쪽으로 일정한 속도로 이동 (중력은 그대로 적용됨)
        rb.linearVelocity = new Vector2(autoWalkSpeed, rb.linearVelocity.y);
    }

    /// <summary>
    /// 수집한 아이템 개수를 초기화합니다.
    /// </summary>
    public static void ResetItemCount()
    {
        nextStageItemCount = 0;
    }
}