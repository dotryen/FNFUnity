using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class LightMixerBehaviour : PlayableBehaviour {
    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        var binding = playerData as Light;
        if (!binding) return;

        int count = TimelineUtils.GetActiveAmount(playable);
        int last = TimelineUtils.GetBefore(playable);

        if (count == 1) {
            var input = ((ScriptPlayable<LightBehaviour>)playable.GetInput(last)).GetBehaviour();
            binding.color = input.color;
            binding.intensity = input.intensity;
        } else if (count >= 2) {
            var first = ((ScriptPlayable<LightBehaviour>)playable.GetInput(last)).GetBehaviour();
            var second = ((ScriptPlayable<LightBehaviour>)playable.GetInput(last + 1)).GetBehaviour();
            var weight = playable.GetInputWeight(last + 1);

            binding.color = Color.Lerp(first.color, second.color, weight);
            binding.intensity = Mathf.Lerp(first.intensity, second.intensity, weight);
        }
    }
}
