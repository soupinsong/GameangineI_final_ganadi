using UnityEngine;
using UnityEngine.UI;

public class InGameUIConnector : MonoBehaviour
{
    [Header("연결할 UI 요소")]
    public Button settingsButton; // 인게임 설정 버튼
    public Button resumeButton;   // 인게임 설정 패널의 '닫기' 또는 '계속하기' 버튼

    void Start()
    {
        // 버튼이 할당되었는지, GameManager 인스턴스가 존재하는지 확인합니다.
        if (settingsButton != null && GameManager.Instance != null)
        {
            // 기존에 Inspector에서 설정된 리스너를 모두 제거합니다.
            settingsButton.onClick.RemoveAllListeners();
            
            // 스크립트에서 올바른 GameManager의 PauseGameForSettings 메서드를 리스너로 추가합니다.
            settingsButton.onClick.AddListener(GameManager.Instance.PauseGameForSettings);
        }

        // 재개 버튼이 할당되었는지, GameManager 인스턴스가 존재하는지 확인합니다.
        if (resumeButton != null && GameManager.Instance != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(GameManager.Instance.ResumeGameFromSettings);
        }
        else
        {
            if (settingsButton == null)
            {
                Debug.LogError("설정 버튼이 InGameUIConnector에 할당되지 않았습니다.");
            }
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager 인스턴스를 찾을 수 없습니다.");
            }
        }
    }
}
