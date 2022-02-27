using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LerpInterpreter : EditorWindow {
    [MenuItem("Tools/Lerp Interpret")]
    static void ShowWindow() {
        GetWindow<LerpInterpreter>().Show();
    }

    private int frameRate = 60;
    private float initialValue = 0f;
    private float targetValue = 10f;
    private float lerpAmount = 0.95f;
    private float result = 0;
    private float higherFps = 0;

    private float scaled1 = 0f;
    private float scaled2 = 0f;

    private void OnGUI() {
        frameRate = EditorGUILayout.IntField("Frame Rate", frameRate);
        initialValue = EditorGUILayout.FloatField("Initial Value", initialValue);
        targetValue = EditorGUILayout.FloatField("Target Value", targetValue);
        lerpAmount = EditorGUILayout.FloatField("Lerp Amount", Mathf.Clamp01(lerpAmount));

        if (GUILayout.Button("Run")) {
            result = RunCalc(frameRate, lerpAmount);
            higherFps = RunCalc(144, lerpAmount);

            var scaled = lerpAmount / FrameTime(frameRate);
            scaled1 = RunCalc(frameRate, scaled * FrameTime(frameRate)); // will always match, mathematic rules dummy
            scaled2 = RunCalc(144, scaled * FrameTime(144));
        }

        EditorGUILayout.LabelField($"Inverse: {lerpAmount / FrameTime(frameRate)}", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"The amount of time needed at {frameRate} FPS is: {result}", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"144 FPS: {higherFps}", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Scaled at 60 FPS: {scaled1}", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Scaled at 144 FPS (Approximate): {scaled2}", EditorStyles.boldLabel);
    }

    private float RunCalc(int fps, float lerp) {
        float frame = FrameTime(fps);
        float currentTime = 0f;
        var value = initialValue;

        while (value != targetValue) {
            value = Mathf.Lerp(value, targetValue, lerp);
            currentTime += frame;
            if (currentTime > fps * 10 * frame) {
                Debug.LogError("Failsafe triggered");
                break;
            }
        }

        return currentTime;
    }

    private float RunCalcApprox(int fps, float lerp) {
        float frame = FrameTime(fps);
        float currentTime = 0f;
        var value = initialValue;

        while (!Mathf.Approximately(value, targetValue)) {
            value = Mathf.Lerp(value, targetValue, lerp);
            currentTime += frame;
            if (currentTime > fps * 10 * frame) {
                Debug.LogError("Failsafe triggered");
                break;
            }
        }

        return currentTime;
    }

    private float FrameTime(int frameRate) {
        return 1f / frameRate;
    }
}
