using UnityEngine;

[RequireComponent(typeof(AudioSource))] // 마이크 입력을 위해 AudioSource가 필요합니다.
public class Player_Controler : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

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
    private Animator anim;
    private AudioSource audioSource;
    private string micDevice;
    private float[] samples = new float[128];
    private float lastAttackTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 소리 감지
        float volume = GetMicrophoneVolume();

        // 공격 (Z 키)
        if (Time.time > lastAttackTime + attackCooldown) // 쿨다운 확인
        {
            if (Input.GetKeyDown(KeyCode.Z) || volume > voiceSensitivity)
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

        // 공격 범위 내의 적 감지
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // 감지된 모든 적에게 데미지 전달
        foreach(Collider2D enemyCollider in hitEnemies)
        {
            EnemyMovement enemy = enemyCollider.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.TakeDamage();
                break; // 한 명의 적만 공격하고 반복문을 종료합니다.
            }
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

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
