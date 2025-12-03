using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM")]
    public AudioSource bgmSource; // 배경음악을 재생할 AudioSource

    [Header("SFX")]
    public AudioSource sfxSource; // 효과음을 재생할 AudioSource
    public AudioClip itemGetSfx;
    public AudioClip collisionSfx;
    public AudioClip gameOverSfx;
    public AudioClip buttonClickSfx;


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
}
