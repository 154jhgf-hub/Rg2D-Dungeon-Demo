using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("音频源")]
    [SerializeField] private AudioSource musicSource;      // 音乐专用
    [SerializeField] private AudioSource sfxSource;        // 音效专用
    [SerializeField] private int sfxPoolSize = 5;          // 音效源池大小
    private List<AudioSource> sfxPool;                     // 音效源池

    [Header("音量设置")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("音频片段库")]
    public AudioClip[] bgmClips;          // 背景音乐
    public AudioClip[] footsteps;          // 脚步声
    public AudioClip[] attackSounds;       // 攻击音效
    public AudioClip[] hitSounds;          // 受击音效
    public AudioClip[] pickupSounds;       // 拾取音效
    public AudioClip[] enemyDeathSounds;   // 敌人死亡
    public AudioClip[] uiSounds;    // UI音效
    public AudioClip[] enemyRoar;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitAudioSyetem();
            LoadVolumeSettings();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitAudioSyetem()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
        sfxPool = new List<AudioSource>();
        for(int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            sfxPool.Add(audioSource);
        }
    }

    public void PlayBgm(AudioClip clip,float fadeinTime)
    {
        if (clip == null)
        {
            return;
        }
        if (fadeinTime > 0)
        {
            StartCoroutine(FadeInTime(clip, fadeinTime));
        }
        else
        {
            musicSource.clip=clip;
            musicSource.volume = masterVolume * musicVolume;
            musicSource.Play();
        }
    }

    public void StopBgm(AudioClip clip,float fadeOutTime)
    {
        if (fadeOutTime > 0)
        {
            StartCoroutine(FadeOutTime(clip, fadeOutTime));
        }
        else
        {
            musicSource.Stop();
        }
    }

    public void PlaySound(AudioClip clip,float volume=1,float pitch=1)
    {
        if (clip == null)
        {
            return;
        }
        AudioSource audioSource = GetAvailableSFXSource();
        if (audioSource != null)
        {
            audioSource.volume = sfxVolume * volume * masterVolume;
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(clip);
        }
    }

    public void Play3DSFX(AudioClip clip, Vector3 position, float volume = 1f, float minDistance = 1f, float maxDistance = 5f,float scale=1)
    {
        if (clip == null) return;

        Transform listener = Camera.main != null ? Camera.main.transform : null;
        if (listener != null)
        {
            float distance = Vector2.Distance(listener.position, position);
            if (distance > maxDistance)
            {
                return;
            }
        }

        // 创建临时3D音源
        GameObject tempSource = new GameObject("TempAudioSource");
        tempSource.transform.position = listener != null
            ? new Vector3(position.x, position.y, listener.position.z)
            : position;
        AudioSource source = tempSource.AddComponent<AudioSource>();

        source.clip = clip;
        source.volume = volume * sfxVolume * masterVolume;
        source.spatialBlend = 1f;  // 完全3D
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.PlayOneShot(clip,scale);

        // 自动销毁
        Destroy(tempSource, clip.length);
    }

    IEnumerator FadeInTime(AudioClip clip,float time)
    {
        float timer = 0f;
        musicSource.clip = clip;
        musicSource.volume = 0;
        musicSource.Play();
        while (timer < time)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, musicVolume * masterVolume, timer/time);
            yield return null;
        }
    }

    IEnumerator FadeOutTime(AudioClip clip, float time)
    {
        float timer = 0f;
        float startVolume = musicSource.volume;
        while (timer < time)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0, timer / time);
            yield return null;
        }
        musicSource.Stop();
    }

    private AudioSource GetAvailableSFXSource()
    {
        // 查找空闲的音效源
        foreach (var source in sfxPool)
        {
            if (!source.isPlaying)
                return source;
        }

        // 如果没有空闲的，返回基础音效源
        return sfxSource;
    }

    public void AppleVolume()
    {
        if (musicSource != null)
        {
            musicSource.volume = masterVolume * musicVolume;
        }
        if (sfxSource != null)
        {
            sfxSource.volume = masterVolume * sfxVolume;
        }
        foreach(AudioSource audioSource in sfxPool)
        {
            audioSource.volume = masterVolume * sfxVolume;
        }
    }

    private void SaveVolume()
    {
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.Save();
    }

    public void SetBgmVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        AppleVolume();
        SaveVolume();
    }
    public void SetSoundVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        AppleVolume();
        SaveVolume();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        AppleVolume();
        SaveVolume();
    }

    private void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("masterVolume", 0.8f);
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.6f);
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 0.8f);
        AppleVolume();
    }
}
