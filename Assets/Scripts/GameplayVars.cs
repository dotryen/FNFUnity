using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameplayVars {
    private static int combo;

    // Gameplay state
    public static bool Playing { get; set; }
    public static string Song { get; set; }
    public static double CurrentTimeMS { get; set; }
    public static double CurrentTime {
        get {
            return CurrentTimeMS / 1000f;
        }
        set {
            CurrentTimeMS = value * 1000f;
        }
    }
    public static float NoteSpeed { get; set; }

    // player stuffs
    public static int Score { get; set; }
    public static float Accuracy { get; set; }
    public static int Combo {
        get {
            return combo;
        }

        set {
            combo = value;
            if (combo > MaxCombo) MaxCombo = combo;
        }
    }
    public static int MaxCombo { get; private set; }
    public static int Deaths { get; set; }

    public static int CurrentStep => (int)Math.Floor(CurrentTimeMS / Conductor.StepCrochet);
    public static int CurrentBeat => Mathf.FloorToInt(CurrentStep / 4f);

    public static void Reset() {
        Restart();
        Song = string.Empty;
        Deaths = 0;
    }

    public static void Restart() {
        Playing = false;
        CurrentTimeMS = 0;
        NoteSpeed = 0;
        Score = 0;
        Accuracy = 0;
        Combo = 0;
    }
}
