using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using FNF.Core;
using FNF.Sprites;
using FNF.UI;

public class TitleScreen : MonoBehaviour {
    public AudioClip music;
    public AudioSource enterSource;
    public UIText[] textStuff;
    public GameObject creditContainer;
    public Image flash;

    public SpriteAnimation logo;
    public SpriteAnimation gf;
    public SpriteAnimation text;

    public TextAsset randomText;

    private int index = 0;
    private bool inIntro;
    private bool pressed;
    private InputAction action;

    private void Awake() {
        action = InputManager.GetAction("Menus", "Submit");

        Cursor.visible = false;
        Conductor.SetBPM(102);

        PersistentMusic.FadeIn(4, 0, 0.7f);
        PersistentMusic.Play(music);

        StartCoroutine(DoIntroShit());

        InvokeRepeating(nameof(BumpLogo), 0, (float)Conductor.SecondsPerBeat);
        gf.speed = ((1f / 24f) * 15f) / (float)Conductor.SecondsPerBeat;
    }

    private void Update() {
        if (inIntro) {
            if (action.WasPressedThisFrame()) {
                ShowTitle();
            }
            return;
        }

        if (action.WasPressedThisFrame()) {
            if (inIntro) {
                ShowTitle();
                PersistentMusic.Stop();
                PersistentMusic.Play(music, ((float)Conductor.Crochet * 1000f) * 16f);
                inIntro = false;
            } else {
                if (pressed) return;

                enterSource.Play();
                text.SetAnimation("Pressed");
                FlashScreen(1);
                Invoke("LoadMainMenu", 2);

                pressed = true;
            }
        }
    }

    IEnumerator DoIntroShit() {
        inIntro = true;

        float time = 0;
        int lastBeat = 0;
        var options = System.StringSplitOptions.RemoveEmptyEntries;
        string[] curWacky = randomText.text.Split(new string[] { "\n" }, options);
        string[] parts;

        parts = curWacky[Mathf.RoundToInt(Random.Range(0, curWacky.Length - 1))].Split(new string[] { "--" }, options);

        while (inIntro) {
            GameplayVars.CurrentTime = time;

            if (GameplayVars.CurrentBeat != lastBeat) {
                switch (GameplayVars.CurrentBeat) {
                    case 1:
                        SetText("ninjamuffin99", "phantomArcade", "kawaisprite", "evilsk8er");
                        break;
                    case 3:
                        AddText("present");
                        break;
                    case 4:
                        HideText();
                        break;
                    case 5:
                        SetText("In association", "with");
                        break;
                    case 7:
                        AddText("newgrounds");
                        break;
                    case 8:
                        HideText();
                        break;
                    case 9:
                        SetText(parts[0]);
                        break;
                    case 11:
                        AddText(parts[1]);
                        break;
                    case 12:
                        HideText();
                        break;
                    case 13:
                        SetText("Friday");
                        break;
                    case 14:
                        AddText("Night");
                        break;
                    case 15:
                        AddText("Funkin");
                        break;
                    case 16:
                        ShowTitle();
                        break;
                }
                lastBeat = GameplayVars.CurrentBeat;
            }

            time += Time.deltaTime;
            yield return null;
        }
    }

    private void SetText(params string[] text) {
        HideText();

        index = 0;
        AddText(text);
    }

    private void AddText(params string[] text) {
        for (int i = 0; i < text.Length; i++) {
            var obj = textStuff[index + i];
            obj.CreateText(text[i]);

            var halfHeight = obj.transform.sizeDelta.y / 2;
            var pos = new Vector2(0, halfHeight - 360);

            pos.y -= ((i + index) * 60) - 200f;

            obj.transform.anchoredPosition = pos;
            obj.gameObject.SetActive(true);
        }
        index += text.Length;
    }

    private void HideText() {
        foreach (var thing in textStuff) {
            thing.gameObject.SetActive(false);
        }
    }

    private void ShowTitle() {
        creditContainer.SetActive(false);
        FlashScreen(4);
        inIntro = false;
    }

    private void FlashScreen(float duration) {
        StopAllCoroutines(); // only coroutine running
        StartCoroutine(FlashScreenCo(duration));
    }

    private IEnumerator FlashScreenCo(float duration) {
        var color = Color.white;
        float time = 0;

        while (time < duration) {
            color.a = 1 - (time / duration);
            time += Time.deltaTime;

            flash.color = color;
            yield return null;
        }

        flash.color = Color.clear;
    }

    private void BumpLogo() {
        logo.SetAnimation("Bump", false);
    }

    private void LoadMainMenu() {
        ScreenFader.FadeAndLoad("MainMenu");
    }
}
