using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.Rendering;

[TrackClipType(typeof(PostControlAsset))]
[TrackBindingType(typeof(Volume))]
public class PostProcessTrack : TrackAsset {
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
        return ScriptPlayable<PostControlMixerBehaviour>.Create(graph, inputCount);
    }
}
