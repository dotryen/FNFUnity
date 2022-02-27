using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightRandomizer : MonoBehaviour {
    [Range(0f, 1f)]
    public float minThreshold = 0f;
    [Range(0f, 1f)]
    public float maxThreshold = 1f;

    private new Light light;

    private void Awake() {
        light = GetComponent<Light>();
    }

    private void Update() {
        light.color = new Color(GetValue(), GetValue(), GetValue());
    }

    private float GetValue() {
        return Random.Range(minThreshold, maxThreshold);
    }
}
