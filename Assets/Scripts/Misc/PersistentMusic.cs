using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentMusic : MonoBehaviour {
    private static PersistentMusic instance;

    public AudioSource source;

    public static bool Playing => instance.source.isPlaying;

    private void Awake() {
        instance = this;
    }

    public static void Play(AudioClip clip, float seconds = 0) {
        instance.source.clip = clip;
        instance.source.time = seconds;
        instance.source.Play();
    }

    public static void Resume() {
        instance.source.UnPause();
    }

    public static void Pause() {
        instance.source.Pause();
    }

    public static void Stop() {
        instance.source.Stop();
    }

    public static void FadeIn(float duration, float begin, float end) {
        instance.StartCoroutine(instance.FadeInCo(duration, begin, end));
    }

    private IEnumerator FadeInCo(float duration, float begin, float end) {
        float time = 0f;

        while (time <= duration) {
            source.volume = Mathf.Lerp(begin, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        source.volume = end;
    }
}
