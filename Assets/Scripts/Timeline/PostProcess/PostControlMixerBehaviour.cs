using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostControlMixerBehaviour : PlayableBehaviour {
    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        var binding = playerData as Volume;
        if (!binding) return;

        List<PostControlBehaviour> behaviours = new List<PostControlBehaviour>();
        {
            var count = playable.GetInputCount();
            for (int i = 0; i < count; i++) {
                var play = (ScriptPlayable<PostControlBehaviour>)playable.GetInput(i);
                behaviours.Add(play.GetBehaviour());
            }
        }

        if (binding.profile.TryGet<Bloom>(out var bloom)) {
            float value = 0;
            for (int i = 0; i < behaviours.Count; i++) {
                value += behaviours[i].bloomIntensity * playable.GetInputWeight(i);
            }
            bloom.intensity.value = value;
        }

        if (binding.profile.TryGet<ChromaticAberration>(out var chromaticAberration)) {
            float value = 0;
            for (int i = 0; i < behaviours.Count; i++) {
                value += behaviours[i].chromaticAbberation * playable.GetInputWeight(i);
            }
            chromaticAberration.intensity.value = value;
        }

        if (binding.profile.TryGet<ColorAdjustments>(out var colorGrading)) {
            float sat = 0;
            float hue = 0;
            for (int i = 0; i < behaviours.Count; i++) {
                sat += behaviours[i].saturation * playable.GetInputWeight(i);
                hue += behaviours[i].hueShift * playable.GetInputWeight(i);
            }
            colorGrading.saturation.value = sat;
            colorGrading.hueShift.value = hue;
        }
    }
}
