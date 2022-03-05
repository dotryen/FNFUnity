using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

namespace FNF.Core {
    using UI;

    public class GameController : MonoBehaviour {
        public static GameController Instance { get; private set; }

        public bool startOnAwake = true;
        public bool returnToMenuOnFinish = true;
        public bool started;
        public bool paused;

        [Header("Modes")]
        public Gameplay gameplay;
        public Menu pauseMenu;
        public PlayableDirector timeline;

        [Header("Pause stuff")]
        public CanvasGroup pauseGroup;
        public AudioClip pauseMusic;
        public AudioSource musicSource;
        [Space]
        public TextMeshProUGUI songText;
        public TextMeshProUGUI diffucultyText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI accuracyText;
        public TextMeshProUGUI blueBallText;

        private SongAssets assets;
        private Dictionary<string, InputAction> actionMap;

        public SongAssets SongAssets => assets;

        private void Awake() {
            if (Instance != null) {
                Destroy(this);
                Debug.LogWarning("GameController already exists!");
                return;
            }

            Instance = this;

            InputManager.EnableMap("Gameplay");
            actionMap = InputManager.GetMapActions("Gameplay");

            // prepare music
            assets = SongAssets.GetSong(GameplayVars.Song);
            Conductor.SetBPM(assets.Metadata.bpm);
            gameplay.songAssets = assets;

            musicSource.outputAudioMixerGroup = FlixelVolume.Instance.mixer.FindMatchingGroups("Music")[0];
            pauseMenu.source.outputAudioMixerGroup = FlixelVolume.Instance.mixer.FindMatchingGroups("Sound")[0];

            // set text
            songText.text = assets.Metadata.songName;
            diffucultyText.text = assets.CurrentDifficulty.ToUpper();

            pauseMenu.Active = false;
        }

        private void Start() {
            if (startOnAwake) Invoke("StartGame", 0.5f);
        }

        private void Update() {
            if (!started) return;
            if (gameplay.dead) return;

            if (!paused) {
                if (actionMap["Pause"].WasPressedThisFrame()) {
                    InputManager.EnableMap("Menus");

                    gameplay.PauseSong();
                    Time.timeScale = 0f;

                    musicSource.time = Random.Range(0f, musicSource.clip.length / 2f);
                    musicSource.Play();

                    pauseGroup.alpha = 1;
                    pauseMenu.ActivateNextFrame();
                    paused = true;

                    scoreText.text = $"Score: {GameplayVars.Score}";
                    accuracyText.text = $"Accuracy: %{GameplayVars.Accuracy}";
                    blueBallText.text = $"Blue balled: {GameplayVars.Deaths}";
                }
            } else {
                if (musicSource.volume < 0.5f) {
                    musicSource.volume += 0.01f * Time.unscaledDeltaTime;
                }
            }
        }

        private void OnDestroy() {
            if (Instance != this) return;

            Instance = null;
            Time.timeScale = 1f;
        }

        public void StartGame() {
            started = true;
            paused = false;

            gameplay.StartSong();
            InvokeRepeating("UpdateDiscord", 1, 15);
        }

        public void OnGameEnd() {
            if (!returnToMenuOnFinish) return;
            ReturnToMenu(true);
        }

        public void Unpause() {
            if (!paused) return;
            Time.timeScale = 1f;

            musicSource.Stop();
            musicSource.volume = 0f;

            pauseGroup.alpha = 0;
            pauseMenu.Active = false;

            gameplay.ResumeSong();
            paused = false;
        }

        public void ReloadScene() {
            GameplayVars.Restart();
            assets.ReloadStage();
        }

        public void ReturnToMenu(bool save = false) {
            if (save) {
                if (Scores.GetScore(assets.FullName).score < GameplayVars.Score) {
                    Scores.ScoreData data = new Scores.ScoreData();
                    data.score = GameplayVars.Score;
                    data.maxCombo = GameplayVars.MaxCombo;
                    Ratings.GetCurrentRating(out data.rank, out data.achievements);

                    Scores.SetScore(assets.FullName, data);
                    Scores.Save();
                }
            }

            DiscordController.Instance.IdleActivity();
            InputManager.EnableMap("Menus");
            MainMenuRoutine().Run();
        }

        private void UpdateDiscord() {
            DiscordController.Instance.PlayingActivity();
        }

        private IEnumerator MainMenuRoutine() {
            yield return assets.UnloadStage();
            yield return SceneManager.LoadSceneAsync("MainMenu");
            yield return ScreenFader.ExitFade();
        }
    }
}
