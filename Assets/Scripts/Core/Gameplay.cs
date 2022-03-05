using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Random = UnityEngine.Random;

namespace FNF.Core {
    using Characters;

    // DO NOT CHANGE TO BEATBEHAVIOUR
    // THOSE ARE MEANT TO RUN AFTER GAMEPLAY AS SET IN PROJECT SETTINGS
    public class Gameplay : MonoBehaviour {
        [Header("Sound Shit")]
        public AudioClip[] missSounds;
        public AudioClip deathSound;
        public AudioClip deathMusic;
        public AudioClip deathConfirm;
        [Space]
        public AudioSource music;
        public AudioSource vocals;
        public AudioSource sounds;

        [Header("UI shit")]
        public RectTransform uiContainer;
        public RectTransform healthBar;
        public RectTransform strumLine;
        public TextMeshProUGUI scoreText;
        public Image countdownSprite;
        public Sprite[] countdownImages;
        public AudioClip[] countdownSounds;

        [Header("girlfrien")]
        public Girlfriend girlfriend;
        [Min(1)]
        public int gfSpeed = 1;

        [Header("Player shit")]
        public NoteDisplay playerDisplay;
        public BaseSinger playerAnimation;
        public RawImage playerHealthIcon;

        [Header("Opponent Shit")]
        public NoteDisplay opponentDisplay;
        public BaseSinger opponentAnimation;
        public RawImage opponentHealthIcon;

        [Space]
        [Min(1)]
        public int beatModulo = 4;
        public SongAssets songAssets;

        // private variable
        private float scrollSpeed;
        private float lookAheadTime;
        private bool camZooming;

        // timing
        private int currentBeat;
        private int currentStep;

        // player stuff
        public bool dead;
        public bool deadConfirmed; // fuck you chris
        public float health = 1;
        private float notesHit;
        private int totalPlayed;
        private DisplayQueue playerNoteQueue;
        private DisplayQueue playerSustainQueue;
        private InputAction[] playerControls;
        private Dictionary<string, InputAction> actionMap;
        private TrackSampler<Note>[] playerSamplers;

        private TrackSampler<Note>[] opponentSamplers;

        private double lastUpdateTime;
        private bool ended;

        public bool Started { get; private set; }
        public bool Playing { get; private set; }
        public double CurrentTime { get; private set; }
        public double DeltaTime { get; private set; }

        public double TimeInSeconds => CurrentTime * 1000f;
        public float ToBottom => 720 + strumLine.anchoredPosition.y;

        private float HudZoom {
            get {
                return uiContainer.localScale.x;
            }

            set {
                uiContainer.localScale = new Vector3(value, value);
            }
        }

        private void Awake() {
            actionMap = InputManager.GetMapActions("Gameplay");
            playerControls = new InputAction[] {
                actionMap["SingLeft"],
                actionMap["SingDown"],
                actionMap["SingUp"],
                actionMap["SingRight"]
            };

            scoreText.gameObject.SetActive(Settings.Gameplay.ShowScore);

            // set mixer groups
            music.outputAudioMixerGroup = FlixelVolume.Instance.mixer.FindMatchingGroups("Music")[0];
            vocals.outputAudioMixerGroup = FlixelVolume.Instance.mixer.FindMatchingGroups("Music")[0];
            sounds.outputAudioMixerGroup = FlixelVolume.Instance.mixer.FindMatchingGroups("Sounds")[0];

            lastUpdateTime = Time.timeAsDouble;
        }

        public void StartSong() {
            if (Started) return;
            dead = false;

            // set bpm
            Conductor.SetBPM(songAssets.Metadata.bpm);

            // create queues
            playerNoteQueue = new DisplayQueue();
            playerSustainQueue = new DisplayQueue();

            // create samplers
            playerSamplers = new TrackSampler<Note>[4];
            for (int i = 0; i < 4; i++) {
                playerSamplers[i] = new TrackSampler<Note>(songAssets.Chart.playerTracks[i]);
                playerSamplers[i].Time = lookAheadTime;
            }

            opponentSamplers = new TrackSampler<Note>[4];
            for (int i = 0; i < 4; i++) {
                opponentSamplers[i] = new TrackSampler<Note>(songAssets.Chart.opponentTracks[i]);
                opponentSamplers[i].Time = lookAheadTime;
            }

            // calculate scroll speed and look time
            scrollSpeed = 0.45f * (Mathf.Round(songAssets.Chart.speed * 100f) / 100f);
            lookAheadTime = ToBottom / scrollSpeed;
            GameplayVars.NoteSpeed = scrollSpeed;

            // more shit
            CurrentTime = Settings.Gameplay.Offset;
            CurrentTime -= Conductor.Crochet * 5;
            health = 1;

            music.clip = songAssets.Instrumentals;
            music.loop = false;
            vocals.clip = songAssets.PlayerVocals;
            vocals.loop = false;

            StartCoroutine(Countdown());

            Started = true;
            Playing = true;
        }

        public void StopSong() {
            Started = false;
            Playing = false;
            music.Pause();
            vocals.Pause();
        }

        public void PauseSong() {
            if (!Playing || !Started) return;
            Playing = false;
            music.Pause();
            vocals.Pause();
        }

        public void ResumeSong() {
            if (Playing || !Started) return;
            Playing = true;
            music.UnPause();
            vocals.UnPause();
        }

        public void Update() {
            if (dead) {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace)) {
                    GameController.Instance.ReturnToMenu();
                } else if (Input.GetKeyDown(KeyCode.Return) && !deadConfirmed) {
                    music.Stop();
                    StopAllCoroutines();

                    sounds.PlayOneShot(deathConfirm);
                    playerAnimation.DeathConfirm();
                    StartCoroutine(Revive(2.7f));
                    deadConfirmed = true;
                }
            }

            if (Playing) {
                if (music.clip.length * 1000f <= CurrentTime && !ended) {
                    GameController.Instance.OnGameEnd();
                    ended = true;
                }

                DeltaTime = (Time.timeAsDouble - lastUpdateTime) * 1000d;
                CurrentTime += DeltaTime;
                GameplayVars.CurrentTimeMS = CurrentTime;

                // stepping and shit
                {
                    var stepCache = currentStep;

                    currentStep = (int)Math.Floor(CurrentTime / Conductor.StepCrochet);
                    currentBeat = Mathf.FloorToInt(currentStep / 4f);

                    if (stepCache != currentStep && currentStep > 0) {
                        StepHit();
                    }
                }

                UpdatePlayer();
                UpdateOpponent();
                UpdateUI();
            }

            lastUpdateTime = Time.timeAsDouble;
        }

        #region Player stuff

        private void UpdatePlayer() {
            for (int i = 0; i < 4; i++) {
                // control stuff
                {
                    // regular notes
                    {
                        GetLatestNote(playerNoteQueue, i, out var note, out var id);

                        if (playerControls[i].WasPressedThisFrame()) {
                            if (note != null) {
                                var rating = Ratings.Judge(CurrentTime - note.time);

                                // adding shit
                                {
                                    int combo = 1;
                                    int score = 350;
                                    float health = 0.04f;
                                    float hit = 1f;

                                    switch (rating) {
                                        case NoteRating.Sick:
                                            GameplayVars.SickNotes++;
                                            break;
                                        case NoteRating.Good:
                                            GameplayVars.GoodNotes++;
                                            score = 200;
                                            health = 0;
                                            hit = 0.75f;
                                            break;
                                        case NoteRating.Bad:
                                            GameplayVars.BadNotes++;
                                            score = 0;
                                            health = -0.06f;
                                            hit = 0.5f;
                                            break;
                                        case NoteRating.Shit:
                                            GameplayVars.ShitNotes++;
                                            score = -300;
                                            health = -0.1f;
                                            hit = 0;
                                            combo = -GameplayVars.Combo;
                                            break;
                                    }

                                    GameplayVars.Combo += combo;
                                    GameplayVars.Score += score;
                                    this.health += health;
                                    notesHit += hit;
                                    SharedNoteHit(i);
                                }

                                girlfriend.ShowRating(rating);
                                playerDisplay.StopNote(id);
                                playerNoteQueue.Dequeue(i, out _, out _);
                            } else {
                                SharedMiss(i, false);
                            }
                        }
                    }

                    // sustain notes
                    {
                        GetLatestNote(playerSustainQueue, i, out var note, out var id);

                        if (note != null) {
                            if (playerControls[i].IsPressed()) {
                                notesHit++;
                                SharedNoteHit(i);

                                playerDisplay.StopNote(id);
                                playerSustainQueue.Dequeue(i, out _, out _);
                            }
                        }
                    }
                }

                // spawning notes
                {
                    var sampler = playerSamplers[i];
                    sampler.Update(DeltaTime);

                    foreach (var note in sampler.Sample()) {
                        var id = playerDisplay.StartNote((SideColor)i, note.type, (float)note.time);

                        if (!note.IsSustain) playerNoteQueue.Enqueue(i, note, id);
                        else playerSustainQueue.Enqueue(i, note, id);
                    }
                }
            }

            if (health > 2) health = 2f;
            if (health <= 0) PlayerDeath();
        }

        private NoteState GetState(Note note) {
            var min = note.time - (Conductor.SAFE_OFFSET * 0.5f);
            var max = note.time + Conductor.SAFE_OFFSET;

            if (CurrentTime < min) return NoteState.TooEarly;
            else if (CurrentTime >= min && CurrentTime <= max) return NoteState.Good;
            else return NoteState.TooLate;
        }

        private void GetLatestNote(DisplayQueue queue, int track, out Note note, out int id) {
            note = null;
            id = -1;

            while (true) {
                if (queue.Count(track) == 0) return;

                queue.Peek(track, out var tempNote, out var tempId);
                var state = GetState(tempNote);

                if (state == NoteState.TooEarly) return;
                if (state == NoteState.Good) {
                    note = tempNote;
                    id = tempId;
                    return;
                }
                if (state == NoteState.TooLate) {
                    queue.Dequeue(track, out _, out _);

                    playerDisplay.StopNoteDelayed(tempId, 0.1f);
                    SharedMiss(track, tempNote.IsSustain);

                    playerSustainQueue.Peek(track, out var susNote, out _);
                    if (susNote.time == tempNote.time + Conductor.StepCrochet) {
                        bool finished = false;

                        while (!finished) {
                            playerSustainQueue.Peek(track, out var latestSustain, out var latestId);

                            playerDisplay.StopNote(latestId);
                            playerSustainQueue.Dequeue(track, out _, out _);

                            UpdateAccuracy();

                            if (latestSustain.type == NoteType.SustainEnd) {
                                finished = true;
                            }
                        }
                    }
                }
            }
        }

        private void SharedNoteHit(int track) {
            // animations
            playerDisplay.staticArrows[track].PlayOneShot("Static" + Enum.GetName(typeof(SideDir), (SideDir)track) + "Confirm");
            playerAnimation.Sing((SideDir)track);

            // vocals
            vocals.volume = 1;

            UpdateAccuracy();
        }

        private void SharedMiss(int track, bool sustain) {
            playerAnimation.Miss((SideDir)track);
            if (GameplayVars.Combo > 5) girlfriend.Sad();

            GameplayVars.Combo = 0;
            GameplayVars.Score -= 10;
            GameplayVars.MissedNotes++;
            vocals.volume = 0;

            if (sustain) health -= 0.05f;
            else health -= 0.15f;

            UpdateAccuracy();

            sounds.PlayOneShot(missSounds[Mathf.RoundToInt(Random.Range(0, missSounds.Length - 1))], Random.Range(0.1f, 0.2f));
        }

        private void PlayerDeath() {
            dead = true;
            deadConfirmed = false;
            StopSong();
            StopAllCoroutines();

            sounds.clip = deathSound;
            sounds.Play();

            music.clip = deathMusic;
            music.loop = true;

            if (GameController.Instance.timeline) GameController.Instance.timeline.Stop();

            playerAnimation.Death();
            playerAnimation.GetAnimLength("Death", out int count, out float rate);
            StartCoroutine(DelayedDeathStuff((1f / rate) * count));

            // cameraCullingMask = Camera.main.cullingMask;
            // cameraClearFlags = Camera.main.clearFlags;
            // cameraBackgroundColor = Camera.main.backgroundColor;

            Camera.main.cullingMask = LayerMask.GetMask("Boyfriend");
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.backgroundColor = Color.black;

            GameplayVars.Deaths++;
        }

        private void UpdateAccuracy() {
            totalPlayed++;
            GameplayVars.Accuracy = Mathf.Round(Mathf.Max(0, notesHit / totalPlayed * 100) * 100) / 100;
        }

        IEnumerator DelayedDeathStuff(float delay) {
            yield return new WaitForSeconds(delay);
            playerAnimation.DeathLoop();
            music.Play();
        }

        IEnumerator Revive(float delay) {
            yield return new WaitForSeconds(delay);

            GameController.Instance.ReloadScene();
        }

        #endregion

        #region Opponent stuff

        private void UpdateOpponent() {
            for (int i = 0; i < 4; i++) {
                var sampler = opponentSamplers[i];
                sampler.Update(DeltaTime);

                var notes = sampler.Sample();
                foreach (var note in notes) {
                    var id = opponentDisplay.StartNote((SideColor)i, note.type, (float)note.time);
                    StartCoroutine(OpponentStopNote(id, note, (SideDir)i));
                }
            }
        }

        IEnumerator OpponentStopNote(int id, Note note, SideDir dir) {
            yield return new WaitUntil(() => note.time <= CurrentTime);
            opponentDisplay.StopNote(id);
            opponentAnimation.Sing(dir);
            vocals.volume = 1;
            camZooming = true;
        }

        #endregion

        private void StepHit() {
            if (currentStep % 4 == 0) BeatHit();
        }

        private void BeatHit() {
            if (camZooming && HudZoom < 1.35 && currentBeat % beatModulo == 0) {
                // bop in game too
                HudZoom += 0.03f;
            }

            // health
            var add = new Vector2(30, 30);
            playerHealthIcon.rectTransform.sizeDelta += add;
            opponentHealthIcon.rectTransform.sizeDelta += add;

            // gf
            if (girlfriend.IsSad) {
                if (currentBeat % gfSpeed == 0) {
                    girlfriend.Dance();
                }
            }
        }

        private void UpdateUI() {
            // zooms
            HudZoom = Mathf.Lerp(HudZoom, 1, Utils.MakeFrameIndependant(1f - 0.95f)); // figure out how to add deltatime

            // health stuffs
            var healthLerp = health / 2;

            var size = healthBar.sizeDelta;
            size.x = Mathf.Lerp(0, 593, healthLerp);
            healthBar.sizeDelta = size;

            var iconLerp = Utils.MakeFrameIndependantInverse(0.5f);
            playerHealthIcon.rectTransform.sizeDelta = Vector2.Lerp(new Vector2(150, 150), playerHealthIcon.rectTransform.sizeDelta, iconLerp);
            opponentHealthIcon.rectTransform.sizeDelta = Vector2.Lerp(new Vector2(150, 150), opponentHealthIcon.rectTransform.sizeDelta, iconLerp);

            if (healthLerp > 0.8f) opponentHealthIcon.uvRect = new Rect(0.5f, 0f, 0.5f, 1f);
            else opponentHealthIcon.uvRect = new Rect(0f, 0f, 0.5f, 1f);
            if (healthLerp < 0.2f) playerHealthIcon.uvRect = new Rect(1f, 0f, -0.5f, 1f);
            else playerHealthIcon.uvRect = new Rect(0.5f, 0f, -0.5f, 1f);

            // score lol
            Ratings.GetCurrentRating(out var rank, out var achievements);
            if (Settings.Gameplay.ShowScore) scoreText.text = $"Score: {GameplayVars.Score} | Combo Breaks: {GameplayVars.MissedNotes} | {Ratings.GetCurrentRatingString()}";
        }

        IEnumerator Countdown() {
            Vector2 originalPos = countdownSprite.rectTransform.anchoredPosition;
            float waitTime = (float)(Conductor.Crochet / 1000d);
            float currentTime = waitTime;
            int index = 0;

            countdownSprite.gameObject.SetActive(true);

            // beginning timer
            yield return new WaitForSeconds(waitTime);

            girlfriend.Dance();

            while (true) {
                playerAnimation.Idle();
                opponentAnimation.Idle();

                if (waitTime <= currentTime) {
                    if (index == countdownSounds.Length) break;

                    sounds.PlayOneShot(countdownSounds[index], 0.6f);
                    if (index != 0) {
                        countdownSprite.sprite = countdownImages[index - 1];
                        countdownSprite.color = new Color(1, 1, 1, 1);
                        countdownSprite.SetNativeSize();
                    }

                    currentTime -= waitTime;
                    index++;
                } else if (index != 1) {
                    // tween

                    float lerp = Easings.CubeInOut(currentTime / waitTime);
                    // countdownSprite.rectTransform.anchoredPosition = new Vector2(originalPos.x, originalPos.y + Mathf.Lerp(0, 100, lerp));
                    countdownSprite.color = new Color(1, 1, 1, 1 - lerp);
                }

                currentTime += Time.deltaTime;
                yield return null;
            }

            countdownSprite.gameObject.SetActive(false);

            // start music
            music.time = 0;
            vocals.time = 0;
            music.clip = songAssets.Instrumentals;
            vocals.clip = songAssets.PlayerVocals;
            music.Play();
            vocals.Play();
            if (GameController.Instance.timeline) {
                GameController.Instance.timeline.time = 0;
                GameController.Instance.timeline.Play();
            }
        }
    }
}
