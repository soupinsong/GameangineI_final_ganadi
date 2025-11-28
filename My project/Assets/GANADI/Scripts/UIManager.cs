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

    [Header("게임 오버 UI")]
    public GameObject gameOverPanel; // 게임 오버 시 활성화할 패널

    [Header("인게임 설정 UI")]
    public GameObject inGameSettingsPanel; // 게임 중 설정 패널

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // 게임 시작 시 UI 패널들은 항상 비활성화 상태로 시작
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (inGameSettingsPanel != null)
            inGameSettingsPanel.SetActive(false);
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
    /// 인게임 설정 패널의 활성화 상태를 토글합니다.
    /// </summary>
    public void ToggleInGameSettingsPanel()
    {
        if (inGameSettingsPanel != null)
        {
            inGameSettingsPanel.SetActive(!inGameSettingsPanel.activeSelf);
        }
    }
}