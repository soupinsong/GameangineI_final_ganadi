using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    // 어디서든 접근할 수 있게 싱글톤으로 만듭니다.
    public static UIManager Instance;

    [Header("체력 UI")]
    public List<Image> healthHearts; // 체력 하트 이미지 리스트
    public Sprite fullHeartSprite;   // 꽉 찬 하트 스프라이트
    public Sprite emptyHeartSprite;  // 빈 하트 스프라이트

    [Header("아이템 UI")]
    public TextMeshProUGUI itemCountText; // 아이템 개수 표시 텍스트

    [Header("보스 UI")]
    public GameObject bossHpBar; // 보스 체력 바 전체 GameObject
    public Image bossHpBarFill;  // 보스 체력 바의 채워지는 이미지
    public GameObject clearPanel; // 클리어 시 활성화할 패널

    [Header("게임 오버 UI")]
    public GameObject gameOverPanel; // 게임 오버 시 활성화할 패널

    [Header("인게임 설정 UI")]
    public GameObject inGameSettingsPanel; // 게임 중 설정 패널




    void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // 이미 인스턴스가 존재하면, 기존 인스턴스를 파괴하고 새로운 인스턴스로 교체합니다.
            Destroy(Instance.gameObject);
            Instance = this;
        }
        DontDestroyOnLoad(gameObject); // 씬이 전환되어도 이 게임 오브젝트가 파괴되지 않도록 설정
    }

    void OnDestroy()
    {
        // 현재 인스턴스가 싱글톤 인스턴스라면, 참조를 null로 초기화합니다.
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        // 게임 시작 시 UI 패널들은 항상 비활성화 상태로 시작
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (inGameSettingsPanel != null)
            inGameSettingsPanel.SetActive(false);

        // 보스 관련 UI는 시작 시 비활성화
        if (bossHpBar != null)
            bossHpBar.SetActive(false);

        if (clearPanel != null)
            clearPanel.SetActive(false);


    }

    /// <summary>
    /// 체력 UI를 현재 체력에 맞게 업데이트합니다.
    /// </summary>
    /// <param name="currentHealth">플레이어의 현재 체력</param>
    public void UpdateHealthUI(int currentHealth)
    {
        // 모든 하트를 순회하며
        for (int i = 0; i < healthHearts.Count; i++)
        {
            // 현재 체력보다 인덱스가 작으면 꽉 찬 하트 스프라이트로, 그렇지 않으면 빈 하트 스프라이트로 변경합니다.
            if (i < currentHealth)
            {
                healthHearts[i].sprite = fullHeartSprite;
            }
            else
            {
                healthHearts[i].sprite = emptyHeartSprite;
            }
        }
    }

    /// <summary>
    /// 아이템 개수 UI를 업데이트합니다.
    /// </summary>
    /// <param name="count">현재 아이템 개수</param>
    public void UpdateItemCountUI(int count)
    {
        if (itemCountText == null) return;

        itemCountText.text = "x " + count;
    }

    /// <summary>
    /// 보스의 체력 UI를 업데이트합니다.
    /// </summary>
    /// <param name="currentHealth">보스의 현재 체력</param>
    /// <param name="maxHealth">보스의 최대 체력</param>
    public void UpdateBossHealthUI(int currentHealth, int maxHealth)
    {
        if (bossHpBar == null || bossHpBarFill == null) return;

        // 보스 체력바가 비활성화 상태이면 활성화
        if (!bossHpBar.activeSelf)
        {
            bossHpBar.SetActive(true);
        }

        if (maxHealth > 0)
        {
            bossHpBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    /// <summary>
    /// 게임 클리어 패널을 활성화합니다.
    /// </summary>
    public void ShowClearPanel()
    {
        if (clearPanel != null)
        {
            clearPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 게임 오버 패널을 활성화합니다.
    /// </summary>
    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }


    /// <summary>
    /// 인게임 설정 패널의 활성화 상태를 토글합니다.
    /// </summary>
    public void ToggleInGameSettingsPanel()
    {
        if (inGameSettingsPanel != null)
        {
            inGameSettingsPanel.SetActive(!inGameSettingsPanel.activeSelf);
        }
    }

    /// <summary>
    /// 재시도 버튼 클릭 시 호출됩니다.
    /// </summary>
    public void OnRetryButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RetryBossBattle();
        }
    }
}