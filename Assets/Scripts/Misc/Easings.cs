using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Easings {
    public static float CubeInOut(float value) {
        return value < 0.5f ? 4 * value * value * value : 1 - Mathf.Pow(-2 * value + 2, 3) / 2;
    }
}
