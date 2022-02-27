using System;
using System.Collections;
using System.Collections.Generic;

public enum Rating : int { Shit, Bad, Good, Sick }

public static class Ratings {
    public static float[] windows = { 160f, 135f, 90f, 45f };

    public static Rating Judge(double diff) {
        diff = Math.Abs(diff);

        for (int i = 0; i < windows.Length; i++) {
            var time = windows[i];
            var nextTime = i + 1 > windows.Length - 1 ? 0 : windows[i + 1];

            if (diff < time && diff >= nextTime) {
                switch (i) {
                    case 0:
                        return Rating.Shit;
                    case 1:
                        return Rating.Bad;
                    case 2:
                        return Rating.Good;
                    case 3:
                        return Rating.Sick;
                }
            }
        }
        return Rating.Good;
    }
}
