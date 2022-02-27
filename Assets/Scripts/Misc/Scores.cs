using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class Scores {
    public const string FILE_NAME = "scores";

    private static Dictionary<int, SongScore> scores = new Dictionary<int, SongScore>();

    private static string FilePath => Application.persistentDataPath +  "\\" + FILE_NAME;

    public static void Save() {
        #if UNITY_EDITOR
        return;
        #endif

        byte[] buffer = new byte[(SongScore.SIZE * scores.Count) + 4];
        SerializeInt(buffer, 0, scores.Count);

        for (int i = 0; i < scores.Count; i++) {
            var offset = (i * SongScore.SIZE) + 4;
            var score = scores.ElementAt(i).Value;
            SerializeInt(buffer, offset, score.hash);
            SerializeInt(buffer, offset + 4, score.score);
            SerializeInt(buffer, offset + 8, score.maxCombo);
        }

        if (!File.Exists(FilePath)) {
            var stream = File.Create(FilePath);
            stream.SetLength(0); // discards current bytes
            stream.Write(buffer);
        } else {
            File.WriteAllBytes(FilePath, buffer);
        }
    }

    [RuntimeInitializeOnLoadMethod]
    public static void Load() {
        #if UNITY_EDITOR
        return;
        #endif

        if (!File.Exists(FilePath)) return;

        var buffer = File.ReadAllBytes(FilePath);
        var length = DeserializeInt(buffer, 0);

        for (int i = 0; i < length; i++) {
            var offset = (i * SongScore.SIZE) + 4;
            var score = new SongScore();
            score.hash = DeserializeInt(buffer, offset);
            score.score = DeserializeInt(buffer, offset + 4);
            score.maxCombo = DeserializeInt(buffer, offset + 8);

            scores.Add(score.hash, score);
        }
    }

    public static void SetScore(string song, SongScore score) {
        var hash = HashFromName(song);
        score.hash = hash;

        if (scores.ContainsKey(hash)) scores[hash] = score;
        else scores.Add(hash, score);
    }

    public static SongScore GetScore(string song) {
        var hash = HashFromName(song);

        SongScore score = default;
        if (scores.ContainsKey(hash)) score = scores[hash];

        return score;
    }

    private static int HashFromName(string name) {
        unchecked {
            int hash = 23;
            foreach (char c in name) {
                hash = hash * 31 + c;
            }
            return hash;
        }
    }

    private static void SerializeInt(byte[] buffer, int offset, int value) {
        var serial = BitConverter.GetBytes(value);
        Array.Copy(serial, 0, buffer, offset, 4);
    }

    private static int DeserializeInt(byte[] buffer, int offset) {
        return BitConverter.ToInt32(buffer, offset);
    }
}

public struct SongScore {
    public int hash;
    public int score;
    public int maxCombo;

    public const int SIZE = 12;
}
