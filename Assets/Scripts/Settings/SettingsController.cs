using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsController {
    private static Dictionary<string, Settings.Setting> settings;
    private static Settings.Setting current;

    public static object CurrentValue => current.property.GetValue(null);

    public static void Initialize() {
        settings = Settings.GetSettings();
    }

    public static bool Select(string key) {
        return settings.TryGetValue(key, out current);
    }

    public static object Left() {
        if (current == null) return null;
        object value = current.property.GetValue(null);

        switch (current.code) {
            case TypeCode.Single:
                value = Mathf.Clamp(Mathf.Round(((float)value - 0.1f) * 10) / 10, current.min, current.max);
                break;
            case TypeCode.Boolean:
                value = !(bool)value;
                break;
            case TypeCode.Int32:
                value = Clamp((int)value - 1, current.min, current.max);
                break;
        }

        current.property.SetValue(null, value);
        return value;
    }

    public static object Right() {
        if (current == null) return null;
        object value = current.property.GetValue(null);

        switch (current.code) {
            case TypeCode.Single:
                value = Mathf.Clamp(Mathf.Round(((float)value + 0.1f) * 10) / 10, current.min, current.max);
                break;
            case TypeCode.Boolean:
                value = !(bool)value;
                break;
            case TypeCode.Int32:
                value = Clamp((int)value + 1, current.min, current.max);
                break;
        }

        current.property.SetValue(null, value);
        return value;
    }

    private static int Clamp(int value, int min, int max) {
        if (value < min) return min;
        else if (value > max) return max;
        else return value;
    }
}
