using UnityEngine;

/// <summary>
/// 카메라의 화면 비율을 고정시켜주는 스크립트입니다.
/// 어떤 해상도에서도 동일한 화면 비율을 유지하도록 레터박스/필러박스를 추가합니다.
/// </summary>
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode] // 에디터 모드에서도 스크립트가 실행되도록 합니다.
public class CameraAspectRatioController : MonoBehaviour
{
    [Tooltip("목표 화면 비율 (너비 / 높이)")]
    public float targetAspectRatio = 16.0f / 9.0f;

    private Camera mainCamera;
    private int lastScreenWidth = 0;
    private int lastScreenHeight = 0;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        AdjustAspectRatio();
    }

    void Update()
    {
        // 에디터 또는 실행 중에 화면 크기가 변경되었는지 확인합니다.
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            AdjustAspectRatio();
        }

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    void AdjustAspectRatio()
    {
        float windowAspectRatio = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspectRatio / targetAspectRatio;

        Rect rect = mainCamera.rect;

        if (scaleHeight < 1.0f) // 현재 화면이 목표보다 세로로 길 경우 (Pillarbox)
        {
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
        }
        else // 현재 화면이 목표보다 가로로 길 경우 (Letterbox)
        {
            float scaleWidth = 1.0f / scaleHeight;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
        }
        mainCamera.rect = rect;
    }
}