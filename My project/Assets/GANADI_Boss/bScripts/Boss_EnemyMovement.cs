using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Boss_EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // 적의 이동 속도

    private Transform playerTransform;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 적의 Collider2D를 Trigger로 설정합니다.
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;


        // Rigidbody 타입을 Kinematic으로 변경하여 물리적으로 밀지 않도록 설정
        rb.bodyType = RigidbodyType2D.Kinematic;
        // Kinematic Body는 중력의 영향을 받지 않으므로 gravityScale을 0으로 설정합니다.
        rb.gravityScale = 0;
        // 회전 방지
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
            return;
        }

        // 플레이어의 수평 방향으로만 이동하도록 수정
        Vector2 direction = new Vector2(playerTransform.position.x - transform.position.x, 0).normalized;

        Vector2 newPos = rb.position + direction * moveSpeed * Time.fixedDeltaTime;

        // Kinematic Rigidbody는 MovePosition으로 움직여야 합니다.
        rb.MovePosition(newPos);

        // 플레이어의 x 위치에 따라 방향 뒤집기
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // 플레이어로부터 공격을 받았을 때 호출될 함수
    public void TakeDamage()
    {
        Debug.Log("적이 맞았음");
        Destroy(gameObject);
    }
}