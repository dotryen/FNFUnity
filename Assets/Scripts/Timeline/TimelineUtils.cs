using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public static class TimelineUtils {
    public static int GetBefore(Playable playable) {
        int count = playable.GetInputCount();

        for (int i = 0; i < count; i++) {
            if (playable.GetInputWeight(i) > 0f) {
                return i;
            }
        }

        return -1;
    }

    public static int GetActiveAmount(Playable playable) {
        int count = playable.GetInputCount();
        int active = 0;

        for (int i = 0; i < count; i++) {
            if (playable.GetInputWeight(i) > 0f) {
                active++;
            }
        }

        return active;
    }
}
