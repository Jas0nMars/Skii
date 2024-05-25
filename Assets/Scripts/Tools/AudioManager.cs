using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource musicSource;
    public AudioSource effectSource;

    private AudioClip bgmClip;

    void Awake()
    {
        // 确保音频管理器在场景加载时不会被销毁
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(float volume = 1.0f)
    {
        if (bgmClip == null) bgmClip = Resources.Load<AudioClip>("audio/music");
        musicSource.clip = bgmClip;
        musicSource.volume = volume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayEffect(AudioClip clip, float volume = 1.0f)
    {
        effectSource.PlayOneShot(clip, volume);
    }

    public void PlayEffect(string name, float volume = 1.0f)
    {
        AudioClip clip = Resources.Load<AudioClip>("audio/" + name);
        effectSource.PlayOneShot(clip, volume);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void StopEffects()
    {
        effectSource.Stop();
    }
}