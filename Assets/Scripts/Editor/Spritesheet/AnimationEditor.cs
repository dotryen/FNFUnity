using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpriteAnimation), true), CanEditMultipleObjects]
public class AnimationEditor : Editor {
    string animation = "";
    bool show = false;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        var comp = (SpriteAnimation)target;

        if (Application.isPlaying) {
            show = EditorGUILayout.Foldout(show, "Animations");
            if (show && comp.sprite) {
                foreach (var animation in comp.sprite.data.animations) {
                    if (GUILayout.Button("Play " + animation.name)) {
                        comp.SetAnimation(animation.name);
                    }
                }
            }
        } else {
            EditorGUILayout.LabelField("Preview Animation (Potentially Unsafe)", EditorStyles.boldLabel);
            animation = EditorGUILayout.TextField("Animation", animation);
            if (GUILayout.Button("Preview")) {
                comp.sprite.SetupSprite();
                comp.SetAnimation(animation);
            }
        }
    }
}
