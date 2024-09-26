using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip introMusic;         // 介绍背景音乐
    public AudioClip ghostNormalMusic;   // 正常状态的鬼魂背景音乐
    private AudioSource audioSource;     // 音频源

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // 播放介绍背景音乐
        PlayIntroMusic();
    }

    void PlayIntroMusic()
    {
        audioSource.clip = introMusic;
        audioSource.loop = false;  // 介绍音乐不需要循环播放
        audioSource.Play();
        StartCoroutine(WaitForIntroToFinish());
    }

    IEnumerator WaitForIntroToFinish()
    {
        // 等待介绍背景音乐播放完毕
        yield return new WaitForSeconds(audioSource.clip.length);
        
        // 播放正常状态的鬼魂背景音乐
        PlayGhostNormalMusic();
    }

    void PlayGhostNormalMusic()
    {
        audioSource.clip = ghostNormalMusic;
        audioSource.loop = true;  // 正常状态的鬼魂音乐需要循环播放
        audioSource.Play();
    }
}
