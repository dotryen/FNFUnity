using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

[TrackClipType(typeof(CameraColorTrackAsset))]
[TrackBindingType(typeof(Camera))]
public class CameraColorTrack : TrackAsset {
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
        return ScriptPlayable<CameraColorMixerBehaviour>.Create(graph, inputCount);
    }
}
