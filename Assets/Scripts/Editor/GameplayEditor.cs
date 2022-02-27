using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Gameplay))]
public class GameplayEditor : Editor {
    private Gameplay Target => (Gameplay)target;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (!Application.isPlaying) return;
        EditorGUILayout.Space();
        if (GUILayout.Button("Start song")) {
            Target.StartSong();
        }
        if (GUILayout.Button("Pause")) {
            Target.PauseSong();
        }
        if (GUILayout.Button("Resume")) {
            Target.ResumeSong();
        }
    }
}
