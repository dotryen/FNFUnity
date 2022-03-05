using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNF.Core {
    public enum NoteRating : int { Shit, Bad, Good, Sick }

    public enum LetterRank : byte {
        Invalid, // Default option
        AAAAA,
        AAAAColon, AAAAPeriod, AAAA,
        AAAColon, AAAPeriod, AAA,
        AAColon, AAPeriod, AA,
        AColon, APeriod, A,
        B, C, D
    }

    public enum RunAchievements : byte {
        MFC, GFC, FC, SDCB, Clear, None
    }

    public static class Ratings {
        public static float[] windows = { 160f, 135f, 90f, 45f };

        public static NoteRating Judge(double diff) {
            diff = Math.Abs(diff);

            for (int i = 0; i < windows.Length; i++) {
                var time = windows[i];
                var nextTime = i + 1 > windows.Length - 1 ? 0 : windows[i + 1];

                if (diff < time && diff >= nextTime) {
                    switch (i) {
                        case 0:
                            return NoteRating.Shit;
                        case 1:
                            return NoteRating.Bad;
                        case 2:
                            return NoteRating.Good;
                        case 3:
                            return NoteRating.Sick;
                    }
                }
            }
            return NoteRating.Good;
        }

        public static void GetCurrentRating(out LetterRank rank, out RunAchievements achievements) {
            rank = LetterRank.Invalid;
            achievements = RunAchievements.None;

            if (GameplayVars.MaxCombo == 0 && GameplayVars.MissedNotes == 0) return;

            var wifeConditions = new float[] {
                99.9935f, // AAAAA
                99.980f, // AAAA:
                99.970f, // AAAA.
                99.955f, // AAAA
                99.90f, // AAA:
                99.80f, // AAA.
                99.70f, // AAA
                99f, // AA:
                96.50f, // AA.
                93f, // AA
                90f, // A:
                85f, // A.
                80f, // A
                70f, // B
                60f, // C
            };

            bool rankAssigned = false;
            for (int i = 0; i < wifeConditions.Length; i++) {
                if (GameplayVars.Accuracy >= wifeConditions[i]) {
                    rank = (LetterRank)i + 1;
                    rankAssigned = true;
                    break;
                }
            }
            if (!rankAssigned) rank = LetterRank.D;

            if (GameplayVars.MissedNotes == 0 && GameplayVars.ShitNotes == 0 && GameplayVars.BadNotes == 0 && GameplayVars.GoodNotes == 0) achievements = RunAchievements.MFC;
            else if (GameplayVars.MissedNotes == 0 && GameplayVars.ShitNotes == 0 && GameplayVars.BadNotes == 0 && GameplayVars.GoodNotes >= 1) achievements = RunAchievements.GFC;
            else if (GameplayVars.MissedNotes == 0) achievements = RunAchievements.FC;
            else if (GameplayVars.MissedNotes < 10) achievements = RunAchievements.SDCB;
            else achievements = RunAchievements.Clear;
        }

        public static string GetCurrentRatingString() {
            GetCurrentRating(out LetterRank rank, out RunAchievements achievements);
            return $"{rank.ToStringProper()}{(rank != LetterRank.Invalid && achievements != RunAchievements.Clear ? $" ({achievements})" : "")}";
        }

        public static string ToStringProper(this LetterRank rank) {
            var result = rank.ToString();
            result = result.Replace("Invalid", "N/A");
            result = result.Replace("Period", ".");
            result = result.Replace("Colon", ":");
            return result;
        }
    }
}
