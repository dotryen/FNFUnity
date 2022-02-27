using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;

public class Loader : MonoBehaviour {
    public TextMeshProUGUI text;
    public Image thing;
    public VideoPlayer player;
    public float breathingSpeed = 2f;

    private bool ready;
    private bool played;

    private void Awake() {
        StartCoroutine(Load());
    }

    private void Update() {
        var time = Time.time * breathingSpeed;
        var normalizedSin = (1 + Mathf.Sin(time + 1)) / 2;
        thing.color = new Color(1, 1, 1, normalizedSin);

        if (ready && Input.GetKeyDown(KeyCode.Return)) {
            SceneManager.LoadSceneAsync("TitleScreen");
            // ScreenFader.FadeAndLoad("MainMenu", 0);
            Destroy(gameObject);
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && !played) {
            StartCoroutine(PlayVideo());
        }
    }

    IEnumerator PlayVideo() {
        played = true;
        player.Play();
        yield return new WaitUntil(() => player.frame > 0 && !player.isPlaying);
        Application.Quit();
    }

    IEnumerator Load() {
        yield return Addressables.InitializeAsync();
        text.text = "Ready... Press ENTER";
        ready = true;
    }
}
