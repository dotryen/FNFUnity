using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class TransformMixerBehaviour : PlayableBehaviour {
    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        var binding = playerData as Transform;
        if (!binding) return;

        int count = TimelineUtils.GetActiveAmount(playable);
        int last = TimelineUtils.GetBefore(playable);

        if (count == 1) {
            var input = ((ScriptPlayable<TransformBehaviour>)playable.GetInput(last)).GetBehaviour();
            if (input.changePosition) binding.position = input.position;
            if (input.changeRotation) binding.rotation = Quaternion.Euler(input.rotation);
            if (input.changeScale) binding.localScale = input.scale;
        } else if (count >= 2) {
            var first = ((ScriptPlayable<TransformBehaviour>)playable.GetInput(last)).GetBehaviour();
            var second = ((ScriptPlayable<TransformBehaviour>)playable.GetInput(last + 1)).GetBehaviour();
            var weight = playable.GetInputWeight(last + 1);

            if (second.changePosition) binding.position = Vector3.Lerp(first.position, second.position, weight);
            if (second.changeRotation) binding.rotation = second.slerp ? Quaternion.Slerp(Quaternion.Euler(first.rotation), Quaternion.Euler(second.rotation), weight) : Quaternion.Euler(Vector3.Lerp(first.rotation, second.rotation, weight));
            if (second.changeScale) binding.localScale = Vector3.Lerp(first.scale, second.scale, weight);
        }
    }
}
