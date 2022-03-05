using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Serialization;

namespace FNF.Core {
    public static class Scores {
        [Serializable]
        public struct ScoreData {
            public int score;
            public int maxCombo;
            public LetterRank rank;
            public RunAchievements achievements;
        }

        public const string FILE_NAME = "scores";

        private static Dictionary<string, ScoreData> scores = new Dictionary<string, ScoreData>();

        public static void Save() {
#if UNITY_EDITOR
            return;
#endif

            BinaryDataStream.Save(scores, FILE_NAME);
        }

        [RuntimeInitializeOnLoadMethod]
        public static void Load() {
#if UNITY_EDITOR
            return;
#endif

            scores = BinaryDataStream.Read<Dictionary<string, ScoreData>>(FILE_NAME);
        }

        public static void SetScore(string song, ScoreData score) {
            if (scores.ContainsKey(song)) scores[song] = score;
            else scores.Add(song, score);
        }

        public static ScoreData GetScore(string song) {
            ScoreData score = default;
            if (scores.ContainsKey(song)) score = scores[song];

            return score;
        }
    }
}
