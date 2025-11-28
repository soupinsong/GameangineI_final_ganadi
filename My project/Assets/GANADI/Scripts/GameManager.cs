using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager Instance;

    [Header("게임 설정값")]
    public bool IsMusicOn { get; set; } = true;
    public bool IsSoundOn { get; set; } = true;
    public bool IsMicOn { get; set; } = true;

    [Header("게임 데이터")]
    public int CollectedItemCount { get; set; } = 0;

    [Header("게임 오버 UI")]
    private GameObject gameOverPanel; // 게임 오버 시 활성화할 패널 (씬 로드 시 자동으로 찾음)

    void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            // 씬이 전환되어도 이 게임 오브젝트가 파괴되지 않도록 설정
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 이미 인스턴스가 존재하면 새로 생성된 것을 파괴
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        // 씬 로드 이벤트 구독
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 씬 로드 이벤트 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 씬이 로드될 때마다 호출되는 함수
    /// </summary>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 게임 씬일 경우에만 GameOverPanel을 찾습니다.
        // "MainUI", "BossScene" 등 게임오버 패널이 없는 씬의 이름을 추가하여 제외할 수 있습니다.
        if (scene.name != "MainUI" && scene.name != "BossScene")
        {            
            // UIManager의 인스턴스를 다시 찾습니다.
            if (UIManager.Instance != null)
            {
                // UIManager를 통해 GameOverPanel을 참조합니다.
                gameOverPanel = UIManager.Instance.gameOverPanel;
                if (gameOverPanel != null) gameOverPanel.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 플레이어가 EndPoint에 도달했을 때 호출됩니다.
    /// </summary>
    /// <param name="itemCount">플레이어가 수집한 아이템 개수</param>
    public void ReachEndPoint(int itemCount)
    {
        Debug.Log($"엔드포인트 도달! 수집한 아이템: {itemCount}개");
        CollectedItemCount = itemCount;
        // TODO: "BossScene"은 실제 보스 씬의 이름으로 변경해야 합니다.
        SceneManager.LoadScene("BossScene");
    }

    /// <summary>
    /// 플레이어의 체력이 0이 되었을 때 호출됩니다.
    /// </summary>
    public void PlayerDied()
    {
        Debug.Log("게임 오버!");
        // 게임 오버 효과음 재생
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.gameOverSfx);

        if (gameOverPanel != null)
        {
            // 게임 오버 패널 활성화
            gameOverPanel.SetActive(true);
        }

        // 게임 시간을 멈춤 (선택 사항)
        // Time.timeScale을 0으로 설정하면 UI 입력 처리가 멈출 수 있습니다.
        // EventSystem의 Update Mode를 Unscaled Time으로 설정하는 것이 더 권장되는 방식입니다.
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 게임을 일시정지하고 설정 UI를 엽니다. (설정 버튼에 연결)
    /// </summary>
    public void PauseGameForSettings()
    {
        // 게임 시간을 멈춤
        Time.timeScale = 0f;

        // UIManager가 존재하면 설정 패널을 활성화합니다.
        if (UIManager.Instance != null)
            UIManager.Instance.ToggleInGameSettingsPanel();
    }

    /// <summary>
    /// 게임을 재개하고 설정 UI를 닫습니다. (설정 패널의 X 버튼에 연결)
    /// </summary>
    public void ResumeGameFromSettings()
    {
        // 게임 시간을 다시 활성화
        Time.timeScale = 1f;

        // UIManager가 존재하면 설정 패널을 비활성화합니다.
        if (UIManager.Instance != null)
            UIManager.Instance.ToggleInGameSettingsPanel();
    }

    #region 게임 오버 UI 버튼 함수

    #endregion
}