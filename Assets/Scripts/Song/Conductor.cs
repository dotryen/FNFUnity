using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Conductor {
    public static int BPM { get; private set; }
    public static double Crochet { get; private set; }
    public static double StepCrochet { get; private set; }
    public static double SecondsPerBeat { get; private set; }
    public static double BeatsPerSecond { get; private set; }

    public const int SAFE_FRAMES = 10;
    public const float SAFE_OFFSET = 166f; // kade lol

    public static void SetBPM(int bpm) {
        BPM = bpm;
        BeatsPerSecond = bpm / 60f;
        SecondsPerBeat = 1f / BeatsPerSecond;
        Crochet = (60f / bpm) * 1000f;
        StepCrochet = Crochet / 4f;
    }
}
