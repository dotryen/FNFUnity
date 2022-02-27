using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour {
    private static ScreenFader instance;
    public static bool Fading { get; private set; }

    public Image fade;

    private float moveAmount;

    private void Awake() {
        instance = this;
        moveAmount = fade.rectTransform.anchoredPosition.y;
    }

    public static void FadeAndLoad(string scene, float fadeIn = 1f, float fadeOut = 0.7f) {
        if (Fading) return;
        instance.StartCoroutine(instance.SceneLoad(scene, fadeIn, fadeOut));
        Fading = true;
    }

    public static void FadeAndLoad(SceneInstance scene, float fadeIn = 1f, float fadeOut = 0.7f) {
        if (Fading) return;
        instance.StartCoroutine(instance.SceneLoadInstance(scene, fadeIn, fadeOut));
        Fading = true;
    }

    public static Coroutine EnterFade(float time = 1f, Action onFinish = null) {
        if (Fading) return null;
        var routine = instance.StartCoroutine(instance.TweenIn(time, onFinish));
        Fading = true;

        return routine;
    }

    public static Coroutine ExitFade(float time = 0.7f, Action onFinish = null) {
        if (Fading) return null;
        var routine = instance.StartCoroutine(instance.TweenOut(time, onFinish));
        Fading = true;

        return routine;
    }

    private IEnumerator SceneLoad(string scene, float fadeIn, float fadeOut) {
        yield return TweenIn(fadeIn, null);
        SceneManager.LoadScene(scene);

        Fading = true;
        yield return TweenOut(fadeOut, null);
    }

    private IEnumerator SceneLoadInstance(SceneInstance instance, float fadeIn, float fadeOut) {
        yield return TweenIn(fadeIn, null);
        yield return instance.ActivateAsync();

        Fading = true;
        yield return TweenOut(fadeOut, null);
    }

    private IEnumerator TweenIn(float time, Action onFinish) {
        fade.rectTransform.anchoredPosition = new Vector2(0, moveAmount);
        var delta = moveAmount / time;
        
        while (fade.rectTransform.anchoredPosition.y != 0) {
            var pos = fade.rectTransform.anchoredPosition;
            pos.y -= delta * Time.unscaledDeltaTime;
            pos.y = Mathf.Clamp(pos.y, 0, moveAmount);

            fade.rectTransform.anchoredPosition = pos;
            yield return null;
        }

        Fading = false;
        onFinish?.Invoke();
    }

    private IEnumerator TweenOut(float time, Action onFinish) {
        fade.rectTransform.anchoredPosition = new Vector2(0, 0);
        var delta = moveAmount / time;

        while (fade.rectTransform.anchoredPosition.y != -moveAmount) {
            var pos = fade.rectTransform.anchoredPosition;
            pos.y -= delta * Time.unscaledDeltaTime;
            pos.y = Mathf.Clamp(pos.y, -moveAmount, 0);

            fade.rectTransform.anchoredPosition = pos;
            yield return null;
        }

        Fading = false;
        onFinish?.Invoke();
    }
}
