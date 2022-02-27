using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class OrdealBoyfriendCallbacks : MonoBehaviour {
    public ParticleSystem[] distortion;
    public float distortionTimeLimit;
    [Space]
    public new CinemachineVirtualCamera camera;
    public CinemachineImpulseSource impulse;
    public float impulseForce = 1f;
    public float shakeTimelimit;
    public float shakeAmount;

    private Vector3 impulseVector;

    private bool hitThisFrame;
    private float hitTimer;
    private float[] distortionTimes;
    private CinemachineBasicMultiChannelPerlin cameraShake;

    private void Start() {
        distortionTimes = new float[distortion.Length];
        cameraShake = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void LateUpdate() {
        for (int i = 0; i < distortionTimes.Length; i++) {
            if (distortionTimes[i] >= distortionTimeLimit) {
                SetEmission(i, false);
            } else {
                distortionTimes[i] += Time.deltaTime;
            }
        }

        if (hitTimer >= shakeTimelimit) {
            cameraShake.m_AmplitudeGain = 0f;
            impulseVector = Vector3.zero;
        } else {
            hitTimer += Time.deltaTime;
        }

        if (hitThisFrame) {
            if (impulseVector.magnitude <= 0.1f) {
                cameraShake.m_AmplitudeGain = shakeAmount;
            } else {
                impulse.GenerateImpulseWithVelocity(impulseVector * impulseForce);
            }
        }

        hitThisFrame = false;
    }

    public void OnSing(SideDir dir) {
        SetEmission((int)dir, true);

        impulseVector += Direction(dir);
        hitTimer = 0f;
        hitThisFrame = true;
    }

    private void SetEmission(int index, bool value) {
        var emission = distortion[index].emission;
        emission.enabled = value;
        if (value) distortionTimes[index] = 0;
    }

    private Vector3 Direction(SideDir dir) {
        if (dir == SideDir.Left) return Vector3.left;
        if (dir == SideDir.Right) return Vector3.right;
        if (dir == SideDir.Up) return Vector3.up;
        return Vector3.down;
    }
}
