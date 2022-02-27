using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class CameraColorTrackAsset : PlayableAsset {
    public CameraColorBehaviour template;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
        return ScriptPlayable<CameraColorBehaviour>.Create(graph, template);
    }
}
