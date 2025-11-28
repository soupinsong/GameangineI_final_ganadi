using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using static GameManager;

public class MainUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject settingsPanel; // 환경설정 패널
    public GameObject tutorialPanel; // 튜토리얼 패널

    [Header("Tutorial Pages")]
    public List<GameObject> tutorialPages; // 튜토리얼 이미지들을 담을 리스트
    private int currentPageIndex = 0;

    [Header("Settings Buttons & Sprites")]
    public Image micButtonImage;
    public Image soundButtonImage;
    public Image musicButtonImage;
    public Sprite micOnSprite;
    public Sprite micOffSprite;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

    void Start()
    {
        // 게임 시작 시 모든 패널을 비활성화합니다.
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        // 초기 설정값에 따라 버튼 이미지 업데이트 (실제 구현 시 PlayerPrefs 등으로 초기값 로드)
        UpdateSettingsButtonImages();

        // GameManager가 존재하고, 이 씬이 처음 로드될 때 설정값을 GameManager에 동기화
        if (GameManager.Instance != null)
        {
            GameManager.Instance.IsMusicOn = isMusicOn;
            GameManager.Instance.IsSoundOn = isSoundOn;
            GameManager.Instance.IsMicOn = isMicOn;
        }
    }

    #region 스테이지 로드 함수
    /// <summary>
    /// 1 스테이지 씬을 로드합니다. 씬 이름은 실제 씬 이름과 일치해야 합니다.
    /// </summary>
    public void LoadStage1()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.buttonClickSfx);
        // 멈춰있을 수 있는 게임 시간을 다시 활성화합니다.
        Time.timeScale = 1f;
        SceneManager.LoadScene("Choco_Run");
         // 재시도 시 아이템 개수를 초기화합니다.
        PlayerController.ResetItemCount();

    }

    /// <summary>
    /// 2 스테이지 씬을 로드합니다.
    /// </summary>
    public void LoadStage2()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.buttonClickSfx);
        // 멈춰있을 수 있는 게임 시간을 다시 활성화합니다.
        Time.timeScale = 1f;
        SceneManager.LoadScene("StrawBerry_Run");
         // 재시도 시 아이템 개수를 초기화합니다.
        PlayerController.ResetItemCount();

    }

    /// <summary>
    /// 3 스테이지 씬을 로드합니다.
    /// </summary>
    public void LoadStage3()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.buttonClickSfx);
        // 멈춰있을 수 있는 게임 시간을 다시 활성화합니다.
        Time.timeScale = 1f;
        SceneManager.LoadScene("BANANA_Run");
         // 재시도 시 아이템 개수를 초기화합니다.
        PlayerController.ResetItemCount();

    }

    /// <summary>
    /// 메인 메뉴(MainUI) 씬으로 전환합니다.
    /// </summary>
    public void LoadMainUIScene()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.buttonClickSfx);
        SceneManager.LoadScene("MainUI");
         // 재시도 시 아이템 개수를 초기화합니다.
        PlayerController.ResetItemCount();

    }
    #endregion

    #region UI 패널 토글 함수
    /// <summary>
    /// 환경설정 패널을 켜거나 끕니다.
    /// </summary>
    public void ToggleSettingsPanel()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.buttonClickSfx);
        if (settingsPanel != null)
        {
            bool isActive = !settingsPanel.activeSelf;
            settingsPanel.SetActive(isActive);
        }
    }

    /// <summary>
    /// 튜토리얼 패널을 켜거나 끕니다.
    /// </summary>
    public void ToggleTutorialPanel()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.buttonClickSfx);
        if (tutorialPanel != null)
        {
            bool isActive = !tutorialPanel.activeSelf;
            tutorialPanel.SetActive(isActive);

            // 튜토리얼 패널이 활성화되면 항상 첫 페이지를 보여줍니다.
            if (isActive)
            {
                ShowTutorialPage(0);
            }
        }
    }
    #endregion

    #region 튜토리얼 페이지 넘기기 함수
    private void ShowTutorialPage(int index)
    {
        if (tutorialPages == null || tutorialPages.Count == 0) return;

        // 인덱스가 유효한 범위 내에 있도록 조정
        currentPageIndex = Mathf.Clamp(index, 0, tutorialPages.Count - 1);

        for (int i = 0; i < tutorialPages.Count; i++)
        {
            tutorialPages[i].SetActive(i == currentPageIndex);
        }
    }

    public void NextTutorialPage()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.buttonClickSfx);
        // 마지막 페이지면 처음으로 돌아갑니다 (원하는 동작에 따라 수정 가능)
        int nextPageIndex = (currentPageIndex + 1) % tutorialPages.Count;
        ShowTutorialPage(nextPageIndex);
    }

    public void PreviousTutorialPage()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.buttonClickSfx);
        // 첫 페이지면 마지막으로 돌아갑니다 (원하는 동작에 따라 수정 가능)
        int prevPageIndex = (currentPageIndex - 1 + tutorialPages.Count) % tutorialPages.Count;
        ShowTutorialPage(prevPageIndex);
    }
    #endregion

    #region 환경설정 토글 함수
    // 각 설정의 On/Off 상태를 저장할 변수들
    private bool isMicOn = true; // TODO: 실제 초기값은 저장된 설정(PlayerPrefs 등)에서 불러와야 합니다.
    private bool isSoundOn = true;
    private bool isMusicOn = true;

    public void SetMicOn()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.buttonClickSfx);
        isMicOn = true;
        if (GameManager.Instance != null) GameManager.Instance.IsMicOn = true;
        Debug.Log("마이크 상태: " + (isMicOn ? "On" : "Off"));
        UpdateSettingsButtonImages();
    }
    public void SetMicOff()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.buttonClickSfx);
        isMicOn = false;
        if (GameManager.Instance != null) GameManager.Instance.IsMicOn = false;
        Debug.Log("마이크 상태: " + (isMicOn ? "On" : "Off"));
        UpdateSettingsButtonImages();
    }

    public void SetSoundOn()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.buttonClickSfx);
        isSoundOn = true;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.IsSoundOn = true;
            GameManager.Instance.IsMusicOn = true; // GameManager의 음악 상태도 동기화
        }
        if (AudioManager.Instance != null) AudioManager.Instance.MuteAll(false); // 모든 소리 켜기
        Debug.Log("사운드 상태: " + (isSoundOn ? "On" : "Off"));
        UpdateSettingsButtonImages();
    }
    public void SetSoundOff()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.buttonClickSfx);
        isSoundOn = false;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.IsSoundOn = false;
            GameManager.Instance.IsMusicOn = false; // GameManager의 음악 상태도 동기화
        }
        if (AudioManager.Instance != null) AudioManager.Instance.MuteAll(true); // 모든 소리 끄기
        Debug.Log("사운드 상태: " + (isSoundOn ? "On" : "Off"));
        UpdateSettingsButtonImages();
    }

    public void SetMusicOn()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.buttonClickSfx);
        isMusicOn = true;
        if (GameManager.Instance != null) GameManager.Instance.IsMusicOn = isMusicOn;
        if (AudioManager.Instance != null) AudioManager.Instance.MuteMusic(!isMusicOn);
        Debug.Log("뮤직 상태: " + (isMusicOn ? "On" : "Off"));
        UpdateSettingsButtonImages();
    }
    public void SetMusicOff()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySfx(AudioManager.Instance.buttonClickSfx);
        isMusicOn = false;
        if (GameManager.Instance != null) GameManager.Instance.IsMusicOn = isMusicOn;
        if (AudioManager.Instance != null) AudioManager.Instance.MuteMusic(!isMusicOn);
        Debug.Log("뮤직 상태: " + (isMusicOn ? "On" : "Off"));
        UpdateSettingsButtonImages();
    }

    private void UpdateSettingsButtonImages()
    {
        if (micButtonImage != null) micButtonImage.sprite = isMicOn ? micOnSprite : micOffSprite;
        // 사운드 버튼은 음악과 효과음 상태를 모두 반영하도록 수정
        bool areAllSoundsOn = isMusicOn && isSoundOn;
        if (soundButtonImage != null) soundButtonImage.sprite = areAllSoundsOn ? soundOnSprite : soundOffSprite;
        if (musicButtonImage != null) musicButtonImage.sprite = areAllSoundsOn ? musicOnSprite : musicOffSprite;
    }
    #endregion
}