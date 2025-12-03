using UnityEngine;

public class QuadScroller : MonoBehaviour
{
    [Header("스크롤 설정")]
    public float speedMultiplier = 0.1f; // 속도 배율 (LevelManager 속도에 곱해짐)

    [Header("화면 노출 설정")]
    [Tooltip("스프라이트와의 순서 정렬 (낮을수록 뒤, 높을수록 앞)")]
    public int sortingOrder = 0; // 바닥은 0, 배경은 -10 추천

    private Material myMaterial;

    void Start()
    {
        Renderer r = GetComponent<Renderer>();
        
        // 1. 마테리얼 가져오기
        myMaterial = r.material;

        // 2. [중요] Quad가 GameView에서 안 보이는 문제 자동 해결
        // 3D 오브젝트인 Quad에게 2D 순서(Sorting Order)를 강제로 부여합니다.
        r.sortingOrder = sortingOrder;
    }

    void Update()
    {
        // LevelManager의 isGameActive가 false이면 움직이지 않음
        if (!LevelManager.Instance.isGameActive) return;

        // 3. 오프셋 계산 (무한 스크롤 핵심)
        float scrollSpeed = LevelManager.Instance.currentSpeed * speedMultiplier;
        // Time.time * speed를 하면 시간이 지날수록 숫자가 너무 커져서 떨림 현상이 생길 수 있습니다.
        // Mathf.Repeat(값, 1)을 사용하여 0 ~ 1 사이에서만 깔끔하게 반복되도록 합니다.
        float xOffset = Mathf.Repeat(Time.time * scrollSpeed, 1);

        // 4. 마테리얼의 오프셋 적용 (X축으로만 이동)
        myMaterial.mainTextureOffset = new Vector2(xOffset, 0);
    }
}