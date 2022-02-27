using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
    public static float MakeFrameIndependant(float elapsed, float t, float fps) {
        var frameTime = 1f / fps;
        var mult = elapsed / frameTime;
        return t * mult;
    }

    public static float MakeFrameIndependant(float t, float fps = 60) {
        return MakeFrameIndependant(Time.deltaTime, t, fps);
    }

    public static float MakeFrameIndependantInverse(float elapsed, float t, float fps) {
        return 1f - MakeFrameIndependant(elapsed, t, fps);
    }

    public static float MakeFrameIndependantInverse(float t, float fps = 60) {
        return MakeFrameIndependantInverse(Time.deltaTime, t, fps);
    }
}
