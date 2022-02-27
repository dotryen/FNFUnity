using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

[TrackClipType(typeof(LightTrackAsset))]
[TrackBindingType(typeof(Light))]
public class LightTrack : TrackAsset {
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
        return ScriptPlayable<LightMixerBehaviour>.Create(graph, inputCount);
    }
}
