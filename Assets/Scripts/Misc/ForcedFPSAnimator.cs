using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ForcedFPSAnimator : MonoBehaviour {
    public int fps;

    private Animator animator;
    private float threshold;
    private float timer;

    private void Awake() {
        animator = GetComponent<Animator>();
        animator.enabled = false;
        SetFPS(fps);
    }

    private void Update() {
        if (threshold <= timer) {
            animator.Update(threshold);
            timer -= threshold;
        }

        timer += Time.deltaTime;
    }

    public void SetFPS(int fps) {
        threshold = 1f / fps;
        timer = 0f;
    }
}
