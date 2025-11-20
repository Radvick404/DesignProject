using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    public AudioMixer mixer;

    [Header("Mixer Exposed Params")]
    public string musicParam = "MusicVolume";
    public string sfxParam = "SFXVolume";
    public string voiceParam = "VoiceVolume";
    public string ambienceParam = "AmbienceVolume";

    [Header("Audio Sources")]
    public AudioSource bgMusicSource;
    public AudioSource sfxSource;
    public AudioSource voiceSource;
    public AudioSource ambienceSource;

    [Header("Ambience Settings")]
    public List<AudioClip> ambienceClips;
    public float minAmbienceDelay = 3f;
    public float maxAmbienceDelay = 6f;

    private Coroutine ambienceRoutine;
    private Coroutine musicFadeRoutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // -------------------------------------------------------
    // MIXER VOLUME CONTROL
    // -------------------------------------------------------
    public void SetMusicVolume(float volume01)
    {
        mixer.SetFloat(musicParam, Mathf.Log10(volume01) * 20f);
    }

    public void SetSFXVolume(float volume01)
    {
        mixer.SetFloat(sfxParam, Mathf.Log10(volume01) * 20f);
    }

    public void SetVoiceVolume(float volume01)
    {
        mixer.SetFloat(voiceParam, Mathf.Log10(volume01) * 20f);
    }

    public void SetAmbienceVolume(float volume01)
    {
        mixer.SetFloat(ambienceParam, Mathf.Log10(volume01) * 20f);
    }

    // -------------------------------------------------------
    // CROSSFADE MUSIC
    // -------------------------------------------------------
    public void PlayBGMusic(AudioClip clip, float fadeDuration = 1f)
    {
        if (musicFadeRoutine != null)
            StopCoroutine(musicFadeRoutine);

        musicFadeRoutine = StartCoroutine(CrossfadeRoutine(clip, fadeDuration));
    }

    private IEnumerator CrossfadeRoutine(AudioClip newClip, float duration)
    {
        float startVol;
        mixer.GetFloat(musicParam, out startVol);
        startVol = Mathf.Pow(10f, startVol / 20f);

        // Fade OUT
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            SetMusicVolume(Mathf.Lerp(startVol, 0f, t / duration));
            yield return null;
        }

        bgMusicSource.clip = newClip;
        bgMusicSource.Play();

        // Fade IN
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            SetMusicVolume(Mathf.Lerp(0f, startVol, t / duration));
            yield return null;
        }
    }

    // -------------------------------------------------------
    // SFX + VOICE
    // -------------------------------------------------------
    public void PlaySFX(AudioClip clip)
    {
        if (clip)
            sfxSource.PlayOneShot(clip);
    }

    public void PlayVoice(AudioClip clip)
    {
        if (clip)
            voiceSource.PlayOneShot(clip);
    }

    // -------------------------------------------------------
    // RANDOM AMBIENCE CYCLE
    // -------------------------------------------------------
    public void StartAmbienceCycle()
    {
        if (ambienceRoutine != null)
            StopCoroutine(ambienceRoutine);

        ambienceRoutine = StartCoroutine(AmbienceRoutine());
    }

    private IEnumerator AmbienceRoutine()
    {
        while (true)
        {
            if (ambienceClips.Count > 0)
            {
                AudioClip clip = ambienceClips[Random.Range(0, ambienceClips.Count)];
                ambienceSource.PlayOneShot(clip);
            }

            yield return new WaitForSeconds(Random.Range(minAmbienceDelay, maxAmbienceDelay));
        }
    }

    // -------------------------------------------------------
    // AREA-BASED AMBIENCE
    // -------------------------------------------------------
    public void PlayAmbienceLoop(AudioClip clip)
    {
        if (clip == null) return;

        ambienceSource.loop = true;
        ambienceSource.clip = clip;
        ambienceSource.Play();
    }

    public void StopAmbienceLoop()
    {
        ambienceSource.Stop();
        ambienceSource.loop = false;
    }
}
