using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))] // 마이크 입력을 위해 AudioSource가 필요합니다.
public class Player_Controler : MonoBehaviour // 클래스 이름이 Player_Controler로 되어있어 그대로 사용합니다.
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;
    public float invincibilityDuration = 1.5f; // 무적 시간
    private bool isInvincible = false; // 현재 무적 상태인지 여부

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    [Header("Voice Attack")]
    public float voiceSensitivity = 0.1f; // 목소리 감지 민감도 (0.0 ~ 1.0)
    public float attackCooldown = 0.5f;   // 공격 쿨다운 (초)

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private AudioSource audioSource;
    private string micDevice;
    private float[] samples = new float[128];
    private float lastAttackTime = 0f;

    [Header("Game Data")]
    private int remainingAttacks; // 남은 공격 횟수

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D 컴포넌트를 찾을 수 없습니다. 플레이어 오브젝트에 추가해주세요.");
        }

        if (groundCheck == null)
        {
            Debug.LogError("Ground Check Transform이 할당되지 않았습니다. 플레이어 자식으로 빈 오브젝트를 만들어 할당해주세요.");
        }

        if (attackPoint == null)
        {
            Debug.LogError("Attack Point Transform이 할당되지 않았습니다. 플레이어 자식으로 빈 오브젝트를 만들어 할당해주세요.");
        }

        // GameManager에서 체력 정보를 가져옵니다.
        if (GameManager.Instance != null)
        {
            currentHealth = GameManager.Instance.PlayerHealth;
            if (UIManager.Instance != null) UIManager.Instance.UpdateHealthUI(currentHealth);
        }

        // 마이크 초기화
        if (Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0];
            audioSource.clip = Microphone.Start(micDevice, true, 1, AudioSettings.outputSampleRate);
            audioSource.loop = true;
            while (!(Microphone.GetPosition(micDevice) > 0)) { } // 마이크가 시작될 때까지 대기
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("사용 가능한 마이크가 없습니다. 음성 공격을 사용할 수 없습니다.");
        }

        // GameManager에서 수집한 아이템 개수를 가져와 남은 공격 횟수로 설정합니다.
        if (GameManager.Instance != null)
        {
            remainingAttacks = GameManager.Instance.CollectedItemCount;
        }

        // UIManager를 통해 초기 아이템 개수(공격 횟수)를 UI에 업데이트합니다.
        if (UIManager.Instance != null) UIManager.Instance.UpdateItemCountUI(remainingAttacks);
    }

    void Update()
    {
        // 바닥 감지
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // 좌우 이동 (A, D 키)
        moveInput = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            moveInput = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveInput = 1f;
        }

        // 애니메이션 제어
        if (anim != null)
        {
            // moveInput의 절대값을 정수로 변환하여 Speed 파라미터 설정 (이동 시 1, 멈춤 시 0)
            anim.SetInteger("Speed", Mathf.RoundToInt(Mathf.Abs(moveInput)));
        }
        // 캐릭터 방향 전환
        if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        // 점프 (Space 키)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        // 소리 감지
        float volume = GetMicrophoneVolume();

        // 공격 (Z 키)
        if (Time.time > lastAttackTime + attackCooldown) // 쿨다운 확인
        {
            if ((Input.GetKeyDown(KeyCode.Z) || volume > voiceSensitivity) && remainingAttacks > 0)
            {
                Attack();
                lastAttackTime = Time.time; // 마지막 공격 시간 기록
            }
        }
    }

    float GetMicrophoneVolume()
    {
        if (micDevice == null) return 0;

        float levelMax = 0;
        audioSource.GetOutputData(samples, 0);
        foreach (float sample in samples)
        {
            levelMax = Mathf.Max(levelMax, Mathf.Abs(sample));
        }
        return levelMax;
    }

    void Attack()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("Attack Point가 할당되지 않아 공격할 수 없습니다. 플레이어 오브젝트의 자식으로 AttackPoint를 생성하고 할당해주세요.");
            return;
        }

        // 공격 애니메이션 실행
        anim.SetTrigger("Attack");

        // 남은 공격 횟수 차감 및 UI 업데이트
        remainingAttacks--;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateItemCountUI(remainingAttacks);
        }
        // 공격 범위 내의 적 감지
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // 감지된 모든 적에게 데미지 전달
        foreach(Collider2D enemyCollider in hitEnemies)
        {
            Boss_EnemyMovement enemy = enemyCollider.GetComponent<Boss_EnemyMovement>();
            if (enemy != null)
            {
                enemy.TakeDamage(); // TakeDamage가 있는 적을 찾으면
                break; // 한 명의 적만 공격하고 반복문을 종료합니다.
            }
        }
    }

    // 적에게 공격받았을 때 호출될 함수 (임시로 추가)
    public void TakeDamage(int damage)
    {
        if (isInvincible) return; // 무적 상태이면 데미지를 입지 않음

        currentHealth -= damage;
        GameManager.Instance.PlayerHealth = currentHealth; // GameManager에 체력 업데이트

        // 충돌 효과음 재생
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.collisionSfx);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealthUI(currentHealth);
        }

        // 무적 코루틴 시작
        StartCoroutine(InvincibilityCoroutine());

        if (currentHealth <= 0)
        {
            // 플레이어 사망 처리 (GameManager에 위임)
            GameManager.Instance.PlayerDied();
        }
    }

    // 충돌 감지
    void OnTriggerEnter2D(Collider2D other)
    {
        // 적(Enemy) 태그와 충돌했는지 확인
        if (other.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1); // 데미지 1을 입음
        }
    }
    void FixedUpdate()
    {
        // 물리 기반 이동 적용
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
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

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
