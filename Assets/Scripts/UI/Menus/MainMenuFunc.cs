using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MainMenuFunc : MenuFunctions {
    public RectTransform loadingText;
    public AudioClip music;

    private bool controlling;
    private string descriptionCache;

    private void Awake() {
        GameplayVars.Reset();

        if (!PersistentMusic.Playing) PersistentMusic.Play(music);
    }

    private void Update() {
        if (!controlling) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) SettingsController.Left();
        if (Input.GetKeyDown(KeyCode.RightArrow)) SettingsController.Right();

        InvokingOption.description = InvokingOption.defaultText + ": " + SettingsController.CurrentValue;

        if (Input.GetKeyDown(KeyCode.Return)) {
            controlling = false;
            InvokingOption.description = descriptionCache;
            InvokingMenu.ActivateNextFrame();
        }
    }

    [Preserve]
    public void StoryMode() {
        InvokingOption.description = "not yet lol!!!! trolllllllllllllllllllllllllllllllllllllllllllllld!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!";
    }

    [Preserve]
    public void LoadFree() {
        var cast = InvokingOption as LoadSongButton;
        if (cast == null) return;

        InvokingMenu.Active = false;
        InvokingMenu.TargetColor = new Color(0.2f, 0.2f, 0.2f, 1);

        InvokingOption.description = "Loading...";
        StartCoroutine(TweenMenu());
        StartCoroutine(LoadSong(cast.songName, cast.difficulty));
    }

    [Preserve]
    public void CloseGame() {
        Application.Quit();
    }

    [Preserve]
    public void ControlSetting() {
        var button = InvokingOption as SettingButton;
        if (button == null) return;

        if (SettingsController.Select(button.settingKey)) {
            InvokingMenu.Active = false;
            descriptionCache = button.description;
            controlling = true;
        }
    }

    IEnumerator TweenMenu() {
        var diff = loadingText.anchoredPosition - (Vector2)InvokingOption.transform.position;
        var menuTarget = new Vector2(InvokingMenu.transform.anchoredPosition.x - diff.x, InvokingMenu.transform.anchoredPosition.y);
        var textTarget = new Vector2(loadingText.anchoredPosition.x - diff.x, InvokingMenu.transform.anchoredPosition.y);

        while (loadingText.anchoredPosition != textTarget) {
            var lerp = Utils.MakeFrameIndependantInverse(0.16f);
            loadingText.anchoredPosition = Vector2.Lerp(textTarget, loadingText.anchoredPosition, lerp);
            InvokingMenu.transform.anchoredPosition = Vector2.Lerp(menuTarget, InvokingMenu.transform.anchoredPosition, lerp);
            yield return null;
        }
    }

    IEnumerator LoadSong(string name, string diff) {
        var song = SongAssets.GetSong(name);

        if (!song.MetadataLoaded) yield return LoadingTask("Metadata", song.LoadMeta().Value);
        if (diff != song.CurrentDifficulty) yield return LoadingTask("Chart", song.LoadChart(diff).Value);
        if (!song.AudioLoaded) yield return LoadingTask("Audio", song.LoadAudio());
        if (!song.StageReady) {
            InvokingOption.description = "Loading stage...";
            yield return song.LoadStage();
        }

        InvokingOption.description = "Showing stage...";
        GameplayVars.Song = name;
        PersistentMusic.Stop();
        song.ActivateStage();
    }

    IEnumerator LoadingTask(string name, params AsyncOperationHandle[] handles) {
        float average = 0;

        while (average != 1) {
            average = 0;

            foreach (var handle in handles) {
                average += handle.PercentComplete;
            }
            average /= handles.Length;

            InvokingOption.description = $"Loading {name}: {Mathf.Round(average * 100)}%";
            yield return null;
        }
    }
}
