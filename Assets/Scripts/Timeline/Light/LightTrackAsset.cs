using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class LightTrackAsset : PlayableAsset {
    public LightBehaviour template;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
        return ScriptPlayable<LightBehaviour>.Create(graph, template);
    }
}
