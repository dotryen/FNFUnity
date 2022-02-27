using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseSinger : BeatBehaviour {
    [Header("Options")]
    public float multiplier = 4f;
    public int beatMod = 2;

    [Header("Events")]
    public UnityEvent<SideDir> OnSing;
    public UnityEvent<SideDir> OnMiss;

    protected float holdTime;
    protected bool holding;
    protected bool missed;
    protected float idleSpeed;

    protected float HoldThreshold => (float)Conductor.StepCrochet * multiplier * 0.001f;

    private void Start() {
        GetAnimLength("Idle", out int frameCount, out float fps);
        var length = frameCount * (1f / fps);
        idleSpeed = Mathf.Max(1, length / (float)Conductor.SecondsPerBeat / beatMod);
    }

    protected override void Update() {
        base.Update();
        holdTime += Time.deltaTime;

        if (holdTime >= HoldThreshold && holding) {
            int offset = missed ? 10 : 0;
            Idle(offset);

            holding = false;
            missed = false;
        }
    }

    protected override void OnBeat() {
        if (holding || GameplayVars.CurrentBeat % beatMod != 0) return;
        Idle();
    }

    public void Idle(int offset = 0) {
        SetAnimation("Idle", offset, idleSpeed);
    }

    public void Death() {
        holding = false;
        SetAnimation("Death", 0, 1);
    }

    public void DeathLoop() {
        SetAnimation("DeathLoop", 0, 1, true);
    }

    public void DeathConfirm() {
        SetAnimation("DeathConfirm", 0, 1);
    }

    public void Sing(SideDir dir) {
        Hold();
        SetAnimation("Note" + System.Enum.GetName(typeof(SideDir), dir), 0, 1);
        OnSing.Invoke(dir);
    }

    public void Miss(SideDir dir) {
        Hold();
        SetAnimation("Note" + System.Enum.GetName(typeof(SideDir), dir) + "Miss", 0, 1);
        missed = true;
        OnMiss.Invoke(dir);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Hold() {
        holdTime = 0f;
        holding = true;
    }

    public void SetAnimationSimple(string name) {
        SetAnimation(name, 0, 1, false);
    }

    public abstract void SetAnimation(string name, int frame, float speed, bool loop = false);

    public abstract void GetAnimLength(string name, out int frameCount, out float fps);
}
