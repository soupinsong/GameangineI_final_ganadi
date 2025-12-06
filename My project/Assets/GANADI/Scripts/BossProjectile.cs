using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float lifetime = 3f; // 발사체가 사라지기까지의 시간 (초)
    public GameObject impactEffect; // 충돌 시 생성될 파티클/이펙트 프리팹

    void Start()
    {
        // 일정 시간이 지나면 자동으로 사라지도록 설정
        Destroy(gameObject, lifetime);
    }

    // 다른 Collider2D와 충돌했을 때 호출됩니다. (Is Trigger가 켜져 있어야 함)
    void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어와 충돌한 경우는 무시합니다.
        if (other.CompareTag("Player"))
        {
            return;
        }

        bool hitSomething = false;

        // "Enemy" 태그를 가진 객체와 충돌 시 처리
        if (other.CompareTag("Enemy"))
        {
            Boss_EnemyMovement enemy = other.GetComponent<Boss_EnemyMovement>();
            if (enemy != null) enemy.TakeDamage();
            else Destroy(other.gameObject); // Boss_EnemyMovement 스크립트가 없다면 그냥 파괴
            hitSomething = true;
        }
        // "Ground" 레이어에 속한 객체(벽)와 충돌 시 처리
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground")) // 벽 레이어 이름이 "Ground"라고 가정
        {
            hitSomething = true;
        }

        // 적 또는 벽에 부딪혔다면 발사체를 파괴합니다.
        if (hitSomething)
        {
            HandleImpact();
        }
    }

    // 충돌 시 처리
    void HandleImpact()
    {
        // 충돌 이펙트가 설정되어 있다면, 충돌 위치에 이펙트를 생성합니다.
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        // 발사체 오브젝트를 즉시 파괴합니다.
        Destroy(gameObject);
    }
}