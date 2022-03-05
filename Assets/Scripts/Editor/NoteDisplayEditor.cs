using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FNF.Core;

[CustomEditor(typeof(NoteDisplay))]
public class NoteDisplayEditor : Editor {
    public SideColor side;

    public NoteDisplay Comp => (NoteDisplay)target;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (!EditorApplication.isPlaying) return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Spawning controls");

        side = (SideColor)EditorGUILayout.EnumPopup("Side to spawn", side);
        // if (GUILayout.Button("Spawn")) {
        //     Comp.StartNote(side);
        // }
    }
}
