using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class CameraColorMixerBehaviour : PlayableBehaviour {
    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        var binding = playerData as Camera;
        if (!binding) return;

        int count = TimelineUtils.GetActiveAmount(playable);
        int last = TimelineUtils.GetBefore(playable);

        if (count == 1) {
            var input = ((ScriptPlayable<CameraColorBehaviour>)playable.GetInput(last)).GetBehaviour();
            binding.backgroundColor = input.backgroundColor;
        } else if (count >= 2) {
            var first = ((ScriptPlayable<CameraColorBehaviour>)playable.GetInput(last)).GetBehaviour();
            var second = ((ScriptPlayable<CameraColorBehaviour>)playable.GetInput(last + 1)).GetBehaviour();
            var weight = playable.GetInputWeight(last + 1);

            binding.backgroundColor = Color.Lerp(first.backgroundColor, second.backgroundColor, weight);
        }
    }
}
