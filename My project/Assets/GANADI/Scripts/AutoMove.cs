using UnityEngine;

public class AutoMove : MonoBehaviour
{
    public float speed = 5f;       // 이동 속도
    public float rotationSpeed = 100f; // 아이템 회전 속도
    public float lifeTime = 10f;   // 생존 시간 (화면 밖으로 나가면 삭제용)

    void Start()
    {
        // 생성된 후 일정 시간이 지나면 메모리 절약을 위해 삭제
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // LevelManager의 isGameActive가 false이면 움직이지 않음
        if (!LevelManager.Instance.isGameActive) return;

        // 왼쪽으로 계속 이동
        transform.Translate(Vector3.left * LevelManager.Instance.currentSpeed * Time.deltaTime, Space.World);

        // 만약 이 오브젝트의 태그가 "Item"이라면, 회전 효과를 줍니다.
        if (gameObject.CompareTag("Item"))
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }
}