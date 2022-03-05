using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using TMPro;
using FNF.UI;
using FNF.Core;

/// <summary>
/// Recreates HaxeFlixel's volume controls.
/// </summary>
public class FlixelVolume : UIBehaviour {
    public static FlixelVolume Instance { get; private set; }

    public AudioSource source;
    public AudioMixer mixer;
    [Space]
    public TextMeshProUGUI percentText;
    public Image barFill;
    [Space]
    public float showTime = 1f;
    public float showSpeed = 5f;

    private float time;
    private Dictionary<string, InputAction> input;

    protected override void Awake() {
        base.Awake();
        Instance = this;
    }

    private void Start() {
        input = InputManager.GetMapActions("Volume");
    }

    private void Update() {
        time -= Time.unscaledDeltaTime;

        {
            int volMod = 0;
            // plus happens when holding shift, they share a key soooo
            if (input["VolumeUp"].WasPressedThisFrame()) volMod++;
            if (input["VolumeDown"].WasPressedThisFrame()) volMod--;
            if (input["Mute"].WasPressedThisFrame()) volMod = -Mathf.RoundToInt(Settings.Audio.Volume * 10f);

            if (volMod != 0) {
                time = showTime;
                Settings.Audio.Volume += volMod * 0.1f;
                percentText.text = Settings.Audio.Volume * 100 + "%";
                barFill.rectTransform.sizeDelta = new Vector2(65 * Settings.Audio.Volume, barFill.rectTransform.sizeDelta.y);
                source.PlayOneShot(source.clip);
            }
        }
        
        
        {
            int posMod = time > 0 ? -1 : 1;
            var pos = new Vector2(0, transform.anchoredPosition.y);
            pos.y += showSpeed * Time.unscaledDeltaTime * posMod;
            pos.y = Mathf.Clamp(pos.y, 0, 65);

            transform.anchoredPosition = pos;
        }
    }
}
