using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FNF.UI {
    using Core;

    public class FreeplayButton : MenuOption {
        public LoadSongButton songTemplate;

        private void Start() {
            StartCoroutine(LoadSongs());
        }

        private IEnumerator LoadSongs() {
            var songs = SongAssets.GetAllSongs();

            foreach (var song in songs) {
                var assets = SongAssets.GetSong(song.Key);

                yield return assets.LoadMeta();
                var meta = assets.Metadata;

                var section = Instantiate(songTemplate, submenuTrans);

                {
                    var minutes = Mathf.FloorToInt(meta.length / 60);
                    var seconds = Mathf.FloorToInt(meta.length % 60).ToString();
                    if (seconds.Length == 1) seconds = "0" + seconds;

                    section.defaultText = meta.songName;
                    section.description = $"Length: {minutes}:{seconds}, BPM: {meta.bpm}";
                }

                foreach (var diff in meta.difficulties) {
                    var button = Instantiate(songTemplate, section.submenuTrans);
                    var score = Scores.GetScore(assets.Name + "-" + diff);

                    button.defaultText = diff;
                    button.description = $"Highscore: {score.score} | Max Combo: {score.maxCombo} | Rating: {score.rank.ToStringProper()}{(score.rank != LetterRank.Invalid && score.achievements != RunAchievements.Clear ? $" ({score.achievements})" : "")}";

                    button.songName = song.Key;
                    button.difficulty = diff;

                    button.gameObject.SetActive(true);
                }

                section.gameObject.SetActive(true);
            }

            yield return null;
            RefreshSubmenu();
            description = "Play any song of your choosing";
        }
    }
}
