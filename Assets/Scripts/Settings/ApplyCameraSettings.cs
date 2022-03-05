using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ApplyCameraSettings : MonoBehaviour {
    public Camera layer;
    public UniversalAdditionalCameraData data;
    public Volume volume;

    private void Awake() {
        data.antialiasing = GetAntialiasing(Settings.Visuals.Antialiasing);
        // volume.gameObject.SetActive(Settings.Visuals.PostProcessing);
        
        if (!Settings.Visuals.PostProcessing) {
            var profile = volume.profile;
            foreach (var setting in profile.components) {
                // you cant use the not keyword in this case (stupid fucking unity mono)
                if (!(setting is DigitalGlitch) && !(setting is AnalogGlitch) && !(setting is LensDistortion)) {
                    setting.active = false;
                }
            }
        }
    }

    private AntialiasingMode GetAntialiasing(Antialiasing antialiasing) {
        switch (antialiasing) {
            default:
                return AntialiasingMode.None;
            case Antialiasing.FXAA:
                return AntialiasingMode.FastApproximateAntialiasing;
            case Antialiasing.SMAA:
                return AntialiasingMode.SubpixelMorphologicalAntiAliasing;
            // case Antialiasing.TAA: // ugly ass
            //     return PostProcessLayer.Antialiasing.TemporalAntialiasing;
        }
    }
}
