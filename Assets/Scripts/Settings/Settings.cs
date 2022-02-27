using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

// Gotta preserve everything in this file or the stripping will get to it

[Preserve]
public static class Settings {
    [SettingAttributes(description = "gamer")]
    [Preserve]
    public static class Gameplay {
        [SettingAttributes(description = "there are people that use this not at zero???????????")]
        [Preserve]
        public static int Offset { get; set; } = 0;
        [SettingAttributes(description = "Show score and accuracy at the bottom of the screen.", name = "Show Score Mid Game")]
        [Preserve]
        public static bool ShowScore { get; set; } = false;
    }

    [SettingAttributes(description = "Things that you will notice, when you look at things.")]
    [Preserve]
    public static class Visuals {
        private static bool fullscreen = true;
        private static bool vsync = true;

        [SettingAttributes(description = "Do you want a window, with borders?")]
        [Preserve]
        public static bool Fullscreen {
            get {
                return fullscreen;
            }

            set {
                fullscreen = value;
                Screen.fullScreen = fullscreen;
            }
        }
        [SettingAttributes(description = "Prevents tearing and might make things look smoother.")]
        [Preserve]
        public static bool VSync {
            get {
                return vsync;
            }

            set {
                vsync = value;
                QualitySettings.vSyncCount = vsync ? 1 : 0;
            }
        }
        [SettingAttributes(description = "Smooths jagged edges.", min = 0, max = 2)]
        [Preserve]
        public static Antialiasing Antialiasing { get; set; } = Antialiasing.FXAA;
        [SettingAttributes(description = "Those cool effects we use. (Does not turn off glitching)", name = "Post Processing")]
        [Preserve]
        public static bool PostProcessing { get; set; } = true;
    }

    [SettingAttributes(description = "The vibrations won't stop.")]
    [Preserve]
    public static class Audio {
        private static float masterVol = 1f;

        [SettingAttributes(description = "How loudy?")]
        [Preserve]
        public static float Volume { 
            get {
                return masterVol;
            }

            set {
                masterVol = Mathf.Clamp01(Mathf.Round(value * 100f) / 100f);
                var decibel = masterVol == 0f ? -144.0f : 20f * Mathf.Log(masterVol);
                FlixelVolume.Instance.mixer.SetFloat("MasterVol", decibel);
            }
        }
        [SettingAttributes(description = "Use new clipping system (Might piss off some users)", name = "Vocal Clipping")]
        [Preserve]
        public static bool VocalClipping { get; set; } = true;
    }

    private static bool initialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Intialize() {
        if (initialized) return;

        var settings = GetSettings();
        foreach (var setting in settings) {
            object value = setting.Value.property.GetValue(null);
            switch (setting.Value.code) {
                case TypeCode.Single:
                    value = PlayerPrefs.GetFloat(setting.Key, (float)value);
                    break;
                case TypeCode.Boolean:
                    value = PlayerPrefs.GetInt(setting.Key, (bool)value ? 1 : 0) == 1;
                    break;
                case TypeCode.Int32:
                    value = PlayerPrefs.GetInt(setting.Key, (int)value);
                    break;
            }

            setting.Value.property.SetValue(null, value);
        }
        initialized = true;
        Application.quitting += () => {
            Save();
        };
    }

    private static void Save() {
        var settings = GetSettings();
        foreach (var setting in settings) {
            switch (setting.Value.code) {
                case TypeCode.Single:
                    PlayerPrefs.SetFloat(setting.Key, (float)setting.Value.property.GetValue(null));
                    break;
                case TypeCode.Boolean: 
                    PlayerPrefs.SetInt(setting.Key, (bool)setting.Value.property.GetValue(null) ? 1 : 0);
                    break;
                case TypeCode.Int32:
                    PlayerPrefs.SetInt(setting.Key, (int)setting.Value.property.GetValue(null));
                    break;
            }
        }

        PlayerPrefs.Save();
    }

    public static Dictionary<string, Setting> GetSettings() {
        var dic = new Dictionary<string, Setting>();
        foreach (var sectionType in typeof(Settings).GetNestedTypes()) {
            var sectionAttr = (SettingAttributes)sectionType.GetCustomAttribute(typeof(SettingAttributes));
            string name = sectionType.Name;
            string description = string.Empty;

            if (sectionAttr != null) {
                if (!string.IsNullOrEmpty(sectionAttr.name)) name = sectionAttr.name;
                description = sectionAttr.description;
            }

            foreach (var prop in sectionType.GetProperties()) {
                dic.Add($"{sectionType.Name}_{prop.Name}", new Setting(prop, name, description));
            }
        }
        return dic;
    }

    public class Setting {
        public string sectionName;
        public string sectionDescription;
        public PropertyInfo property;
        public TypeCode code;
        public string displayName;
        public string description;
        public string prefKey;
        public int min;
        public int max;

        public Setting(PropertyInfo info, string secName, string secDesc) {
            sectionName = secName;
            sectionDescription = secDesc;
            property = info;
            code = Type.GetTypeCode(info.PropertyType);
            prefKey = secName + "_" + info.Name;

            var attr = (SettingAttributes)property.GetCustomAttribute(typeof(SettingAttributes));
            if (attr != null) {
                if (!string.IsNullOrEmpty(attr.name)) displayName = attr.name;
                else displayName = property.Name;
                description = attr.description;
                min = attr.min;
                max = attr.max;
            } else {
                displayName = property.Name;
                description = string.Empty;
                min = int.MinValue;
                max = int.MaxValue;
            }
        }
    }
}
