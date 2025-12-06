using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource), typeof(Collider2D))]
public class newbossPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    public float invincibilityDuration = 0.5f; // 무적 시간
    private bool isInvincible = false;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    private float groundCheckDistance = 0.1f; // 콜라이더 아래로 체크할 거리
    private bool isGrounded;

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float attackCooldown = 1f; // 공격 쿨다운
    private float lastAttackTime = -1f;
    private int remainingAttacks;

    [Header("Voice Attack Settings")]
    public float voiceSensitivity = 0.1f;
    private AudioSource audioSource;
    private string micDevice;
    private float[] samples = new float[128];
    private bool isMicAvailable = false;

    // 컴포넌트 참조
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Collider2D playerCollider;

    void Start()
    {
        // 컴포넌트 초기화
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        playerCollider = GetComponent<Collider2D>();

        // 필수 컴포넌트 확인
        if (rb == null) Debug.LogError("Rigidbody2D 컴포넌트가 없습니다.");
        if (anim == null) Debug.LogError("Animator 컴포넌트가 없습니다.");

        // 체력 초기화 및 UI 연동
        currentHealth = GameManager.Instance != null ? GameManager.Instance.PlayerHealth : maxHealth;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealthUI(currentHealth);
        }

        // 공격 횟수 초기화 및 UI 연동
        if (GameManager.Instance != null)
        {
            remainingAttacks = GameManager.Instance.CollectedItemCount;
        }
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateItemCountUI(remainingAttacks);
        }

        // 마이크 초기화
        InitializeMicrophone();
    }

    void Update()
    {
        // 바닥 감지
        CheckIfGrounded();

        // 이동 처리
        HandleMovement();

        // 점프 처리
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // 공격 처리
        HandleAttack();
    }

    private void CheckIfGrounded()
    {
        // 플레이어 콜라이더의 크기와 위치를 기반으로 BoxCast를 사용하여 지면을 감지합니다.
        // 이 방법은 단일 Raycast보다 더 안정적으로 지면을 감지할 수 있습니다.
        Vector2 boxSize = new Vector2(playerCollider.bounds.size.x * 0.9f, 0.1f);
        Vector2 castOrigin = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.min.y);
        RaycastHit2D hit = Physics2D.BoxCast(castOrigin, boxSize, 0f, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // 애니메이션 파라미터 설정
        anim.SetInteger("Speed", Mathf.RoundToInt(Mathf.Abs(moveInput)));

        // 캐릭터 방향 전환
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // 점프 효과음 재생
        if (AudioManager.Instance != null && AudioManager.Instance.jumpSfx != null)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Instance.jumpSfx);
        }
    }

    private void HandleAttack()
    {
        float volume = GetMicrophoneVolume();
        bool useMic = isMicAvailable && GameManager.Instance != null && GameManager.Instance.IsMicOn;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if ((Input.GetKeyDown(KeyCode.Z) || (useMic && volume > voiceSensitivity)) && remainingAttacks > 0)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        lastAttackTime = Time.time;
        remainingAttacks--;

        // 공격 애니메이션 실행
        anim.SetTrigger("Attack");

        // 공격 효과음 재생
        if (AudioManager.Instance != null && AudioManager.Instance.attackSfx != null)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Instance.attackSfx);
        }

        // 총알 발사
        if (projectilePrefab != null)
        {
            GameObject projectileGO = Instantiate(projectilePrefab, playerCollider.bounds.center, Quaternion.identity);
            Rigidbody2D projectileRb = projectileGO.GetComponent<Rigidbody2D>();
            if (projectileRb != null)
            {
                float direction = transform.localScale.x;
                projectileRb.velocity = new Vector2(projectileSpeed * direction, 0);
                projectileGO.transform.localScale = new Vector3(direction, 1, 1);
            }
        }

        // UI 업데이트
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateItemCountUI(remainingAttacks);
        }
    }

    private void InitializeMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0];
            audioSource.clip = Microphone.Start(micDevice, true, 1, AudioSettings.outputSampleRate);
            audioSource.loop = true;
            while (!(Microphone.GetPosition(micDevice) > 0)) { } // 마이크 시작 대기
            audioSource.Play();
            isMicAvailable = true;
        }
        else
        {
            Debug.LogWarning("사용 가능한 마이크가 없습니다.");
            isMicAvailable = false;
        }
    }

    private float GetMicrophoneVolume()
    {
        if (!isMicAvailable) return 0;

        float levelMax = 0;
        audioSource.GetOutputData(samples, 0);
        foreach (float sample in samples)
        {
            levelMax = Mathf.Max(levelMax, Mathf.Abs(sample));
        }
        return levelMax;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerHealth = currentHealth;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealthUI(currentHealth);
        }

        if (AudioManager.Instance != null && AudioManager.Instance.collisionSfx != null)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Instance.collisionSfx);
        }

        if (currentHealth <= 0)
        {
            // 보스전에서는 GameManager의 PlayerDied()를 직접 호출하지 않고,
            // UIManager를 통해 게임오버 패널을 활성화합니다.
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameOverPanel();
                Time.timeScale = 0f; // 게임 시간을 멈춥니다.
            }
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        float endTime = Time.time + invincibilityDuration;
        while (Time.time < endTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        spriteRenderer.enabled = true;
        isInvincible = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }
}
