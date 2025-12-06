using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM")]
    public AudioSource bgmSource; // 배경음악을 재생할 AudioSource
    public AudioClip mainMenuBgm;     // 메인 메뉴 BGM
    public AudioClip runningGameBgm;  // 러닝 게임 BGM
    public AudioClip bossSceneBgm;    // 보스전 BGM



    [Header("SFX")]
    public AudioSource sfxSource; // 효과음을 재생할 AudioSource
    public AudioClip itemGetSfx;
    public AudioClip collisionSfx;
    public AudioClip gameOverSfx;
    public AudioClip buttonClickSfx;
    public AudioClip jumpSfx; // 점프 효과음 추가
    public AudioClip attackSfx; // 공격 효과음 추가


    void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 전환되어도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 오브젝트가 파괴될 때 호출됩니다.
    void OnDestroy()
    {
        // 현재 인스턴스가 싱글톤 인스턴스라면, 참조를 null로 초기화합니다.
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// 배경음악을 켜거나 끕니다.
    /// </summary>
    /// <param name="mute">true이면 음소거, false이면 음소거 해제</param>
    public void MuteMusic(bool mute)
    {
        if (bgmSource != null)
        {
            bgmSource.mute = mute;
        }
    }

    /// <summary>
    /// 효과음을 켜거나 끕니다.
    /// </summary>
    /// <param name="mute">true이면 음소거, false이면 음소거 해제</param>
    public void MuteSfx(bool mute)
    {
        if (sfxSource != null)
        {
            sfxSource.mute = mute;
        }
    }

    /// <summary>
    /// 지정된 효과음을 한 번 재생합니다.
    /// </summary>
    /// <param name="clip">재생할 오디오 클립</param>
    public void PlaySfx(AudioClip clip)
    {
        if (sfxSource != null && clip != null) {
            sfxSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// 모든 소리(배경음악, 효과음)를 켜거나 끕니다.
    /// </summary>
    /// <param name="mute">true이면 음소거, false이면 음소거 해제</param>
    public void MuteAll(bool mute)
    {
        MuteMusic(mute);
        MuteSfx(mute);
    }

    /// <summary>
    /// 지정된 배경음악을 재생합니다. 현재 재생 중인 BGM과 다를 경우에만 교체합니다.
    /// </summary>
    /// <param name="bgmClip">재생할 BGM 오디오 클립</param>
    public void PlayBgm(AudioClip bgmClip)
    {
        if (bgmSource == null || bgmClip == null) return;

        // 현재 재생 중인 BGM과 다를 경우에만 교체
        if (bgmSource.clip != bgmClip)
        {
            bgmSource.clip = bgmClip;
            bgmSource.Play();
        }
    }
}
