using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // 적의 이동 속도

    private Transform playerTransform;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // [중요 설정] 부드러운 움직임을 위한 보간 설정
        // 물리 계산 사이에 비어있는 프레임을 부드럽게 채워줍니다.
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        
        // 적이 굴러다니지 않도록 Z축 회전 고정
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }

    void FixedUpdate()
    {
        // 플레이어가 없으면 움직임을 멈춥니다.
        if (playerTransform == null)
        {
            // [수정됨] linearVelocity 대신 velocity 사용 (2022 버전 호환)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); 
            return;
        }

        // 1. 플레이어 방향 계산 (x축 거리)
        float directionX = playerTransform.position.x - transform.position.x;

        // 2. 속도(Velocity)를 이용한 부드러운 이동
        // 거리가 0.1보다 멀 때만 이동합니다.
        if (Mathf.Abs(directionX) > 0.1f)
        {
            // 방향(1 또는 -1)을 구합니다.
            float moveDir = Mathf.Sign(directionX);
            
            // [핵심 수정] 
            // X축: 플레이어 방향으로 moveSpeed만큼 속도 설정
            // Y축: 원래 가지고 있던 속도(중력 영향)를 그대로 유지! (rb.velocity.y)
            rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            // 플레이어와 가까워지면 X축 속도만 0으로 만들어 멈춥니다. (Y축 중력은 유지)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        // 3. 방향 뒤집기 (기존 코드와 동일)
        if (directionX > 0.1f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (directionX < -0.1f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}