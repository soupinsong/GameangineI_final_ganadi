using UnityEngine;

public class InfiniteScroller : MonoBehaviour
{
    [Header("설정")]
    public float speedMultiplier = 1f; // 속도 배율 (배경은 0.5, 바닥은 1 등으로 설정)
    
    private float targetWidth;     // 이미지의 가로 길이

    void Start()
    {
        // 1. 현재 오브젝트의 SpriteRenderer 컴포넌트를 가져옵니다.
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        
        // 2. 이미지의 가로 길이(width)를 자동으로 계산합니다.
        // size.x는 스프라이트의 실제 가로 길이입니다.
        targetWidth = sr.bounds.size.x;
    }

    void Update()
    {
        // LevelManager의 isGameActive가 false이면 움직이지 않음
        if (!LevelManager.Instance.isGameActive) return;

        // 3. 왼쪽 방향으로 계속 이동시킵니다.
        // Vector3.left = (-1, 0, 0)
        transform.Translate(Vector3.left * LevelManager.Instance.currentSpeed * speedMultiplier * Time.deltaTime);

        // 4. 이미지가 화면 왼쪽 끝을 완전히 벗어났는지 확인합니다.
        // 현재 x 위치 < -가로길이 (즉, 시작점보다 이미지 길이만큼 왼쪽으로 갔다면)
        if (transform.position.x < -targetWidth)
        {
            Reposition();
        }
    }

    // 5. 이미지를 다시 오른쪽 끝으로 이동시키는 함수
    void Reposition()
    {
        // 현재 위치에서 (가로 길이 * 2)만큼 오른쪽으로 이동
        // *2를 하는 이유: 이미지가 2장(현재 이미지 + 뒤따라오는 이미지)이 있다고 가정하기 때문
        Vector3 newPos = transform.position;
        newPos.x += targetWidth * 2f;
        transform.position = newPos;
    }
}