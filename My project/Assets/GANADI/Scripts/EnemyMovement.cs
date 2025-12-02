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
        
        // [중요 수정] Rigidbody 타입을 Kinematic으로 변경하여 물리적으로 밀지 않도록 설정
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.useFullKinematicContacts = true; // 다른 오브젝트와 충돌 감지는 유지

        // 부드러운 움직임을 위한 보간 설정
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        
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
            rb.linearVelocity = Vector2.zero;
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

            // [핵심 수정] Kinematic Rigidbody는 MovePosition으로 움직여야 합니다.
            rb.MovePosition(transform.position + new Vector3(moveDir * moveSpeed * Time.fixedDeltaTime, 0, 0));
        }
        else
        {
            // 멈춤 (Kinematic에서는 별도 처리 불필요)
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

    // 플레이어로부터 공격을 받았을 때 호출될 함수
    public void TakeDamage()
    {
        Destroy(gameObject);
    }
}