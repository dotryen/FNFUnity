using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class TransformTrackAsset : PlayableAsset {
    public TransformBehaviour template;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
        return ScriptPlayable<TransformBehaviour>.Create(graph, template);
    }
}
