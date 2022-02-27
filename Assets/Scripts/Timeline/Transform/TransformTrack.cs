using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

[TrackClipType(typeof(TransformTrackAsset))]
[TrackBindingType(typeof(Transform))]
public class TransformTrack : TrackAsset {
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
        return ScriptPlayable<TransformMixerBehaviour>.Create(graph, inputCount);
    }
}
